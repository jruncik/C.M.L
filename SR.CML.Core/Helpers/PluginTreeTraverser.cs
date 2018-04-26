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

using SR.CML.Core.Actions;
using SR.CML.Core.Plugins;

using System.Diagnostics;

namespace SR.CML.Core
{
	internal class PluginTreeTraverser
	{
		PluginHolder _rootPlugin;

		internal PluginTreeTraverser(PluginHolder rootPlugin)
		{
			Debug.Assert(rootPlugin!=null);
			_rootPlugin = rootPlugin;
		}

		internal void TraverseFromTop(ITraverseAction action)
		{
			if (action==null) {
				throw new ArgumentNullException("Action can't be null");
			}

			Queue<PluginHolder>	plugins	= new Queue<PluginHolder>();
			PluginHolder		plugin	= null;

			if (_rootPlugin!=null) {
				plugins.Enqueue(_rootPlugin);
			}

			while (plugins.Count>0) {
				plugin = plugins.Dequeue();
				action.Run(plugin);

				foreach(PluginHolder item in plugin.Dependants) {
					if (item.LatestDependsOn == plugin) {	// There can by cyclic dependency. Each plugin holder know his latest plugin.
						plugins.Enqueue(item);
					}
				}
			}
		}

		internal void TraverseFromBottom(ITraverseAction action)
		{
			if (action==null) {
				throw new ArgumentNullException("Action can't be null");
			}

			if (_rootPlugin!=null) {
				TraverseFromBottomInternal(_rootPlugin, action);
			}
		}

		private void TraverseFromBottomInternal(PluginHolder plugin, ITraverseAction action)
		{
			foreach(PluginHolder item in plugin.Dependants) {
				if (item.LatestDependsOn == plugin) {
					TraverseFromBottomInternal(item, action);	// There can by cyclic dependency. Each plugin holder know his latest plugin.
				}
			}
			action.Run(plugin);
		}

	}
}
