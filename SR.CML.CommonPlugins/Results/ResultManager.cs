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

using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.Results
{
	internal class ResultManager : IResultManager
	{
		private static ILog		_log		= LogManager.GetLogger(typeof(ResultManager));
		private static bool		_logDebug	= _log.IsDebugEnabled;

		private bool								_disposed			= false;
		private	InSimHandler						_inSimHandler		= null;
		private	ICarDriverManager					_carDriverManager	= null;
		private Dictionary<IInSimCar, RaceResult>	_carsResults		= null;
		private Byte								_splitsCount		= 0;
		private bool								_racing				= false;

		internal Byte SplitsCount
		{
			get { return _splitsCount; }
		}

		internal ResultManager(IPluginManager pluginManager)
		{
			Debug.Assert(pluginManager!=null);
			if (pluginManager==null) {
				_log.Fatal("IPluginManager isn't set!");
				throw new ArgumentNullException("IPluginManager isn't set!");
			}

			ICMLCore cmlCore = pluginManager.GetPlugin(CmlPlugins.CMLCoreGuid) as ICMLCore;
			Debug.Assert(cmlCore!=null);
			if (cmlCore==null) {
				_log.Fatal("CML Core plugin wasn't found");
				throw new ArgumentNullException("CML Core plugin wasn't found!");
			}

			_inSimHandler = cmlCore.InSimHandler;
			Debug.Assert(_inSimHandler!=null);
			if (_inSimHandler==null) {
				_log.Fatal("InSimHandler isn't set!");
				throw new ArgumentNullException("InSimHandler isn't set!");
			}

			_carDriverManager = pluginManager.GetPlugin(CmlPlugins.CarDriverManagerGuid) as ICarDriverManager;
			Debug.Assert(_carDriverManager!=null);
			if (_carDriverManager==null) {
				_log.Fatal("ICarDriverManager plugin wasn't found!");
				throw new ArgumentNullException("ICarDriverManager plugin wasn't found!");
			}

			_carsResults = new Dictionary<IInSimCar,RaceResult>(32);
		}

		~ResultManager()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "ResultManager, call dispose before the application is closed!");
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
				}
				_disposed = true;
			}
		}

		#region IResultManager

		public event EventHandler<EventArgs> AllCarsFinished;
		public event EventHandler<CarFinishedEventArgs> CarFinished;

		public IRaceResult GetResult(IInSimCar car)
		{
			if (!_carsResults.ContainsKey(car)) {
				return null;
			}

			return _carsResults[car];
		}

		private void ForceFinish() {
			foreach (RaceResult raceRsult in _carsResults.Values) {
				raceRsult.Finished = true;
			}
		}

		public IList<IRaceResult> GetSortedRaceResults()
		{
			ForceFinish();
			List<IRaceResult> sortedResult = new List<IRaceResult>(_carsResults.Values.Count);
			foreach (IRaceResult raceRsult in _carsResults.Values) {
				sortedResult.Add(raceRsult);
			}
			sortedResult.Sort(RaceResult.RaceResultComparer);
			return sortedResult.AsReadOnly();
		}

		public IList<IRaceResult> GetSortedQualifyResults() {
			ForceFinish();
			List<IRaceResult> sortedResult = new List<IRaceResult>(_carsResults.Values.Count);
			foreach (IRaceResult raceRsult in _carsResults.Values) {
				sortedResult.Add(raceRsult);
			}
			sortedResult.Sort(RaceResult.QualifyResultComparer);
			return sortedResult.AsReadOnly();
		}

		public void ClearResults()
		{
			_carsResults.Clear();
		}

		#endregion

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

		private void BindEvents()
		{
			Debug.Assert(_inSimHandler!=null);

			_inSimHandler.RaceTrackRaceStart		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceStartHandler(_inSimHandler_RaceTrackRaceStart);
			_inSimHandler.RaceTrackRaceEnd			+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceEndHandler(_inSimHandler_RaceTrackRaceEnd);
			_inSimHandler.RaceTrackPlayerLap		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerLapHandler(_inSimHandler_RaceTrackPlayerLap);
			_inSimHandler.RaceTrackPlayerResult		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerResultHandler(_inSimHandler_RaceTrackPlayerResult);
			_inSimHandler.RaceTrackPlayerFinish		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerFinishHandler(_inSimHandler_RaceTrackPlayerFinish);
			_inSimHandler.RaceTrackPlayerSplitTime	+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerSplitTimeHandler(_inSimHandler_RaceTrackPlayerSplitTime);

			_carDriverManager.CarStateChanged += new EventHandler<CarStateChangedEventArgs>(CarDriverManager_CarStateChanged);
		}

		private void UnbindEvents()
		{
			Debug.Assert(_inSimHandler!=null);

			_inSimHandler.RaceTrackRaceStart		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceStartHandler(_inSimHandler_RaceTrackRaceStart);
			_inSimHandler.RaceTrackRaceEnd			-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceEndHandler(_inSimHandler_RaceTrackRaceEnd);
			_inSimHandler.RaceTrackPlayerLap		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerLapHandler(_inSimHandler_RaceTrackPlayerLap);
			_inSimHandler.RaceTrackPlayerResult		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerResultHandler(_inSimHandler_RaceTrackPlayerResult);
			_inSimHandler.RaceTrackPlayerFinish		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerFinishHandler(_inSimHandler_RaceTrackPlayerFinish);
			_inSimHandler.RaceTrackPlayerSplitTime	-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerSplitTimeHandler(_inSimHandler_RaceTrackPlayerSplitTime);

			_carDriverManager.CarStateChanged -= new EventHandler<CarStateChangedEventArgs>(CarDriverManager_CarStateChanged);
		}

		private void _inSimHandler_RaceTrackRaceStart(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackRaceStart e)
		{
			_racing = true;
			_splitsCount = 0;
			bool isntFinishLineSplit = true;

			if (e.Split1NodeIndex != Int32.MaxValue) {
				_splitsCount++;
			} else {
				isntFinishLineSplit = false;
				_splitsCount++;
			}

			if (e.Split2NodeIndex != UInt16.MaxValue && isntFinishLineSplit) {
				_splitsCount++;
			} else {
				isntFinishLineSplit = false;
				_splitsCount++;
			}

			if (e.Split3NodeIndex != UInt16.MaxValue && isntFinishLineSplit) {
				_splitsCount++;
			}

			ClearResults();
		}

		private void _inSimHandler_RaceTrackRaceEnd(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackRaceEnd e)
		{
			_racing = false;
		}

		private void _inSimHandler_RaceTrackPlayerLap(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerLap e)
		{
			Debug.Assert(_carDriverManager!=null);

			RaceResult raceResult = GetRaceRsult(e.PlayerId);
			if (raceResult==null) {
				return;
			}

			if (raceResult.Finished) {
				return;
			}

			raceResult.CurrentLap.Time = e.LapTime;
			raceResult.AddNewLap();
		}

		private void _inSimHandler_RaceTrackPlayerResult(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerResult e)
		{
			_racing = false;

			Debug.Assert(_carDriverManager!=null);

			RaceResult raceResult = GetRaceRsult(e.PlayerId);
			if (raceResult==null) {
				return;
			}

			if (e.Confirmed) {
				Debug.Assert(raceResult.Finished==true);
				raceResult.DNF = e.Disqualified;

				if (CarFinished!=null) {
					IInSimCar car = _carDriverManager.GetCarByPlayerID(e.PlayerId);
					if (car!=null) {
						CarFinished(this, new CarFinishedEventArgs(car, raceResult));
					} else {
						Debug.Assert(_carDriverManager.IsAiDriver(e.PlayerId));
					}
				}
				AllCarsFinishedInternal();
			}
		}

		private void _inSimHandler_RaceTrackPlayerFinish(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerFinish e)
		{
			_racing = false;

			RaceResult raceResult = GetRaceRsult(e.PlayerId);
			if (raceResult==null) {
				return;
			}

			Debug.Assert(raceResult.Finished==false);
			raceResult.Finished	= true;
		}

		private void _inSimHandler_RaceTrackPlayerSplitTime(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerSplitTime e)
		{
			Debug.Assert(_carDriverManager!=null);

			RaceResult raceResult = GetRaceRsult(e.PlayerId);
			if (raceResult==null) {
				return;
			}

			raceResult.CurrentLap.SetSplit(e.Split, e.SplitTime);
		}

		void CarDriverManager_CarStateChanged(object sender, CarStateChangedEventArgs e)
		{
			switch (e.NewState) {
				case CarState.LeavingTrack : {
					AllCarsFinishedInternal();
				} break;
			}
		}

		private RaceResult GetRaceRsult(Byte playerId)
		{
			IInSimCar car = _carDriverManager.GetCarByPlayerID(playerId);
			if (car==null) {
				if (_carDriverManager.IsAiDriver(playerId)) {
					return null;
				}

				Debug.Assert(car!=null);
				_log.ErrorFormat("Some car did lap/split, but isn't added in cars! Player id {0}", playerId);
				return null;
			}

			if (_carsResults.ContainsKey(car)) {
				return _carsResults[car];
			}

			RaceResult raceResult = new RaceResult(this, car);
			_carsResults.Add(car, raceResult);

			return raceResult;
		}

		private bool AllCarsFinishedInternal()
		{
			SetFinishedForAllNonRacingCars();

			foreach (RaceResult raceResult in _carsResults.Values) {
				if (raceResult.Finished == false) {
					return false;
				}
			}

			if (_carsResults.Count > 0 && AllCarsFinished!=null) {
				AllCarsFinished(this, new EventArgs());
			}

			return true;
		}

		private void SetFinishedForAllNonRacingCars()
		{
			if (_racing) {
				return;
			}

			RaceResult raceResult = null;

			foreach (IInSimCar car in _carsResults.Keys) {
				raceResult = _carsResults[car];

				Debug.Assert(raceResult!=null);
				if (raceResult==null) {
					_log.ErrorFormat("Missing Raceresult for car {0}", car.Number);
					continue;
				}

				if (car.State==CarState.LeavingTrack) {
					Debug.Assert(car.ActiveDriver.State != DriverState.InCar);
					raceResult.Finished = true;
				}
			}
		}

	}
}
