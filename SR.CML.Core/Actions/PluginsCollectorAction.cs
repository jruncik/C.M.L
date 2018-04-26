﻿/* ------------------------------------------------------------------------- *
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

namespace SR.CML.Core.Actions
{
	internal class PluginsCollectorAction : ITraverseAction
	{
		private bool _eachPluginCalledOnyOnce;
		internal bool EachPluginCalledOnyOnce
		{
			get { return _eachPluginCalledOnyOnce; }
		}

		private IList<PluginHolder> _plugins;
		internal IList<PluginHolder> Plugins
		{
			get { return _plugins; }
		}

		internal PluginsCollectorAction()
		{
			_eachPluginCalledOnyOnce	= true;
			_plugins					= new List<PluginHolder>();
		}

		public void Run(PluginHolder item)
		{
			if (_plugins.Contains(item)) {
				_eachPluginCalledOnyOnce = false;
			}
			_plugins.Add(item);
		}
	}
}
