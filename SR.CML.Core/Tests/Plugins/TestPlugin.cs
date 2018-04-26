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

namespace SR.CML.Core.Tests.Plugins
{
	public class TestPluginBase: IPlugin
	{
		protected bool			_disposed	= false;
		protected PluginState	_state		= PluginState.Undefined;

		~TestPluginBase()
		{
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
				}
				_disposed = true;
			}
		}

		#region IPlugin

		public PluginState State
		{
			get { return _state; }
		}

		public void Initialize(IPluginManager pluginManager)
		{
			_state = PluginState.Initialized;
		}

		public void Activate()
		{
			if (_state != PluginState.Activated) {
				_state = PluginState.Activated;
			}
		}

		public void Deactivate()
		{
			if (_state != PluginState.Deactivated) {
				_state = PluginState.Deactivated;
			}
		}

		#endregion
	}
}
