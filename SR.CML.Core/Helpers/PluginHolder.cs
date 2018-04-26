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
using System.Reflection;
using System.Collections.Generic;

using SR.CML.Core.Plugins;

using System.Diagnostics;

namespace SR.CML.Core
{
	internal class PluginHolder : IDisposable
	{
		private bool			_disposed		= false;
		private PluginAttribute	_pluginAttribute;
		private String			_pluginFullName;
		private Assembly		_assembly;

		internal String Name
		{
			get { return _pluginAttribute.Name; }
		}

		internal String Description
		{
			get { return _pluginAttribute.Description; }
		}

		internal Guid Id
		{
			get { return _pluginAttribute.Id; }
		}

		internal IList<Guid> Dependency
		{
			get { return _pluginAttribute.Dependency; }
		}

		private IList<PluginHolder> _dependants;
		internal IList<PluginHolder> Dependants
		{
			get { return _dependants; }
		}

		private IList<PluginHolder> _dependsOn;
		internal IList<PluginHolder> DependsOn
		{
			get { return _dependsOn; }
		}

		private PluginHolder _latestDependsOn;
		internal PluginHolder LatestDependsOn
		{
			get { return _latestDependsOn; }
			set { _latestDependsOn = value; }
		}

		private bool _selected;
		internal bool Selected
		{
			get { return _selected; }
			set { _selected = value; }
		}

		private IPlugin _plugin;
		internal IPlugin Plugin
		{
			get { return _plugin; }
		}

		internal PluginHolder(Assembly assembly, String pluginFullName, PluginAttribute pluginAttribute)
		{
			Debug.Assert(assembly!=null);
			Debug.Assert(pluginAttribute!=null);
			Debug.Assert(!String.IsNullOrEmpty(pluginFullName));

			_assembly				= assembly;
			_pluginFullName			= pluginFullName;
			_pluginAttribute		= pluginAttribute;

			_selected	= false;
			_plugin					= null;
			_dependants				= new List<PluginHolder>(_pluginAttribute.Dependency.Count);
			_dependsOn				= new List<PluginHolder>();
		}

		~PluginHolder()
		{
			Debug.Assert(_disposed, "Call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!_disposed) {
				if (disposing) {
					if (_plugin!=null) {
						_plugin.Dispose();
						_plugin = null;
					}
				}
				_disposed = true;
			}
		}


		internal void InstanciatePlugin()
		{
			_plugin = _assembly.CreateInstance(_pluginFullName) as IPlugin;
			if (_plugin==null) {
				throw new CreateInstanceException(String.Format("Plugin '{0}' isn't created", _pluginFullName));
			}
		}

		internal void AddDependantPlugin(PluginHolder dependantPlugin)
		{
			if (ContainsPlugin(dependantPlugin, _dependants)) {
				Debug.Assert(false, String.Format("Plugin ID: '{0}' is already added", dependantPlugin.Id.ToString()));
				return;
			}

			_dependants.Add(dependantPlugin);
		}

		internal void AddDependsOn(PluginHolder dependsOn)
		{
			if (ContainsPlugin(dependsOn, _dependsOn)) {
				Debug.Assert(false, String.Format("Plugin ID: '{0}' is already added", dependsOn.Id.ToString()));
				return;
			}

			_dependsOn.Add(dependsOn);
		}

		private bool ContainsPlugin(PluginHolder item, IList<PluginHolder>list)
		{
			foreach(PluginHolder pluginHolder in list) {
				if (pluginHolder.Id == item.Id) {
					return true;
				}
			}
			return false;
		}
	}
}
