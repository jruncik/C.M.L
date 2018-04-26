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
using System.Text;
using System.Collections.Generic;

using SR.CML.Common;
using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;
using SR.CML.CommonPlugins.CarDriverManager;

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins
{
	internal class RaceDirector : IRaceDirector
	{
		private static ILog _log = LogManager.GetLogger(typeof(RaceDirector));
		private static bool _logDebug = _log.IsDebugEnabled;

		private bool				_disposed			= false;
		private RaceState			_state				= RaceState.Undefined;
		private InSimHandler		_inSimHandler		= null;
		private IMessaging			_messaging			= null;
		private ICMLCore			_cmlCore			= null;
		private ICarDriverManager	_carDriverManager	= null;
		private IResultManager		_resultManager		= null;

		private String[]			_driversOrder			= new String[0];
		private bool				_ensureEmptySpaceOnGrid	= false;

		private FullMotion.LiveForSpeed.InSim.Enums.RaceState _raceState = FullMotion.LiveForSpeed.InSim.Enums.RaceState.NoRace;

		public RaceDirector(IPluginManager pluginManager)
		{
			Debug.Assert(pluginManager!=null);
			if (pluginManager==null) {
				_log.Fatal("IPluginManager isn't set!");
				throw new ArgumentNullException("IPluginManager isn't set!");
			}

			GetRequestedPlugins(pluginManager);
		}

		~RaceDirector()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "RaceDirector, call dispose before the application is closed!");
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

					UnbindEvents();

					if (_logDebug) {
						_log.Debug("Disposed");
					}
				}
				_disposed = true;
			}
		}

		internal bool Activate()
		{
			BindEvents();
			return true;
		}

		internal bool Deactivate()
		{
			UnbindEvents();
			return true;
		}

		#region IRaceDirector
		public event EventHandler<RaceStateChangedEventArgs> StateChanged;

		public RaceState State
		{
			get { return _state; }
		}

		public void SetGrid(String[] driversOrder, bool ensureEmptySpaceOnGrid)
		{
			if (driversOrder.Length <= 0) {
				return;
			}

			_driversOrder			= driversOrder;
			_ensureEmptySpaceOnGrid = ensureEmptySpaceOnGrid;

			List<InSimDriverAi> aiDrivers = new List<InSimDriverAi>(8);
			foreach (IInSimDriverAi aiDriver in _carDriverManager.GetAiDrivers()) {
				InSimDriverAi aiDriverTmp = aiDriver as InSimDriverAi;
				if (aiDriverTmp!=null && !aiDriverTmp.UsedInGrid) {
					aiDrivers.Add(aiDriverTmp);
				}
			}

			Byte[]			playerIds	= new Byte[driversOrder.Length];
			IInSimCar		car			= null;
			Int32			index		= 0;
			bool			idSet		= false;
			StringBuilder	gridIds		= new StringBuilder(512);

			foreach(String playerName in driversOrder) {
				idSet	= false;
				car		= _carDriverManager.GetCarByLFSName(playerName);	// in car can be other driver from crew

				gridIds.Length = 0;
				gridIds.Append(playerName);

				if (car!=null) {
					if (car.ActiveDriver!=null) {
						if (car.ActiveDriver.PlayerId!=null) {
							playerIds[index]	= car.ActiveDriver.PlayerId.Value;
							idSet				= true;
							gridIds.Append(":'");
							gridIds.Append(car.ActiveDriver.PlayerId.Value);
							gridIds.Append("',");
						}
					}
				}

				if (ensureEmptySpaceOnGrid && !idSet) {
					if (aiDrivers.Count > 0) {
						gridIds.Append(":'empty -'");
						gridIds.Append(aiDrivers[0].PlayerId);
						gridIds.Append("',");
						playerIds[index] = aiDrivers[0].PlayerId;
						aiDrivers.RemoveAt(0);
					} else {
						gridIds.Append(":'empty - missing AI',");
						//Debug.Assert(false, "Not enought AI drivers for empty space on start grid");
						_log.ErrorFormat("Missing AI driver for empty space on grid for player '{0}'", playerName);
					}
				}

				if (_logDebug) {
					_log.Debug(gridIds.ToString());
				}

				++index;
			}
			_inSimHandler.SetGridOrder(playerIds);
		}

		public void StartRace()
		{
			if (_logDebug) {
				_log.Debug("Starting race");
			}
			_cmlCore.SendCommand(LfsCommands.StarRacte());
		}

		public void StartRace(Int32 laps)
		{
			if (_logDebug) {
				_log.DebugFormat("Starting race, laps {0}", laps.ToString());
			}
			SetGrid(_driversOrder, _ensureEmptySpaceOnGrid);

			_cmlCore.SendCommand(LfsCommands.SetRaceTime(0));
			_cmlCore.SendCommand(LfsCommands.SetRaceLaps(laps));
			_cmlCore.SendCommand(LfsCommands.StarRacte());
		}

		public void StartRace(TimeSpan time)
		{
			if (_logDebug) {
				_log.DebugFormat("Starting race, hours {0}", time.ToString());
			}
			SetGrid(_driversOrder, _ensureEmptySpaceOnGrid);

			_cmlCore.SendCommand(LfsCommands.SetRaceLaps(0));
			_cmlCore.SendCommand(LfsCommands.SetRaceTime(time.Hours));
			_cmlCore.SendCommand(LfsCommands.StarRacte());
		}

		public void StartQualify(Int32 minuts)
		{
			if (_logDebug) {
				_log.DebugFormat("Starting qulify, minuts {0}", minuts.ToString());
			}
			_cmlCore.SendCommand(LfsCommands.SetQualifyTime(minuts));
			_cmlCore.SendCommand(LfsCommands.StarQualify());
		}

		public void StartPractice()
		{
			if (_logDebug) {
				_log.Debug("Starting practice");
			}
			_cmlCore.SendCommand(LfsCommands.SetRaceLaps(0));
			_cmlCore.SendCommand(LfsCommands.SetRaceTime(0));
			_cmlCore.SendCommand(LfsCommands.StarRacte());
		}
		#endregion

		private void GetRequestedPlugins(IPluginManager pluginManager)
		{
			_cmlCore = pluginManager.GetPlugin(CmlPlugins.CMLCoreGuid) as ICMLCore;
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

			_messaging = pluginManager.GetPlugin(CmlPlugins.MessagingGuid) as IMessaging;
			Debug.Assert(_inSimHandler != null);
			if (_inSimHandler == null) {
				_log.Fatal("IMessaging plugin wasn't found!");
				throw new ArgumentNullException("IMessaging plugin wasn't found!");
			}

			_carDriverManager = pluginManager.GetPlugin(CmlPlugins.CarDriverManagerGuid) as ICarDriverManager;
			Debug.Assert(_carDriverManager != null);
			if (_carDriverManager == null) {
				_log.Fatal("ICarDriverManager plugin wasn't found!");
				throw new ArgumentNullException("ICarDriverManager plugin wasn't found!");
			}

			_resultManager = pluginManager.GetPlugin(CmlPlugins.ResultManagerGuid) as IResultManager;
			Debug.Assert(_resultManager != null);
			if (_resultManager == null) {
				_log.Fatal("IResultManager plugin wasn't found!");
				throw new ArgumentNullException("IResultManager plugin wasn't found!");
			}
		}

		private void BindEvents()
		{
			_inSimHandler.LFSState				+= new FullMotion.LiveForSpeed.InSim.EventHandlers.StateEventHandler(_inSimHandler_LFSState);
			_inSimHandler.RaceTrackRaceStart	+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceStartHandler(_inSimHandler_RaceTrackRaceStart);
			_inSimHandler.RaceTrackRaceEnd		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceEndHandler(_inSimHandler_RaceTrackRaceEnd);
			_inSimHandler.RaceTrackPlayerResult	+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerResultHandler(_inSimHandler_RaceTrackPlayerResult);
			_resultManager.AllCarsFinished		+= new EventHandler<EventArgs>(_resultManager_AllCarsFinished);
		}

		private void UnbindEvents()
		{
			_inSimHandler.LFSState				-= new FullMotion.LiveForSpeed.InSim.EventHandlers.StateEventHandler(_inSimHandler_LFSState);
			_inSimHandler.RaceTrackRaceStart	-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceStartHandler(_inSimHandler_RaceTrackRaceStart);
			_inSimHandler.RaceTrackRaceEnd		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceEndHandler(_inSimHandler_RaceTrackRaceEnd);
			_inSimHandler.RaceTrackPlayerResult	-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerResultHandler(_inSimHandler_RaceTrackPlayerResult);
			_resultManager.AllCarsFinished		-= new EventHandler<EventArgs>(_resultManager_AllCarsFinished);
		}

		private void _inSimHandler_LFSState(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.State e)
		{
			_raceState = e.RaceState;

			if (_state==RaceState.Undefined) {
				switch (e.RaceState) {
					case FullMotion.LiveForSpeed.InSim.Enums.RaceState.NoRace : {
						OnSetRaceSate(RaceState.GameSetupScrean);
					} break;

					case FullMotion.LiveForSpeed.InSim.Enums.RaceState.Qualifying : {
						OnSetRaceSate(RaceState.QualifyStarted);
					} break;

					case FullMotion.LiveForSpeed.InSim.Enums.RaceState.Race : {
						OnSetRaceSate(RaceState.RaceStarted);
					} break;
				}
				return;
			}

			if (_raceState==FullMotion.LiveForSpeed.InSim.Enums.RaceState.NoRace) {
				OnSetRaceSate(RaceState.GameSetupScrean);
			}
		}

		private void _inSimHandler_RaceTrackRaceStart(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackRaceStart e)
		{
			_carDriverManager.RemoveAllAiDrivers();

			if (e.QualifyingMinutes!=0) {
				OnSetRaceSate(RaceState.QualifyStarted);
				return;
			}

			if (_state==RaceState.RaceStarted) {
				OnSetRaceSate(RaceState.RaceRestarted);
				return;
			}

			OnSetRaceSate(RaceState.RaceStarted);
		}

		private void _inSimHandler_RaceTrackRaceEnd(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackRaceEnd e)
		{
			switch (_raceState) {
				case FullMotion.LiveForSpeed.InSim.Enums.RaceState.Qualifying: {
					OnSetRaceSate(RaceState.QualifyFinished);
				} break;

				case FullMotion.LiveForSpeed.InSim.Enums.RaceState.Race: {
					OnSetRaceSate(RaceState.RaceFinished);
				} break;
			}
		}

		void _inSimHandler_RaceTrackPlayerResult(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerResult e)
		{
			switch (_raceState) {
				case FullMotion.LiveForSpeed.InSim.Enums.RaceState.Qualifying: {
					OnSetRaceSate(RaceState.QualifyFinished);
				} break;

				case FullMotion.LiveForSpeed.InSim.Enums.RaceState.Race: {
					OnSetRaceSate(RaceState.RaceFinished);
				} break;
			}
		}

		void _resultManager_AllCarsFinished(object sender, EventArgs e)
		{
			OnSetRaceSate(RaceState.RaceFinishedByAllCars);
		}

		private void OnSetRaceSate(RaceState raceState)
		{
			if (_state == raceState) {
				return;
			}

			RaceState oldState = _state;

			if (oldState == RaceState.RaceFinishedByAllCars && raceState == RaceState.RaceFinished) {
				if (_logDebug) {
					_log.Debug("RaceFinished has to be send before as fake state");
				}
				return;
			}

			_state = raceState;

			if (_state == RaceState.RaceFinishedByAllCars && oldState != RaceState.RaceFinished) {
				if (_logDebug) {
					_log.DebugFormat("Fire fake RaceFinished. Old '{0}', New '{1}'.", oldState.ToString(), _state.ToString());
				}
				FireSetRaceState(oldState, RaceState.RaceFinished);
				oldState = RaceState.RaceFinished;
			}

			FireSetRaceState(oldState, _state);
		}

		private void FireSetRaceState(RaceState oldState, RaceState newState)
		{
			if (_logDebug) {
				_log.DebugFormat("New race state. Old '{0}', New '{1}'.", oldState.ToString(), newState.ToString());
			}

			if (StateChanged!=null) {
				StateChanged(this, new RaceStateChangedEventArgs(oldState, newState));
			}
		}
	}
}
