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

using SR.CML.Common;

using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using SR.CML.CommonPlugins.Controls;
using SR.CML.CommonPlugins.CarDriverManager;
using SR.CML.CommonPlugins.Results;

using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Events;

using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.Plugins
{
	[Plugin("27253220-5A44-4d8f-A1D6-D52A0FF060FB",
		"ServerSetting", "Server Setting",
		new String[2] {	"536550BD-BE60-4fd2-91B7-098FCF43F5B1",		// CMLCoreGuid
						"F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3"})]	// CarDriverManagerGuid

	internal class PluginServerSetting : PluginBase, IServerSetting
	{
		private PluginCMLCore			_cmlCore			= null;
		private InSimHandler			_inSimHandler		= null;

		private SR.CML.Common.CmlConfiguration _configuration = null;

		public PluginServerSetting()
		{
			_log		= LogManager.GetLogger(typeof(PluginServerSetting));
			_logDebug	= _log.IsDebugEnabled;
		}

		protected override void InitializeInternal()
		{
			GetRequestedPlugins(_pluginManager);

			_cmlCore.Host = _configuration.Host;
		}

		protected override void ActivateInternal() {
			BindEvents();
		}

		protected override void DeactivateInternal() {
			UnbindEvents();
		}

		protected override void DisposeInternal() {
		}

		#region IServerSetting Members

		public CmlConfiguration Config {
			get { return _configuration; }
			set {
				if (value == null) {
					throw new ArgumentNullException("Configuration is null");
				}
				_configuration = value;
			}
		}
		#endregion

		private void GetRequestedPlugins(IPluginManager pluginManager) {
			_cmlCore = pluginManager.GetPlugin(CmlPlugins.CMLCoreGuid) as PluginCMLCore;
			Debug.Assert(_cmlCore != null);
			if (_cmlCore == null) {
				_log.Fatal("CML Core plugin wasn't found");
				throw new ArgumentNullException("CML Core plugin wasn't found!");
			}

			_inSimHandler = _cmlCore.InSimHandler;
			Debug.Assert(_inSimHandler != null);
			if (_inSimHandler == null) {
				_log.Fatal("InSimHandler isn't set!");
				throw new ArgumentNullException("InSimHandler isn't set!");
			}
		}

		private void BindEvents() {
			_pluginManager.PluginActivated	+= new EventHandler<EventArgs>(_pluginManager_PluginActivated);
			_inSimHandler.LFSState			+= new FullMotion.LiveForSpeed.InSim.EventHandlers.StateEventHandler(_inSimHandler_LFSState);
		}

		private void UnbindEvents() {
			_pluginManager.PluginActivated	-= new EventHandler<EventArgs>(_pluginManager_PluginActivated);
			_inSimHandler.LFSState			-= new FullMotion.LiveForSpeed.InSim.EventHandlers.StateEventHandler(_inSimHandler_LFSState);
		}

		void _pluginManager_PluginActivated(object sender, EventArgs e) {
			SetEventConfiguration(_configuration.EventConfiguration);
		}

		void _inSimHandler_LFSState(InSimHandler sender, State e) {
			if (e.RaceState == FullMotion.LiveForSpeed.InSim.Enums.RaceState.NoRace) {

				State lastLfsSate				= _inSimHandler.LastLFSState;

				EventConfiguration eventConfig	= _configuration.EventConfiguration;
				if (eventSettingIsDifferent(lastLfsSate, eventConfig)) {
					SetEventConfigurationToServer(eventConfig);
				}
			}
		}

		private void SetEventConfiguration(EventConfiguration eventConfig) {
			State lastLfsSate = _inSimHandler.LastLFSState;
			if (eventSettingIsDifferent(lastLfsSate, eventConfig)) {
				if (hasToBeReset(lastLfsSate)) {
					ResetGameToInitialScrean();
					return;
				}
				SetEventConfigurationToServer(eventConfig);
			}
		}

		private void ResetGameToInitialScrean() {
			_cmlCore.SendCommand(LfsCommands.End());
		}

		private void SetEventConfigurationToServer(EventConfiguration eventConfig) {
			_cmlCore.SendCommand(LfsCommands.SetHostName("CML"));

			_cmlCore.SendCommand(LfsCommands.SetPassword(eventConfig.ServerPassword));
			_cmlCore.SendCommand(LfsCommands.SetTrack(eventConfig.Track));
			_cmlCore.SendCommand(LfsCommands.SetWeather(eventConfig.Wheather));
			_cmlCore.SendCommand(LfsCommands.SetWind(eventConfig.Wind));
			_cmlCore.SendCommand(LfsCommands.SetMaxCars(eventConfig.MaxCars));

			_cmlCore.SendCommand(LfsCommands.SetMaxGuests(42));
			_cmlCore.SendCommand(LfsCommands.SetMaxCars(32));
			_cmlCore.SendCommand(LfsCommands.SetCarsHost(3));

			_cmlCore.SendCommand(LfsCommands.SetStartGridOrder(GridOrder.Fixed));

			_cmlCore.SendCommand(LfsCommands.SetAllowWrongWay(false));
			_cmlCore.SendCommand(LfsCommands.SetForceCockpitView(false));
			_cmlCore.SendCommand(LfsCommands.SetMidRaceConnect(true));
			_cmlCore.SendCommand(LfsCommands.SetMustPit(false));
			_cmlCore.SendCommand(LfsCommands.SetNetworkDebug(true));
			_cmlCore.SendCommand(LfsCommands.SetSelectTrack(true));
			_cmlCore.SendCommand(LfsCommands.SetVote(false));
		}

		private bool eventSettingIsDifferent(State lastLfsSate, EventConfiguration eventConfig) {
			if (lastLfsSate.Weather != (FullMotion.LiveForSpeed.InSim.Enums.Weather)eventConfig.Wheather) {
				return true;
			}

			if (lastLfsSate.Wind != (FullMotion.LiveForSpeed.InSim.Enums.Wind)eventConfig.Wind) {
				return true;
			}

			if (lastLfsSate.ShortTrackName.ToUpper().CompareTo(eventConfig.Track.ToString().ToUpper()) !=0 ) {
				return true;
			}

			return false;
		}

		private bool hasToBeReset(State lastLfsSate) {
			if (lastLfsSate.RaceState != FullMotion.LiveForSpeed.InSim.Enums.RaceState.NoRace) {
				return true;
			}
			return false;
		}
	}
}
