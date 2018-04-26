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
using System.Threading;
using System.Collections.Generic;

using SR.CML.Common;

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
	[Plugin("536550BD-BE60-4fd2-91B7-098FCF43F5B1",
			"CML_Core", "Central Manager of Leagues - Core",
			new String[0]{})]
	internal class PluginCMLCore : PluginBase, ICMLCore
	{
		private bool			_connected		= false;
		private InSimHandler	_inSimHandler	= null;

		private Host _host = null;
		internal Host Host {
			set { _host = value; }
		}

		public PluginCMLCore()
		{
			_log			= LogManager.GetLogger(typeof(PluginCMLCore));
			_logDebug		= _log.IsDebugEnabled;
		}

		~PluginCMLCore()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "PluginCMLCore, call dispose before the application is closed!");
			Dispose(false);
		}

		protected override void InitializeInternal()
		{
			if (_inSimHandler==null) {
				_inSimHandler = new InSimHandler();
			}
			_inSimHandler.StateChanged += new FullMotion.LiveForSpeed.InSim.EventHandlers.StateChangeHandler(_inSimHandler_StateChanged);
		}

		protected override void ActivateInternal()
		{
			SetupInSim();
			Connect();
		}

		protected override void DeactivateInternal()
		{
			Disconnect();
		}

		protected override void DisposeInternal()
		{
			if (_inSimHandler!=null) {
				if (_inSimHandler.State==InSimHandler.HandlerState.Connected) {
					_inSimHandler.Close();
				}
				_inSimHandler.StateChanged -= new FullMotion.LiveForSpeed.InSim.EventHandlers.StateChangeHandler(_inSimHandler_StateChanged);

				_inSimHandler.Dispose();
				_inSimHandler = null;
			}
		}

		#region ICMLCore

		public event EventHandler<EventArgs> ConnectionStateChanged;

		public InSimHandler InSimHandler
		{
			get { return _inSimHandler; }
		}

		public bool IsConnected
		{
			get { return _connected; }
		}

		public void SendCommand(String command)
		{
			if (!_connected) {
				return;
			}
			_inSimHandler.SendMessage(command);
		}

		#endregion

		private void SetupInSim()
		{
			if (_host == null) {
				throw new ArgumentNullException("Host configuration is null");
			}

			_inSimHandler.Configuration.UseTCP				= true;
			_inSimHandler.Configuration.TrackingInterval	= 250;
			_inSimHandler.Configuration.NodeLapTracking		= true;
			_inSimHandler.Configuration.MultiCarTracking	= true;;
			_inSimHandler.Configuration.Local				= false;
			_inSimHandler.Configuration.LFSHostPort			= _host.InSimPort;
			_inSimHandler.Configuration.LFSHost				= _host.IP;
			_inSimHandler.Configuration.AdminPass			= _host.AdminisratorPassword;
			_inSimHandler.Configuration.AutoReconnect		= false;
			_inSimHandler.Configuration.ProgramName			= "CML";
		}

		private bool Connect()
		{
			if (_connected) {
				return _connected;
			}
			_inSimHandler.Initialize();
			_connected = (_inSimHandler.State == InSimHandler.HandlerState.Connected);
			return _connected;
		}

		private bool Disconnect()
		{
			if (!_connected) {
				return !_connected;
			}

			Thread.Sleep(250);

			switch (_inSimHandler.State) {
				case InSimHandler.HandlerState.Connected: {
					_inSimHandler.Close();
					_connected = !(_inSimHandler.State == InSimHandler.HandlerState.Closed);
					return true;
				} //break;

				case InSimHandler.HandlerState.New :
				case InSimHandler.HandlerState.Closed : {
					Debug.Assert(_connected == !(_inSimHandler.State == InSimHandler.HandlerState.Closed));
				} break;

				default : {
					Debug.Assert(false, "Unknown connection state");
					_log.Fatal("Unknown connection state");
				} break;
			}
			return !_connected;
		}

		private void _inSimHandler_StateChanged(InSimHandler sender, StateChangeEventArgs e)
		{
			_log.InfoFormat("Server state changed from '{0}' to '{1}'", e.PreviousState.ToString(), e.CurrentState.ToString());
			switch(e.CurrentState) {
				case InSimHandler.HandlerState.Closed : {
					_connected = false;
					Console.WriteLine("Disconnected from server");
				} break;

				case InSimHandler.HandlerState.Connected : {
					Console.WriteLine("Connected to server");
					_connected = true;
				} break;

				case InSimHandler.HandlerState.New : {
					_connected = false;
				} break;

				case InSimHandler.HandlerState.Initiaized: {
				} break;

				default : {
					Debug.Assert(false);
					_connected = false;
				} break;
			}

			if (e.CurrentState!=e.PreviousState) {
				if (ConnectionStateChanged!=null) {
					ConnectionStateChanged(this, new EventArgs());
				}
			}
		}

	}
}
