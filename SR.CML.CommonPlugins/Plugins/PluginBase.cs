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
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using SR.CML.CommonPlugins.Controls;
using SR.CML.CommonPlugins.CarDriverManager;
using SR.CML.CommonPlugins.Results;

using FullMotion.LiveForSpeed.InSim;
using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins
{
	internal abstract class PluginBase : IPlugin
	{
		protected ILog				_log			= null;
		protected bool				_logDebug		= false;
		protected bool				_disposed		= false;
		protected IPluginManager	_pluginManager	= null;
		protected PluginState		_state			= PluginState.Undefined;

		~PluginBase()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
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
					DisposeInternal();
					if (_logDebug) {
						_log.Debug("Disposed");
					}
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
			Debug.Assert(pluginManager != null);
			if (pluginManager == null) {
				_log.Fatal("PluginMamner isn't set!");
				throw new ArgumentNullException("PluginManager isn't set!");
			}

			_pluginManager = pluginManager;

			InitializeInternal();
			_state = PluginState.Initialized;

			if (_logDebug) {
				_log.Debug("Initialized");
			}
		}

		public void Activate()
		{
			if (_state == PluginState.Activated) {
				Debug.Assert(false, String.Format("Plugin '{0}' is already activated.", this.GetType().Name));
				_log.Error("Plugin is already activated.");
				return;
			}

			ActivateInternal();
			_state = PluginState.Activated;
			if (_logDebug) {
				_log.Debug("Activated");
			}
		}

		public void Deactivate()
		{
			if (_state != PluginState.Activated) {
				return;
			}

			DeactivateInternal();
			_state = PluginState.Deactivated;
			if (_logDebug) {
				_log.Debug("Deactivated");
			}
		}

		#endregion

		protected abstract void InitializeInternal();
		protected abstract void ActivateInternal();
		protected abstract void DeactivateInternal();
		protected abstract void DisposeInternal();
	}
}
