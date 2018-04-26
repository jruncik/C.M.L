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
using SR.CML.Core.Plugins.Exceptions;
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class RallycrossDirector : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(RallycrossDirector));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool										_disposed	= false;
		private Rallycross									_rallycross	= null;
		private Dictionary<String, RallycrossDirectorMenu>	_menu		= null;
		private ICountDown									_countDown	= null;

		private ICarDriverManager	_carDriverManager	= null;
		private IControlFactory		_controlFactory		= null;
		private IMessaging 			_messaging			= null;
		private IRaceDirector		_raceDirector		= null;

		private RallycrossSate _rallycrossState = RallycrossSate.none;
		internal RallycrossSate RallycrossSate
		{
			get { return _rallycrossState; }
		}

		internal IControlFactory ControlFactory
		{
			get { return _controlFactory; }
		}

		private StageSate _stageState = StageSate.none;
		internal StageSate StageState
		{
			get { return _stageState; }
		}

		internal RallycrossDirector(Rallycross rallycross)
		{
			Debug.Assert(rallycross!=null);
			if (rallycross==null) {
				_log.Fatal("Rallycross is null!");
				throw new ArgumentNullException("Rallycross is null!");
			}
			_rallycross = rallycross;

			GetPlugins();

			_menu = new Dictionary<String, RallycrossDirectorMenu>(8);
			RallycrossDirectorMenu.NewButtonText = "^3Initialize";

			InitializeCountDown();

			BindEvents();
		}

		~RallycrossDirector()
		{
			Debug.Assert(_disposed, "RallycrossDirector, call dispose before the application is closed!");
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

					if (_countDown!=null) {
						_countDown.Elapsed -= new EventHandler<EventArgs>(_countDown_Elapsed);
						_countDown.Delete();
						_countDown = null;
					}

					if (_menu!=null) {
						foreach(RallycrossDirectorMenu menu in _menu.Values) {
							menu.Dispose();
						}
						_menu = null;
					}
				}
				_disposed = true;
			}
		}

		private void InitializeCountDown()
		{
			_countDown			= _controlFactory.CreateCountDown();
			_countDown.Left		= 90;
			_countDown.Top		= 40;
			_countDown.Width	= 20;
			_countDown.Height	= 20;
			_countDown.Ticks	= 20;
		}

		internal void CarDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e)
		{
			switch (e.NewState) {
				case DriverState.Connected : {
					String directorName = e.Driver.LfsName;
					if (!_rallycross.IsRaceDirector(directorName)) {
						return;
					}

					if (!_menu.ContainsKey(directorName)) {
						DriverInfo driverInfo = _rallycross.GetDirectorDriverInfo(directorName);
						Debug.Assert(driverInfo!=null);
						if (driverInfo==null) {
							_log.Fatal("DriverInfo for driver is null, but driver is connected!");
							throw new ArgumentNullException("DriverInfo for driver is null, but driver is connected!");
						}
						_menu.Add(directorName, new RallycrossDirectorMenu(this, driverInfo));
					}
				} break;

				case DriverState.Disconnecting : {
					String directorName = e.Driver.LfsName;
					if (_menu.ContainsKey(directorName)) {
						_menu[directorName].Dispose();
						_menu.Remove(directorName);
					}

				} break;
			}

			foreach(RallycrossDirectorMenu menu in _menu.Values) {
				menu.CarDriverManager_DriverStateChanged(sender, e);
			}
		}

		internal void NextStage()
		{
			switch (_rallycrossState) {
				case RallycrossSate.none: {
					_rallycross.InitRacingEvent();
					_rallycrossState	= RallycrossSate.groups;
					_stageState			= StageSate.none;
					NextStage();
				} break;

				case RallycrossSate.groups: {
					StageStateGroup();
				} break;
			}
		}

		private void BindEvents()
		{
			if (_countDown!=null) {
				_countDown.Elapsed += new EventHandler<EventArgs>(_countDown_Elapsed);
			}

			_raceDirector.StateChanged				+= new EventHandler<RaceStateChangedEventArgs>(RaceDirector_StateChanged);
			_carDriverManager.AiDriverStateChanged	+= new EventHandler<AiDriverStateChangedEventArgs>(CarDriverManager_AiDriverStateChanged);
		}

		private void UnbindEvents()
		{
			if (_countDown!=null) {
				_countDown.Elapsed -= new EventHandler<EventArgs>(_countDown_Elapsed);
			}
			_carDriverManager.AiDriverStateChanged	-= new EventHandler<AiDriverStateChangedEventArgs>(CarDriverManager_AiDriverStateChanged);
			_raceDirector.StateChanged				-= new EventHandler<RaceStateChangedEventArgs>(RaceDirector_StateChanged);
		}

		private void StageStateGroup()
		{
			switch (_stageState) {
				case StageSate.none: {
					_rallycross.GroupManager.ShowGroups();

					if (_rallycross.GroupManager.ActiveGroupe.CanBeStarted) {
						_stageState = StageSate.groupeActivated;
						LogStageState();
						SetControlButtonTextToAllDirectors("^2Start");
					} else {
						_stageState = StageSate.groupeWaitingForAiDrivers;
						LogStageState();
						SetControlButtonTextToAllDirectors(String.Format("^3Add Ai driver(s) ({0})", _rallycross.GroupManager.ActiveGroupe.RequestedAiDriversCount));
					}
				} break;

				case StageSate.groupeWaitingForAiDrivers : {
					_stageState = StageSate.groupeWaitingForAiDrivers;
					LogStageState();
					SetControlButtonTextToAllDirectors(String.Format("^3Add Ai driver(s) ({0})", _rallycross.GroupManager.ActiveGroupe.RequestedAiDriversCount));
				} break;

				case StageSate.groupeAllAiDriversAdded : {
					_stageState = StageSate.groupeActivated;
					LogStageState();
					SetControlButtonTextToAllDirectors("^2Start");
				} break;

				case StageSate.groupeActivated : {
					_countDown.Start();
					_stageState = StageSate.countdownStarted;
					LogStageState();
					HideControlButtonTextToAllDirectors();
				} break;

				case StageSate.raceStarted : {
					_stageState = StageSate.raceRestarting;
					LogStageState();
					HideControlButtonTextToAllDirectors();
					_raceDirector.StartRace();
				} break;

				case StageSate.raceRestarting : {
					_messaging.SendRaceControlMessageToAll("^1Race stopped", 15);

					_rallycross.GroupManager.ActiveGroupe.RaceRestartting();

					if (_rallycross.GroupManager.ActiveGroupe.CanBeStarted) {
						_stageState = StageSate.raceRestarting;
						LogStageState();
						SetControlButtonTextToAllDirectors("^2Start");
					} else {
						_stageState = StageSate.groupeWaitingForAiDrivers;
						LogStageState();
						SetControlButtonTextToAllDirectors(String.Format("^3Add Ai driver(s) ({0})", _rallycross.GroupManager.ActiveGroupe.RequestedAiDriversCount));
					}
				} break;

				case StageSate.raceRestart : {
					_stageState = StageSate.raceRestarting;
					LogStageState();
					HideControlButtonTextToAllDirectors();
					_raceDirector.StartRace();
				} break;

				case StageSate.raceFinished: {
					_stageState = StageSate.watingForFinishOfAllDrivers;
					LogStageState();
					HideControlButtonTextToAllDirectors();
				} break;

				case StageSate.allDriversFinished: {
					_stageState = StageSate.activateNextGroup;
					LogStageState();
					ShowControlButtonTextToAllDirectors();
					SetControlButtonTextToAllDirectors("^3Activate next group");
				} break;

				case StageSate.activateNextGroup: {
					_rallycross.GroupManager.ActivateNextGroupe();
					if (_rallycross.GroupManager.ActiveGroupe.CanBeStarted) {
						_stageState = StageSate.groupeActivated;
						LogStageState();
						SetControlButtonTextToAllDirectors("^2Start");
					} else {
						_stageState = StageSate.groupeWaitingForAiDrivers;
						LogStageState();
						SetControlButtonTextToAllDirectors(String.Format("^3Add Ai driver(s) ({0})", _rallycross.GroupManager.ActiveGroupe.RequestedAiDriversCount));
					}
					_rallycross.GroupManager.ShowGroups();
				} break;
			}
		}

		private void _countDown_Elapsed(object sender, EventArgs e)
		{
			Debug.Assert(_stageState==StageSate.countdownStarted);
			_rallycross.GroupManager.SetGridForActiveGroup();
			_rallycross.GroupManager.HideGroups();
			_raceDirector.StartRace();
		}

		private void RaceDirector_StateChanged(object sender, RaceStateChangedEventArgs e)
		{
			if (_rallycrossState == RallycrossSate.none) {
				return;
			}

			if (e.OldRaceSate == RaceState.Undefined) {
				return;
			}
			_log.Debug("RaceDirector_StateChanged");

			switch (e.NewRaceSate) {
				case RaceState.RaceFinished: {
					_stageState = StageSate.raceFinished;
					LogStageState();
					NextStage();
				} break;

				case RaceState.RaceFinishedByAllCars : {
					_stageState = StageSate.allDriversFinished;
					LogStageState();
					NextStage();
				} break;

				case RaceState.RaceStarted : {
					_stageState = StageSate.raceRestarting;
					LogStageState();
					SetControlButtonTextToAllDirectors("^1Restart");
					ShowControlButtonTextToAllDirectors();
				} break;

				case RaceState.RaceRestarted : {
					_stageState = StageSate.raceRestarting;
					LogStageState();
					SetControlButtonTextToAllDirectors("^1Restart");
					ShowControlButtonTextToAllDirectors();
				} break;
			}
		}

		private void SetControlButtonTextToAllDirectors(String text)
		{
			foreach(RallycrossDirectorMenu menu in _menu.Values) {
				menu.SetControlButtonText(text);
			}
		}

		private void ShowControlButtonTextToAllDirectors()
		{
			RallycrossDirectorMenu.CanBeButtonDisplayed = true;
			foreach(RallycrossDirectorMenu menu in _menu.Values) {
				menu.ControlButtonShow();
			}
		}

		private void HideControlButtonTextToAllDirectors()
		{
			RallycrossDirectorMenu.CanBeButtonDisplayed = false;
			foreach(RallycrossDirectorMenu menu in _menu.Values) {
				menu.ControlButtonHide();
			}
		}

		private void CarDriverManager_AiDriverStateChanged(object sender, AiDriverStateChangedEventArgs e)
		{
			switch (_stageState) {
				case StageSate.groupeWaitingForAiDrivers : {
					_rallycross.GroupManager.ActiveGroupe.CarDriverManager_AiDriverStateChanged(sender, e);
					SetControlButtonTextToAllDirectors(String.Format("^3Add Ai driver(s) ({0})", _rallycross.GroupManager.ActiveGroupe.RequestedAiDriversCount));
					if (_rallycross.GroupManager.ActiveGroupe.CanBeStarted) {
						Debug.Assert(e.Added);
						_stageState = StageSate.groupeAllAiDriversAdded;
						LogStageState();
						StageStateGroup();
					}
				} break;

				case StageSate.groupeActivated : {
					_rallycross.GroupManager.ActiveGroupe.CarDriverManager_AiDriverStateChanged(sender, e);
					if (!_rallycross.GroupManager.ActiveGroupe.CanBeStarted) {
						Debug.Assert(!e.Added);
						SetControlButtonTextToAllDirectors(String.Format("^3Add Ai driver(s) ({0})", _rallycross.GroupManager.ActiveGroupe.RequestedAiDriversCount));
						_stageState = StageSate.groupeWaitingForAiDrivers;
						LogStageState();
						StageStateGroup();
					}
				} break;
			}
		}

		private void GetPlugins()
		{
			IPluginManager pluginManager = _rallycross.PluginManager;
			_carDriverManager = pluginManager.GetPlugin(CmlPlugins.CarDriverManagerGuid) as ICarDriverManager;
			if (_carDriverManager == null) {
				_log.Fatal("Car and Driver Manager wasn't found!");
				throw new PluginActivateException("Car and Driver Manager wasn't found");
			}

			_controlFactory = pluginManager.GetPlugin(CmlPlugins.ControlFactoryGuid) as IControlFactory;
			if (_controlFactory == null) {
				_log.Fatal("Control factory wasn't found!");
				throw new PluginActivateException("Control factory wasn't found");
			}

			_messaging = pluginManager.GetPlugin(CmlPlugins.MessagingGuid) as IMessaging;
			if (_messaging == null) {
				_log.Fatal("Messaging wasn't found!");
				throw new PluginActivateException("Messaging wasn't found");
			}

			_raceDirector = pluginManager.GetPlugin(CmlPlugins.RaceDirectorGuid) as IRaceDirector;
			if (_raceDirector == null) {
				_log.Fatal("Race director wasn't found!");
				throw new PluginActivateException("Race director wasn't found");
			}
		}

		[Conditional("DEBUG")]
		private void LogStageState()
		{
			if (_logDebug) {
				_log.DebugFormat("Stage State: '{0}'", _stageState);
			}
		}
	}
}
