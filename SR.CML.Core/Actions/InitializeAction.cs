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

using SR.CML.Core.Plugins;

namespace SR.CML.Core.Actions
{
	internal class InitializeAction : ITraverseAction
	{
		private PluginManager	_pluginManager;
		private bool			_initializeAll;

		internal InitializeAction(PluginManager pluginManager, bool initializeAll)
		{
			if (pluginManager==null) {
				throw new ArgumentNullException("Plugin manager has to be set!");
			}

			_initializeAll	= initializeAll;
			_pluginManager	= pluginManager;
		}

		internal InitializeAction(PluginManager pluginManager) : this (pluginManager, false)
		{
		}

		public void Run(PluginHolder item)
		{
			if (item.Plugin.State!=PluginState.Undefined) {
				return;
			}

			if (_initializeAll) {
				item.Plugin.Initialize(_pluginManager);
			} else {
				if (item.Selected) {
					item.Plugin.Initialize(_pluginManager);
				}
			}
		}
	}
}
