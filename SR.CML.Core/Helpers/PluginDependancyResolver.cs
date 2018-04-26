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

using System.Collections.Generic;

using SR.CML.Core.Plugins;

using log4net;

using System.Diagnostics;

namespace SR.CML.Core
{
	internal class PluginDependancyResolver
	{
		private static ILog	_log		= LogManager.GetLogger(typeof(PluginDependancyResolver));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private IList<DependancyResolverItem>	_registeredPlugins;
		private IList<DependancyResolverItem>	_resoledPlugins;
		private IList<DependancyResolverItem>	_waitingForResolution;

		private DependancyResolverItem _rootPluginItem;
		internal PluginHolder RootPlugin
		{
			get { return _rootPluginItem.PluginHolder; }
		}

		internal IList<PluginHolder> UnresolvedPlugins
		{
			get {
				List<PluginHolder> unresolvedPlugins = new List<PluginHolder>();
				foreach (DependancyResolverItem item in _waitingForResolution) {
					unresolvedPlugins.Add(item.PluginHolder);
				}

				foreach (DependancyResolverItem item in _registeredPlugins) {
					unresolvedPlugins.Add(item.PluginHolder);
				}

				return unresolvedPlugins;
			}
		}

		internal PluginDependancyResolver()
		{
			_rootPluginItem			= null;
			_registeredPlugins		= new List<DependancyResolverItem>();
			_resoledPlugins			= new List<DependancyResolverItem>();
			_waitingForResolution	= new List<DependancyResolverItem>();
		}

		internal void RegisterPlugin(PluginHolder newPlugin)
		{
			if (ContainsPluginHolder(newPlugin)) {
				Debug.Assert(false, "Plugin is already registered");
				_log.ErrorFormat("Plugin '{0}' already registered. Id: '{1}', Description: '{2}'.", newPlugin.Name, newPlugin.Id.ToString(), newPlugin.Description);
				return;
			}

			_registeredPlugins.Add(new DependancyResolverItem(newPlugin));

			if (_logDebug) {
				_log.Debug(String.Format("Plugin '{0}' registered. Id: '{1}', Description: '{2}'.", newPlugin.Name, newPlugin.Id.ToString(), newPlugin.Description));
			}
		}

		internal bool ResolveDependancy()
		{
			_rootPluginItem = GetRootPlugin();
			_resoledPlugins.Add(_rootPluginItem);

			while (_resoledPlugins.Count>0) {
				DependancyResolverItem item = RemoveFirstItem(_resoledPlugins);

				ResolveWaitingDependancy(item.PluginHolder);

				IList<DependancyResolverItem> dependantsPlugins = GetDependantsPlugins(item.PluginHolder);
				ResolveDependancy(dependantsPlugins, item.PluginHolder);
			}

			return (_waitingForResolution.Count==0 && _registeredPlugins.Count==0);
		}

		private bool ContainsPluginHolder(PluginHolder newPlugin)
		{
			foreach (DependancyResolverItem item in _registeredPlugins) {
				if (item.PluginHolder.Id == newPlugin.Id) {
					return true;
				}
			}
			return false;
		}

		private DependancyResolverItem RemoveFirstItem(IList<DependancyResolverItem> list)
		{
			DependancyResolverItem firstItem = list[0];
			list.RemoveAt(0);
			return firstItem;
		}

		private DependancyResolverItem GetRootPlugin()
		{
			DependancyResolverItem rootPluginItem = null;

			foreach (DependancyResolverItem item in _registeredPlugins) {
				if (item.IsRoot) {
					if (rootPluginItem!=null) {
						_log.Fatal("More than one root plugin detected!");
						throw new ArgumentException("More than one root plugin detected!");
					}
					rootPluginItem = item;
				}
			}

			if (rootPluginItem==null) {
				throw new ArgumentException("Root plugin wasn't found");
			}

			_registeredPlugins.Remove(rootPluginItem);

			return rootPluginItem;
		}

		private IList<DependancyResolverItem> GetDependantsPlugins(PluginHolder pluginHolder)
		{
			IList<DependancyResolverItem> dependantsPlugins = new List<DependancyResolverItem>();
			foreach (DependancyResolverItem item in _registeredPlugins) {
				if (item.DependsOn(pluginHolder.Id)) {
					dependantsPlugins.Add(item);
				}
			}

			foreach (DependancyResolverItem item in dependantsPlugins) {
				_registeredPlugins.Remove(item);
			}

			return dependantsPlugins;
		}

		private void ResolveDependancy(IList<DependancyResolverItem> plugins, PluginHolder pluginHolder)
		{
			foreach (DependancyResolverItem item in plugins) {

				item.ResolveRependancy(pluginHolder);

				if (item.AllDependancyResolved) {
					_resoledPlugins.Add(item);
				} else {
					_waitingForResolution.Add(item);
				}
			}
		}

		private void ResolveWaitingDependancy(PluginHolder pluginHolder)
		{
			IList<DependancyResolverItem> resolvedPlugins = new List<DependancyResolverItem>();

			foreach (DependancyResolverItem item in _waitingForResolution) {

				item.ResolveRependancy(pluginHolder);

				if (item.AllDependancyResolved) {
					resolvedPlugins.Add(item);
				}
			}

			foreach (DependancyResolverItem item in resolvedPlugins) {
				_waitingForResolution.Remove(item);
				_resoledPlugins.Add(item);
			}
		}

	}
}
