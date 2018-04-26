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

using log4net;

using System.Diagnostics;

namespace SR.CML.Core
{
	public class Core : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(Core));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool _disposed = false;

		private PluginManager _pluginsManager = null;
		public IPluginManager PluginManager
		{
			get { return _pluginsManager; }
		}

		public Core(String pluginsPath)
		{
			_pluginsManager = new PluginManager(pluginsPath);

			_pluginsManager.LoadPlugins();
			_pluginsManager.InstanciateAllPlugins();

			if (_logDebug) {
				_log.Debug("Initialized");
			}
		}

		~Core()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "Core, call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed) {

				if (disposing) {
					if (_pluginsManager!=null) {
						_pluginsManager.Dispose();
						_pluginsManager = null;
					}
				}
				_disposed = true;
			}
		}

	}
}
