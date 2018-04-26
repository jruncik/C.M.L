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

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	class Rallycross : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(Rallycross));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool							_disposed			= false;
		private RallycrossDirector				_director			= null;
		private Dictionary<String, DriverInfo>	_raceDirectors		= null;
		private Results							_results			= null;
		private ResultsView						_resultsView		= null;
		private IResultManager					_resultManager		= null;

		private IPluginManager _pluginManager;
		internal IPluginManager PluginManager
		{
			get { return _pluginManager; }
		}

		private ICarDriverManager _carDriverManager;
		internal ICarDriverManager CarDriverManager
		{
			get { return _carDriverManager; }
		}

		private ICMLCore _cmlCore;
		//internal IMessaging Messaging
		//{
		//    get { return _messaging; }
		//}

		private	GroupManager _groupManager	= null;
		internal GroupManager GroupManager
		{
			get { return _groupManager; }
		}

		private List<DriverInfo> _drivers = null;
		internal List<DriverInfo> Drivers
		{
			get { return _drivers; }
		}

		public Rallycross(IPluginManager pluginManager)
		{
			if (pluginManager==null) {
				_log.Fatal("Plugin Manager is null!");
				throw new ArgumentNullException("Plugin Manager is null!");
			}
			_pluginManager = pluginManager;

			_carDriverManager = _pluginManager.GetPlugin(CmlPlugins.CarDriverManagerGuid) as ICarDriverManager;
			if (_carDriverManager == null) {
				_log.Fatal("Car and Driver Manager wasn't found!");
				throw new PluginActivateException("Car and Driver Manager wasn't found");
			}

			_resultManager = pluginManager.GetPlugin(CmlPlugins.ResultManagerGuid) as IResultManager;
			if (_resultManager == null) {
				_log.Fatal("Result Manager wasn't found!");
				throw new PluginActivateException("Result Manager wasn't found");
			}

			_cmlCore = _pluginManager.GetPlugin(CmlPlugins.CMLCoreGuid) as ICMLCore;
			if (_cmlCore == null) {
				_log.Fatal("CML Core wasn't found!");
				throw new PluginActivateException("CML Core wasn't found");
			}

			_raceDirectors	= new Dictionary<String, DriverInfo>(8);
			_director		= new RallycrossDirector(this);
			_groupManager	= new GroupManager(this);
			_results		= new Results(this);
			_resultsView	= new ResultsView(this);

			BindEvents();
			GetRaceDirectores();
		}

		~Rallycross()
		{
			Debug.Assert(_disposed, "Rallycross, call dispose before the application is closed!");
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

					if (_director!=null) {
						_director.Dispose();
						_director = null;
					}

					if (_groupManager!=null) {
						_groupManager.Dispose();
						_groupManager = null;
					}

					if (_drivers!=null) {
						foreach(DriverInfo driverInfo in _drivers) {
							driverInfo.Dispose();
						}
						_drivers.Clear();
						_drivers = null;
					}

					if (_raceDirectors!=null) {
						foreach(DriverInfo driverInfo in _raceDirectors.Values) {
							if (driverInfo!=null) {
								driverInfo.Dispose();
							}
						}
						_raceDirectors.Clear();
						_raceDirectors = null;
					}

					if (_results!=null) {
						_results.Dispose();
						_results = null;
					}

					if (_resultsView!=null) {
						//_resultsView.Dispose();
						_resultsView = null;
					}
				}
				_disposed = true;
			}
		}

		private void BindEvents()
		{
			_carDriverManager.DriverStateChanged	+= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
			_groupManager.ActivatingNextGroup		+= new EventHandler<ActivatingNextGroupEventArgs>(_groupManager_NextHeatActivated);
		}

		private void UnbindEvents()
		{
			_carDriverManager.DriverStateChanged	-= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
			_groupManager.ActivatingNextGroup		-= new EventHandler<ActivatingNextGroupEventArgs>(_groupManager_NextHeatActivated);
		}

		internal void InitRacingEvent()
		{
			GetAllRacingDrivers();

			_groupManager.CreateBasicGroups();
			//_groupManager.CreateFinalGroup("Final B");
			//_groupManager.CreateFinalGroup("Final A");
			

			_results.Initialize();
		}

		private void GetAllRacingDrivers()
		{
			if (_drivers==null) {
				_drivers = new List<DriverInfo>(_carDriverManager.Drivers.Count);
			} else {
				_drivers.Clear();
			}

			DriverInfo driverinfo = null;
			foreach (IInSimDriver inSimDriver in _carDriverManager.Drivers) {

				driverinfo = null;
				if (IsRaceDirector(inSimDriver.LfsName)) {
					driverinfo = GetDirectorDriverInfo(inSimDriver.LfsName);

					Debug.Assert(driverinfo!=null);
					Debug.Assert(driverinfo.Driver==inSimDriver);
				} else {
					driverinfo = new DriverInfo(inSimDriver);
				}

				if (driverinfo.CanDrive) {
					_drivers.Add(driverinfo);
				}
			}
		}

		internal DriverInfo GetDriverInfo(String lfsDrivername)
		{
			if (_drivers==null) {
				return null;
			}

			foreach(DriverInfo driverInfo in _drivers) {
				if (driverInfo.LfsUserName == lfsDrivername) {
					return driverInfo;
				}
			}

			return null;
		}

		internal bool IsRaceDirector(String lfsName)
		{
			return _raceDirectors.ContainsKey(lfsName);
		} 

		internal DriverInfo GetDirectorDriverInfo(String lfsName)
		{
			if (_raceDirectors==null) {
				return null;
			}

			if (!_raceDirectors.ContainsKey(lfsName)) {
				return null;
			}

			return _raceDirectors[lfsName];
		}

		private void CarDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e)
		{
			DriverInfo driverInfo = GetDriverInfo(e.Driver.LfsName);

			if (e.OldState == DriverState.Undefined && e.NewState == DriverState.Connected) {
				if (_raceDirectors.ContainsKey(e.Driver.LfsName)) {
					Debug.Assert(_raceDirectors[e.Driver.LfsName]!=null);
					Debug.Assert(_raceDirectors[e.Driver.LfsName].Driver == null);
					_raceDirectors[e.Driver.LfsName].SetInSimDriver(e.Driver);
				}
				if (driverInfo!=null) {
					driverInfo.SetInSimDriver(e.Driver);
				}
			}

			if (e.NewState == DriverState.Disconnecting) {
				if (_raceDirectors.ContainsKey(e.Driver.LfsName)) {
					if (_raceDirectors[e.Driver.LfsName]!=null) {
						_raceDirectors[e.Driver.LfsName].RemoveInSimDriver();
					}
				}
				if (driverInfo!=null) {
					driverInfo.RemoveInSimDriver();
				}
			}

			if (_director!=null) {
				_director.CarDriverManager_DriverStateChanged(sender, e);
			}

			if (e.NewState == DriverState.InCar) {
				if (driverInfo==null || !driverInfo.Active) {
					_cmlCore.SendCommand(LfsCommands.Spectate(e.Driver));
					return;
				}
			}
		}

		private void _groupManager_NextHeatActivated(object sender, ActivatingNextGroupEventArgs e)
		{
			UpdateHeatResults(e);

			if (e.NewHeatWillBeActivated) {
				_results.CalculateHeatResults(e.HeatIndex);
				DisplayResults();
			}
		}

		private void DisplayResults()
		{
			_resultsView.ShowResults(_results.GetSortedResultsByPoints());
		}

		private void UpdateHeatResults(ActivatingNextGroupEventArgs e)
		{
			Group		finishedGroup	= e.FinishedGrupe;
			HeatResult	heatResult		= null;
			IInSimCar	car				= null;
			IRaceResult	raceResults		= null;

			foreach (DriverInfo driverInfo in finishedGroup.DriversInfo) {
				car			= _carDriverManager.GetCarByLFSName(driverInfo.LfsUserName);
				raceResults	= null;

				if (car==null) {
					heatResult = HeatResult.CreateDNFHeatResult();
				} else {
					raceResults = _resultManager.GetResult(car);
					if (raceResults==null) {
						_log.FatalFormat("There isn't race result for car '{0}'", car.Number);
						throw new ArgumentNullException(String.Format("There isn't race result for car '{0}'", car.Number));
					}
					heatResult = HeatResult.CreateHeatResult(raceResults.Time, raceResults.LapsCount, raceResults.DNF);
				}

				_results.SetResult(e.HeatIndex, driverInfo.LfsUserName, heatResult);
			}
		}

		private void GetRaceDirectores()
		{
			_raceDirectors.Add("babyonwheels", new DriverInfo(false));
			_raceDirectors.Add("bluun", new DriverInfo(true));
		}
	}
}
