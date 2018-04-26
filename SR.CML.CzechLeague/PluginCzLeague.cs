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
using SR.CML.Core.Plugins.Exceptions;

using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using log4net;

using System.Diagnostics;

namespace SR.CML.CzechLeague
{
	[Plugin("4D756233-69DF-4a65-8D85-DAA23B168318",
		"CzLeague", "Two races with swaped grid in second race",
		new String[7]{	"863E4F77-94E9-43fb-8A46-323220837A0D",		// MessagingGuid
						"F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3",		// CarDriverManagerGuid
						"46B960EA-864B-467e-9AC6-43E884185692",		// ControlFactoryGuid
						"536550BD-BE60-4fd2-91B7-098FCF43F5B1",		// CMLCoreGuid
						"AFACAC8A-B63D-4006-940B-4C31999ECE37",		// RaceDirectorGuid
						"6301C0A4-8FFE-4baf-9569-07753A268AAC",		// ResultManagerGuid
						"27253220-5A44-4d8f-A1D6-D52A0FF060FB"})]	// ServerSetting
	public class PluginCzLeague : IPlugin
	{
		private static ILog _log		= LogManager.GetLogger(typeof(PluginCzLeague));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool				_disposed	= false;
		private PluginState			_state		= PluginState.Undefined;
		private IMessaging			_messaging	= null;
		private LeagueController	_director	= null;

		private IPluginManager _pluginManager = null;
		internal IPluginManager PluginManager
		{
			get { return _pluginManager; }
		}

		public PluginCzLeague()
		{
		}

		~PluginCzLeague()
		{
			Debug.Assert(_disposed, "PluginCzLeague, call dispose before the application is closed!");
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
					DisposeDirector();
				}
				_disposed = true;
			}
		}

		private void DisposeDirector() {
			if (_director != null) {
				_director.Dispose();
				_director = null;
			}
		}

		#region IPlugin

		public PluginState State
		{
			get { return _state; }
		}

		public void Initialize(IPluginManager pluginManager)
		{
			if (pluginManager==null) {
				throw new PluginInitializeException("Plugin manager is null.");
			}

			_pluginManager = pluginManager;
			_messaging = _pluginManager.GetPlugin(CmlPlugins.MessagingGuid) as IMessaging;
			if (_messaging==null) {
				throw new PluginInitializeException("Plugin IMessaging not found.");
			}

			_state = PluginState.Initialized;
		}

		public void Activate()
		{
			if (_state == PluginState.Activated) {
				return;
			}

			_director = new LeagueController(_pluginManager);
			_director.Activate();
			_state = PluginState.Activated;

			_messaging.SendMessageToAll("^1Cz League^8 activated");
		}

		public void Deactivate()
		{
			if (_state != PluginState.Activated) {
				return;
			}

			_director.Deactivate();
			DisposeDirector();

			_messaging.SendMessageToAll("^1Cz League^8 deactivated");
			_state = PluginState.Deactivated;
		}

		#endregion
	}
}
