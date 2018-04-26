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

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	[Plugin("6455756F-C4EB-45b1-BBA4-2C491DD97351",
			"Rallycross", "Real rally cross race event",
			new String[7]{"863E4F77-94E9-43fb-8A46-323220837A0D", "F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3", "46B960EA-864B-467e-9AC6-43E884185692",
						  "536550BD-BE60-4fd2-91B7-098FCF43F5B1", "AFACAC8A-B63D-4006-940B-4C31999ECE37", "6301C0A4-8FFE-4baf-9569-07753A268AAC",
						  "27253220-5A44-4d8f-A1D6-D52A0FF060FB"})]
	public class RallycrossPlugin : IPlugin
	{
		private static ILog _log		= LogManager.GetLogger(typeof(RallycrossPlugin));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool			_disposed			= false;
		private PluginState		_state				= PluginState.Undefined;
		private IMessaging		_messaging			= null;
		private Rallycross		_rallycross;

		private IPluginManager _pluginManager = null;
		internal IPluginManager PluginManager
		{
			get { return _pluginManager; }
		}

		public RallycrossPlugin()
		{
			_rallycross = null;
		}

		~RallycrossPlugin()
		{
			Debug.Assert(_disposed, "RallycrossPlugin, call dispose before the application is closed!");
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
					DisposeRallycros();
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
			if (pluginManager==null) {
				throw new PluginInitializeException("Plugin manager is null.");
			}

			_pluginManager = pluginManager;
			_messaging = _pluginManager.GetPlugin(CmlPlugins.MessagingGuid) as IMessaging;
			if (_messaging==null) {
				throw new PluginInitializeException("Plugin IMessaging not found.");
			}

			_rallycross = null;
			_state = PluginState.Initialized;
		}

		public void Activate()
		{
			if (_state == PluginState.Activated) {
				return;
			}

			Debug.Assert(_rallycross==null);
			_rallycross = new Rallycross(_pluginManager);
			_state = PluginState.Activated;

			_messaging.SendMessageToAll("^1Rallycros^8 activated");
		}

		public void Deactivate()
		{
			if (_state != PluginState.Activated) {
				return;
			}

			DisposeRallycros();
			_messaging.SendMessageToAll("^1Rallycros^8 deactivated");
			_state = PluginState.Deactivated;
		}

		#endregion

		private void DisposeRallycros()
		{
			if (_rallycross != null) {
				_rallycross.Dispose();
				_rallycross = null;
			}
		}
	}
}
