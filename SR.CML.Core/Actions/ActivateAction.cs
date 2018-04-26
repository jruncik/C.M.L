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

using System.Diagnostics;

namespace SR.CML.Core.Actions
{
	internal class ActivateAction : ITraverseAction
	{
		private bool _activateAll;

		internal ActivateAction(bool activateAll)
		{
			_activateAll = activateAll;
		}

		internal ActivateAction() : this(false)
		{
		}

		public void Run(PluginHolder item)
		{
			if (_activateAll) {
				Debug.Assert(item.Plugin.State == PluginState.Initialized || item.Plugin.State == PluginState.Deactivated);
				item.Plugin.Activate();
			} else {
				if (item.Selected) {
					Debug.Assert(item.Plugin.State == PluginState.Initialized || item.Plugin.State == PluginState.Deactivated);
					item.Plugin.Activate();
				}
			}
		}
	}
}
