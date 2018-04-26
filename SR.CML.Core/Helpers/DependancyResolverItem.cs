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
	internal class DependancyResolverItem
	{
		private IList<Guid> _dependsOn;

		private PluginHolder _pluginHolder;
		internal PluginHolder PluginHolder
		{
			get { return _pluginHolder; }
		}

		internal DependancyResolverItem(PluginHolder pluginHolder)
		{
			Debug.Assert(pluginHolder!=null);
			_pluginHolder = pluginHolder;

			_dependsOn = new List<Guid>(_pluginHolder.Dependency.Count);
			foreach (Guid id in _pluginHolder.Dependency) {
				_dependsOn.Add(id);
			}
		}

		internal bool IsRoot
		{
			get { return _pluginHolder.Dependency.Count == 0; }
		}

		internal bool AllDependancyResolved
		{
			get { return _dependsOn.Count == 0; }
		}

		internal bool DependsOn(Guid id) {
			foreach (Guid pluginID in _dependsOn) {
				if (id == pluginID) {
					return true;
				}
			}

			return false;
		}

		internal void ResolveRependancy(PluginHolder plugin)
		{
			Guid id = plugin.Id;
			foreach (Guid pluginID in _dependsOn) {
				if (id == pluginID) {
					_dependsOn.Remove(pluginID);
					plugin.AddDependantPlugin(_pluginHolder);
					_pluginHolder.AddDependsOn(plugin);

					break;
				}
			}

			if (AllDependancyResolved) {
				_pluginHolder.LatestDependsOn = plugin;
			}

			Debug.Assert(!DependsOn(id));
		}

	}
}
