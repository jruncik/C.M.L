/* ------------------------------------------------------------------------- *
 * Copyright (C) 2008-2009 Jaroslav Runcik
 *
 * Jaroslav Runcik <J [dot] Runcik [at] seznam [dot] cz>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * ------------------------------------------------------------------------- */

using System;

using System.IO;
using System.Reflection;
using System.Collections.Generic;

using SR.CML.Core.Plugins;
using SR.CML.Core.Actions;

using log4net;

using System.Diagnostics;

namespace SR.CML.Core
{
	internal class PluginManager : IPluginManager
	{
		private static ILog	_log		= LogManager.GetLogger(typeof(PluginManager));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool							_disposed;
		private Dictionary<Guid, PluginHolder>	_plugins;
		private PluginHolder					_rootPlugin;
		private PluginTreeTraverser				_traverser;
		private String							_pluginsSubDirectory;
		private static Type						PLUGIN_ATTR_TYPE = typeof(PluginAttribute);

		internal PluginManager(String pluginsSubDiretory)
		{
			_pluginsSubDirectory = pluginsSubDiretory;

			if (String.IsNullOrEmpty(pluginsSubDiretory)) {
				_log.Fatal("Path to Plugins-SubDirectory isn't set!");
			} else {
				if (!Directory.Exists(_pluginsSubDirectory)) {
					Debug.Assert(false, "Plugins-SubDirectory doesn't exist!");
					_log.Fatal("Plugin-SubDirectory doesn't exist!");
					throw new ArgumentException("Plugin-SubDirectory doesn't exist!");
				}
			}

			_logDebug		= _log.IsDebugEnabled;
			_plugins		= new Dictionary<Guid,PluginHolder>(16);

			if (_logDebug) {
				_log.Debug("PluginManager initialized");
			}
		}

		~PluginManager()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "PluginManager, call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing) {
				DeactivateAllInternal();
				DisposeAllPlugins();
				_traverser = null;

				if (_plugins!=null) {
					_plugins.Clear();
					_plugins = null;
				}

				if (_logDebug) {
					_log.Debug("Disposed");
				}
			}
			_disposed = true;
		}

		#region IPluginManager

		public event EventHandler<EventArgs> PluginActivated;

		public IPlugin GetPlugin(Guid Id)
		{
			if (_plugins.ContainsKey(Id)) {
				return _plugins[Id].Plugin;
			}

			return null;
		}

		public IList<IPlugin> Plugins
		{
			get
			{
				IPlugin[]	pluginsArray	= new IPlugin[_plugins.Count];
				Int32		index			= 0;

				foreach(PluginHolder pluginHolder in _plugins.Values) {
					pluginsArray[index++] = pluginHolder.Plugin;
				}

				return Array.AsReadOnly<IPlugin>(pluginsArray);
			}
		}

		public void LoadPlugins()
		{
			if (_logDebug) {
				_log.Debug(String.Format("Loading plugins from '{0}'.", AppDomain.CurrentDomain.BaseDirectory + _pluginsSubDirectory));
			}

			PluginDependancyResolver	dependancyResolver = new PluginDependancyResolver();
			Assembly					assembly;
			List<PluginDescription>		pluginFiles;
			DirectoryInfo				di = new DirectoryInfo(_pluginsSubDirectory);

			pluginFiles = GetPluginsFiles();

			foreach (PluginDescription pluginDescription in pluginFiles) {
				if (_logDebug) {
					_log.Debug(String.Format("Checking file '{0}'.", Path.GetFileName(pluginDescription.FullName)));
				}

				assembly = Assembly.LoadFrom(pluginDescription.FullName);
				FindPluginsInAssembly(assembly, dependancyResolver);
			}

			if (dependancyResolver.ResolveDependancy()) {
				_rootPlugin	= dependancyResolver.RootPlugin;
				_traverser	= new PluginTreeTraverser(_rootPlugin);

				FillPluginDictionary();

			} else {
				throw new Exception("Plugin dependency wasn't resolved");
			}
		}

		public bool ActivatePlugin(Guid pluginID)
		{
			try {
				SelectPlugin(pluginID);
				InitializeSelectedPlugins();
				ActivateSelectedPlugins();
				return true;

			} catch (KeyNotFoundException ex) {
				_log.Error(ex.Message);
			}

			return false;
		}

		public void DeactivateAll()
		{
			DeactivateAllInternal();
		}

		#endregion

		private bool SelectPlugin(Guid pluginID)
		{
			PluginHolder plugin = _plugins[pluginID];
			Queue<PluginHolder> pluginsForSelect = new Queue<PluginHolder>();

			plugin.Selected = true;
			foreach(PluginHolder pluginHolder in plugin.DependsOn) {
				pluginsForSelect.Enqueue(pluginHolder);
			}

			while (pluginsForSelect.Count>0) {
				PluginHolder item = pluginsForSelect.Dequeue();
				item.Selected = true;

				foreach(PluginHolder pluginHolder in item.DependsOn) {
					pluginsForSelect.Enqueue(pluginHolder);
				}
			}

			return true;
		}

		private void FindPluginsInAssembly(Assembly assembly, PluginDependancyResolver dependancyResolver)
		{
			foreach (Type type in assembly.GetTypes()) {
				foreach (Attribute attribute in Attribute.GetCustomAttributes(type)) {

					if (attribute.GetType() == PLUGIN_ATTR_TYPE) {
						PluginAttribute pluginAttribute = attribute as PluginAttribute;
						Debug.Assert(pluginAttribute != null);
						dependancyResolver.RegisterPlugin(new PluginHolder(assembly, type.FullName, pluginAttribute));
					}
				}
			}
		}

		internal void InstanciateAllPlugins()
		{
			if (_traverser==null) {
				Debug.Assert(_plugins.Count==0);
				return;
			}

			InstanciateAction instanciateAction = new InstanciateAction();
			_traverser.TraverseFromTop(instanciateAction);
		}

		private void InitializeSelectedPlugins()
		{
			if (_traverser==null) {
				Debug.Assert(_plugins.Count==0);
				return;
			}

			InitializeAction initialize = new InitializeAction(this);
			_traverser.TraverseFromTop(initialize);
		}

		private void ActivateSelectedPlugins()
		{
			if (_traverser==null) {
				Debug.Assert(_plugins.Count==0);
				return;
			}

			ActivateAction activate = new ActivateAction();
			_traverser.TraverseFromTop(activate);

			OnPluginActivated();
		}

		private void DeactivateAllInternal()
		{
			if (_traverser==null) {
				Debug.Assert(_plugins.Count==0);
				return;
			}

			DeactivateAllAction deactivate = new DeactivateAllAction();
			_traverser.TraverseFromBottom(deactivate);
		}

		private void FillPluginDictionary()
		{
			if (_traverser==null) {
				return;
			}

			PluginsCollectorAction collector = new PluginsCollectorAction();
			_traverser.TraverseFromTop(collector);

			_plugins.Clear();
			foreach(PluginHolder item in collector.Plugins) {
				_plugins.Add(item.Id, item);
			}
		}

		private void OnPluginActivated()
		{
			if (PluginActivated!=null) {
				PluginActivated(this, new EventArgs());
			}
		}

		private void DisposeAllPlugins()
		{
			foreach(PluginHolder pluginHolder in _plugins.Values) {
				pluginHolder.Dispose();
			}
		}

		/// <summary>
		/// Find plugins from plugins sub directory. It has to be loaded in other AppDomain because Assembly.Unload doesn't work.
		/// </summary>
		/// <returns>List of dlls with supported plugins.</returns>
		private List<PluginDescription> GetPluginsFiles()
		{
			List<PluginDescription>	pluginsFiles		= null;
			PluginExplorer			explorer			= null;
			AppDomainSetup			appDomainSetup		= new AppDomainSetup();

			appDomainSetup.ApplicationBase	= AppDomain.CurrentDomain.BaseDirectory;
			appDomainSetup.PrivateBinPath	= AppDomain.CurrentDomain.BaseDirectory + _pluginsSubDirectory;

			AppDomain appDomainExplorer = AppDomain.CreateDomain("PluginExplorer", null, appDomainSetup);

			try {
				explorer = appDomainExplorer.CreateInstanceAndUnwrap("SR.CML.Core", "SR.CML.Core.PluginExplorer") as PluginExplorer;

				Debug.Assert(explorer != null);
				if (explorer!=null) {
					pluginsFiles = explorer.GetPluginList();
				} else {
					_log.Error("PluginExplorer wan't created!");
				}

			} catch (Exception ex) {
				_log.Error(ex.Message);

			} finally {
				AppDomain.Unload(appDomainExplorer);
			}

			return pluginsFiles;
		}
	}
}



