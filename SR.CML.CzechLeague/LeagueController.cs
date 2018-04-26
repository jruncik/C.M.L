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
using SR.CML.Core.Plugins.Exceptions;

using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using log4net;

using System.Diagnostics;

namespace SR.CML.CzechLeague
{
	internal class LeagueController : IDisposable
	{
		private static ILog _log = LogManager.GetLogger(typeof(LeagueController));
		private static bool _logDebug = _log.IsDebugEnabled;

		private bool			_disposed		= false;
		private IControlFactory	_controlFactory = null;
		private ICountDown		_countDown		= null;
		private IMessaging		_messaging		= null;

		private IServerSetting _serverSetting = null;
		public IServerSetting ServerSetting {
			get { return _serverSetting; }
		}

		private IPluginManager _pluginManager = null;
		internal IPluginManager PluginManager {
			get { return _pluginManager; }
		}

		private ICarDriverManager _carDriverManager = null;
		internal ICarDriverManager CarDriverManager {
			get { return _carDriverManager; }
		}

		private ICMLCore _cmlCore;
		public ICMLCore CmlCore {
			get { return _cmlCore; }
		}

		private IRaceDirector _raceDirector = null;
		public IRaceDirector RaceDirector {
			get { return _raceDirector; }
		}

		private IResultManager _resultManager = null;
		public IResultManager ResultManager {
			get { return _resultManager; }
		}

		private RaceEventDirectors _raceEventDirectors;
		internal RaceEventDirectors RaceEventDirectors {
			get { return _raceEventDirectors; }
		}

		IList<DriverInfo> _drivers = new List<DriverInfo>();
		internal IList<DriverInfo> Drivers {
			get { return _drivers; }
			set { _drivers = value; }
		}

		private GridBuilder _gridOrderBuilder;
		internal GridBuilder GridOrderBuilder {
			get {
				if (_gridOrderBuilder==null) {
					_gridOrderBuilder = new GridBuilder(this);
				}
				return _gridOrderBuilder;
			}
		}

		private SelectDriversForRace _selectDrivers;
		internal SelectDriversForRace SelectDrivers {
			get {
				if (_selectDrivers == null) {
					_selectDrivers = new SelectDriversForRace(this);
				}
				return _selectDrivers;
			}
		}

		private EventState _lastFinishedEventState;
		private EventState _eventState = EventState.Uninitialized;
		internal EventState EventState {
			get { return _eventState; }
			set { _eventState = value; }
		}

		internal LeagueController(IPluginManager pluginManager)
		{
			if (pluginManager==null) {
				_log.Fatal("Plugin Manager is null!");
				throw new ArgumentNullException("Plugin Manager is null!");
			}
			_pluginManager = pluginManager;

			GetRequestedPlugins();
		}

		private void GetRequestedPlugins() {

			_cmlCore = _pluginManager.GetPlugin(CmlPlugins.CMLCoreGuid) as ICMLCore;
			if (_cmlCore == null) {
				_log.Fatal("CML core wasn't found!");
				throw new PluginActivateException("CML core wasn't found");
			}

			_carDriverManager = _pluginManager.GetPlugin(CmlPlugins.CarDriverManagerGuid) as ICarDriverManager;
			if (_carDriverManager == null) {
				_log.Fatal("Car and Driver Manager wasn't found!");
				throw new PluginActivateException("Car and Driver Manager wasn't found");
			}

			_raceDirector = _pluginManager.GetPlugin(CmlPlugins.RaceDirectorGuid) as IRaceDirector;
			Debug.Assert(_raceDirector != null);
			if (_raceDirector == null) {
				_log.Fatal("RaceDirector wasn't found");
				throw new ArgumentException("RaceDirector wasn't found");
			}

			_resultManager = _pluginManager.GetPlugin(CmlPlugins.ResultManagerGuid) as IResultManager;
			Debug.Assert(_resultManager != null);
			if (_resultManager == null) {
				_log.Fatal("ResultManager wasn't found");
				throw new ArgumentException("ResultManager wasn't found");
			}

			_controlFactory = _pluginManager.GetPlugin(CmlPlugins.ControlFactoryGuid) as IControlFactory;
			Debug.Assert(_controlFactory != null);
			if (_controlFactory == null) {
				_log.Fatal("ControlFactory wasn't found");
				throw new ArgumentException("ControlFactory wasn't found");
			}

			_messaging = _pluginManager.GetPlugin(CmlPlugins.MessagingGuid) as IMessaging;
			Debug.Assert(_messaging != null);
			if (_messaging == null) {
				_log.Fatal("Messaging wasn't found");
				throw new ArgumentException("Messaging wasn't found");
			}

			_serverSetting = _pluginManager.GetPlugin(CmlPlugins.ServerSettingGuid) as IServerSetting;
			Debug.Assert(_serverSetting != null);
			if (_serverSetting == null) {
				_log.Fatal("IServerSetting wasn't found");
				throw new ArgumentException("IServerSetting wasn't found");
			}
		}

		~LeagueController()
		{
			Debug.Assert(_disposed, "LeagueDirector, call dispose before the application is closed!");
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
					UnbindEvents();
					DisposeSelectDrivers();
					DisposeGridOrderBuilder();
					DisposeDriversAndDirectors();
				}
				_disposed = true;
			}
		}

		private void CreateCountDown() {
			Debug.Assert(_countDown == null);
			_countDown = _controlFactory.CreateCountDown();

			_countDown.Left		= 90;
			_countDown.Top		= 45;
			_countDown.Width	= 20;
			_countDown.Height	= 20;
			_countDown.Ticks	= 30;
		}

		private void DisposeCountDown() {
			if (_countDown == null) {
				return;
			}
			_countDown.Delete();
			_countDown = null;
		}

		internal void Activate() {
			_raceEventDirectors = new RaceEventDirectors(this);
			CreateCountDown();
			BindEvents();
		}

		internal void Deactivate() {
			UnbindEvents();
			DisposeCountDown();
			DisposeSelectDrivers();
			DisposeGridOrderBuilder();
		}

		private void DisposeGridOrderBuilder() {
			if (_gridOrderBuilder != null) {
				_gridOrderBuilder.Hide();
				_gridOrderBuilder.Dispose();
				_gridOrderBuilder = null;
			}
		}

		private void DisposeSelectDrivers() {
			if (_selectDrivers != null) {
				_selectDrivers.Hide();
				_selectDrivers.Dispose();
				_selectDrivers = null;
			}
		}

		private void DisposeDriversAndDirectors() {
			if (_drivers != null) {
				foreach (DriverInfo driverInfo in _drivers) {
					driverInfo.Dispose();
				}
				_drivers.Clear();
				_drivers = null;

				if (_raceEventDirectors != null) {
					_raceEventDirectors.Dispose();
					_raceEventDirectors = null;
				}
			}
		}

		private void BindEvents()
		{
			if (_countDown != null) {
				_countDown.Elapsed += new EventHandler<EventArgs>(_countDown_Elapsed);
			}
			_carDriverManager.DriverStateChanged	+= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
			_carDriverManager.CarStateChanged		+= new EventHandler<CarStateChangedEventArgs>(_carDriverManager_CarStateChanged);
			_resultManager.AllCarsFinished			+= new EventHandler<EventArgs>(_resultManager_AllCarsFinished);
			_resultManager.CarFinished				+= new EventHandler<CarFinishedEventArgs>(_resultManager_CarFinished);
		}

		private void UnbindEvents()
		{
			if (_countDown != null) {
				_countDown.Elapsed -= new EventHandler<EventArgs>(_countDown_Elapsed);
			}
			_carDriverManager.DriverStateChanged	-= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
			_carDriverManager.CarStateChanged		-= new EventHandler<CarStateChangedEventArgs>(_carDriverManager_CarStateChanged);
			_resultManager.AllCarsFinished			-= new EventHandler<EventArgs>(_resultManager_AllCarsFinished);
			_resultManager.CarFinished				-= new EventHandler<CarFinishedEventArgs>(_resultManager_CarFinished);
		}

		private void GetAllRacingDrivers() {
			lock (this) {
				LogDebug("Getting all racing drivers");
				if (_drivers == null) {
					_drivers = new List<DriverInfo>(_carDriverManager.Drivers.Count);
				} else {
					foreach (DriverInfo driver in _drivers) {
						driver.Dispose();
					}
					_drivers.Clear();
				}

				DriverInfo driverInfo = null;
				foreach (IInSimDriver inSimDriver in _carDriverManager.Drivers) {

					driverInfo = null;
					if (_raceEventDirectors.IsRaceDirector(inSimDriver.LfsName)) {
						driverInfo = _raceEventDirectors.GetDirectorDriverInfo(inSimDriver.LfsName);
						Debug.Assert(driverInfo != null);
					} else {
						driverInfo = GetDriverInfo(inSimDriver.LfsName);
					}

					if (driverInfo == null) {
						driverInfo = new DriverInfo(inSimDriver);
					}

					if (driverInfo.Driver == null) {
						driverInfo.SetInSimDriver(inSimDriver);
					}

					_drivers.Add(driverInfo);
					LogDebugFormat("Driver '{0}' added.", driverInfo.LfsUserName);
					LogDrivers();
				}
			}
		}

		internal DriverInfo GetDriverInfo(String lfsDrivername) {
			if (_drivers == null || String.IsNullOrEmpty(lfsDrivername)) {
				return null;
			}

			foreach (DriverInfo driverInfo in _drivers) {
				if (driverInfo.LfsUserName == lfsDrivername) {
					return driverInfo;
				}
			}
			return null;
		}

		private void CarDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e) {
			lock (this) {
				DriverInfo driverInfo = GetDriverInfo(e.Driver.LfsName);

				if (e.OldState == DriverState.Undefined && e.NewState == DriverState.Connected) {
					if (driverInfo == null) {
						driverInfo = new DriverInfo(e.Driver);

						if (driverInfo.CanParticipate) {
							_drivers.Add(driverInfo);
							LogDebugFormat("Driver '{0}' added.", driverInfo.LfsUserName);
							LogDrivers();
						}
					}
					DriverConnected(e, driverInfo);
				}

				if (e.NewState == DriverState.Disconnecting) {
					DriverDisconnected(e, driverInfo);
				}

				if (e.NewState == DriverState.InCar) {
					if (driverInfo == null || !driverInfo.CanParticipate || !driverInfo.CanRace) {
						if (driverInfo != null) {
							_log.DebugFormat("Send player '{0}' to spect. CanParticipate='{1}', CanRace='{2}'", driverInfo.LfsUserName, driverInfo.CanParticipate, driverInfo.CanRace);
						}
						_cmlCore.SendCommand(LfsCommands.Spectate(e.Driver));
					}
				}
			}
		}

		void _carDriverManager_CarStateChanged(object sender, CarStateChangedEventArgs e) {
			if (EventState != EventState.Qualify) {
				return;
			}

			DriverInfo driverInfo = GetDriverInfo(e.Car.ActiveDriver.LfsName);
			if (driverInfo == null) {
				return;
			}

			if (e.OldState == CarState.OnTrack || e.OldState == CarState.InGarage) {
				switch (e.NewState) {
					case CarState.LeavingTrack: {
						driverInfo.CanRace = false;
					} break;
				}
			}

			if (!driverInfo.CanRace) {
				_cmlCore.SendCommand(LfsCommands.Spectate(e.Car.ActiveDriver));
			}
		}

		private void AllPossibleDriversCanDrive() {
			_log.Debug("AllPossibleDriversCanDrive");
			foreach (DriverInfo driverInfo in _drivers) {
				driverInfo.CanRace = true;
			}
		}

		private void DisableNonReadyDrivers() {
			_log.Debug("DisableNonReadyDrivers");
			foreach (DriverInfo driverInfo in _drivers) {
				if (driverInfo.Driver != null && driverInfo.Driver.State == DriverState.InCar) {
					_log.DebugFormat("Driver can drive '{0}' this race", driverInfo.LfsUserName);
					driverInfo.CanRace = true;
				} else {
					_log.DebugFormat("Driver can't drive '{0}' this race", driverInfo.LfsUserName);
					driverInfo.CanRace = false;
				}
			}
		}

		private void DriverConnected(DriverStateEventArgs e, DriverInfo driverInfo) {
			driverInfo.SetInSimDriver(e.Driver);
			if (_eventState == EventState.Qualify) {
				driverInfo.CanRace = true;
			}
		}

		private void DriverDisconnected(DriverStateEventArgs e, DriverInfo driverInfo) {
			driverInfo.RemoveInSimDriver();
		}

		private void LogDebug(String message) {
			if (_logDebug==false) {
				return;
			}

			_log.Debug(message);
		}

		private void LogDebugFormat(String message, params Object[] args) {
			if (_logDebug == false) {
				return;
			}

			_log.DebugFormat(message, args);
		}

		internal void ShowGridOrderBuilder() {
			AllPossibleDriversCanDrive();
			GridOrderBuilder.Show();
		}

		internal void HideGridOrderBuilder() {
			GridOrderBuilder.Hide();
		}

		public void ShowSelectDrivers() {
			SelectDrivers.Show();
		}

		internal void HideSelectDrivers() {
			SelectDrivers.Hide();
		}

		private void _resultManager_AllCarsFinished(object sender, EventArgs e) {
			if (_eventState == EventState.Initialized || _eventState==EventState.Uninitialized) {
				return;
			}

			_log.Debug("resultManager_AllCarsFinished");

			_raceEventDirectors.SetQualifyButtonText("^3Qualify");
			_raceEventDirectors.SetRaceButtonText("^3Race");

			EventState = EventState.Initialized;
		}

		void _resultManager_CarFinished(object sender, CarFinishedEventArgs e) {
			if (_eventState == EventState.Initialized || _eventState == EventState.Uninitialized) {
				return;
			}

			_log.Debug("_resultManager_CarFinished");

			_raceEventDirectors.SetQualifyButtonText("^3Qualify");
			_raceEventDirectors.SetRaceButtonText("Race");

			EventState = EventState.Initialized;
		}

		private void _countDown_Elapsed(object sender, EventArgs e) {
			_log.Debug("Count Down Elapsed");

			if (_gridOrderBuilder != null) {
				_gridOrderBuilder.Hide();
			}
			if (_selectDrivers != null) {
				_selectDrivers.Hide();
			}

			if (_eventState == EventState.Qualify) {
				_raceDirector.StartQualify(_serverSetting.Config.CzLeagueConfig.QualifyMin);
				_raceEventDirectors.SetQualifyButtonText("^3Restart Qualify");
				_lastFinishedEventState = EventState.Qualify;

			} else {
				DisableNonReadyDrivers();
				_raceDirector.StartRace(_serverSetting.Config.CzLeagueConfig.RaceLaps);
				_raceEventDirectors.SetRaceButtonText("^1Stop Race");
				_lastFinishedEventState = EventState.Race;
			}

			_log.DebugFormat("_countDown_Elapsed - lastFinishedEventState '{0}'", _lastFinishedEventState);
		}

		internal void Initialize() {
			GetAllRacingDrivers();
			_raceEventDirectors.SetQualifyButtonText("^3Qualify");
			_raceEventDirectors.SetRaceButtonText("^3Race");
			EventState = EventState.Initialized;
		}

		internal void StartRace() {
			_log.DebugFormat("StartRace, state {0}", _eventState);
			if (_eventState == EventState.Uninitialized) {
				Initialize();
				return;
			}

			AllPossibleDriversCanDrive();

			_log.DebugFormat("RD state: {0}", _raceDirector.State);

			if (EventState == EventState.Race) {
				_messaging.SendControlMessageToAll("^1Race stopped", 15);
				_raceEventDirectors.SetRaceButtonText("^3Race");
				_raceEventDirectors.SetQualifyButtonText("^3Qualify");
				EventState = EventState.Initialized;
				return;
			}

			if (!_countDown.Active) {
				_messaging.SendControlMessageToAll("^3Race...", 20);
				_countDown.Ticks = 60;
				_countDown.Start();
				_raceEventDirectors.SetQualifyButtonText("Qualify");
				_raceEventDirectors.SetRaceButtonText("^2Race");
			}
			EventState = EventState.Race;
		}

		internal void StartQualify() {
			_log.DebugFormat("StartQualify, state {0}", _eventState);
			if (_eventState == EventState.Uninitialized) {
				Initialize();
				return;
			}

			AllPossibleDriversCanDrive();

			EventState = EventState.Qualify;

			if (!_countDown.Active) {
				_messaging.SendControlMessageToAll("^3Qualify...", 20);
				_countDown.Ticks = 30;
				_countDown.Start();
				_raceEventDirectors.SetQualifyButtonText("^2Qualify");
				_raceEventDirectors.SetRaceButtonText("^3Race");
			}
		}

		internal void SwapDrivers() {
			_log.Debug("SwapDrivers");
			lock (this) {
				Int32 swappedDriversCount = _serverSetting.Config.CzLeagueConfig.SwapFirstDrivers;
				if (swappedDriversCount > _drivers.Count) {
					swappedDriversCount = _drivers.Count;
				}

				for (Int32 i = 0; i < swappedDriversCount / 2; ++i) {
					SwapDriver(i, swappedDriversCount - i - 1);
				}
				_gridOrderBuilder.Update();
			}
		}

		private void SwapDriver(Int32 firstIndex, Int32 secondIndex) {
			if (firstIndex == secondIndex) {
				return;
			}

			DriverInfo driverInfoTmp = _drivers[firstIndex];
			_drivers[firstIndex] = _drivers[secondIndex];
			_drivers[secondIndex] = driverInfoTmp;
		}

		internal void GetDriversOrderFromResult()
		{
			_log.Debug("GetDriversOrderFromResult");
			lock (this) {
				Dictionary<String, DriverInfo> driversTmp = new Dictionary<String, DriverInfo>();
				foreach (DriverInfo driver in _drivers) {
					driversTmp.Add(driver.LfsUserName, driver);
				}

				IList<IRaceResult> results = null;
				_log.DebugFormat("GetDriversOrderFromResult - lastFinishedEventState '{0}'", _lastFinishedEventState);
				if (_lastFinishedEventState == EventState.Qualify) {
					_log.Debug("Getting qualify results");
					results = _resultManager.GetSortedQualifyResults();
				} else {
					_log.Debug("Getting race results");
					results = _resultManager.GetSortedRaceResults();
				}

				_drivers.Clear();

				foreach (IRaceResult raceResult in results) {
					if (driversTmp.ContainsKey(raceResult.LfsPlayerName)) {
						_drivers.Add(driversTmp[raceResult.LfsPlayerName]);
						driversTmp.Remove(raceResult.LfsPlayerName);
					}
				}

				foreach (DriverInfo driver in driversTmp.Values) {
					_drivers.Add(driver);
				}
				driversTmp.Clear();
				_gridOrderBuilder.Update();
				LogDrivers();
			}
		}

		[Conditional("DEBUG")]
		private void LogDrivers() {
			String driverNames = String.Empty;

			foreach (DriverInfo driver in _drivers) {
				if (driverNames.Length != 0) {
					driverNames += ", ";
				}
				driverNames += driver.LfsUserName;
			}

			_log.DebugFormat("Drivers: '{0}'", driverNames);
		}
	}
}
