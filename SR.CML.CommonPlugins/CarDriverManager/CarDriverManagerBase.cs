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
using SR.CML.CommonPlugins.Plugins;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Events;

using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.CarDriverManager
{
	internal abstract class CarDriverManagerBase : ICarDriverManager
	{
		protected static ILog	_log		= null;
		protected static bool	_logDebug	= false;

		protected	bool							_disposed			= false;

		protected	InSimHandler					_inSimHandler		= null;
		protected	IMessaging						_messaging			= null;
		protected	ICMLCore						_cmlCore			= null;

		protected	Dictionary<Byte, InSimDriver>	_activeDrivers		= null;
		protected	Dictionary<Byte, InSimDriver>	_inactiveDrivers	= null;
		protected	List<IInSimDriverAi>			_aiDrivers			= null;

		protected	List<InSimCar>					_cars				= null;
		private		IList<IInSimDriver>				_allDrivers			= null;
		private		IPluginManager					_pluginManager		= null;

		protected CarsAndDriversConfiguration _carsAndDriversConfiguration = null;
		internal CarsAndDriversConfiguration CarsAndDriversConfiguration {
			set { _carsAndDriversConfiguration = value; }
		}

		internal abstract void InitializeFromSetting();
		protected abstract void GetOrCreateCar(InSimDriver driver);
		protected abstract void RemoveCar(InSimDriver driver);
		protected abstract void ActivateDriver(InSimDriver driver, Byte playerId, RaceTrackPlayer e);
		protected abstract void DeactivateDriver(InSimDriver driver);
		protected abstract void SwitchDriversInCar(InSimDriver oldDriver, InSimDriver newDriver, Byte newPlayerId);
		protected abstract void DisposeDriversAndCars();
		protected abstract bool CheckCarRestrictions(InSimCar driverscar, List<String> reasons, RaceTrackPlayer e);

		protected CarDriverManagerBase(IPluginManager pluginManager)
		{
			Debug.Assert(pluginManager!=null);
			if (pluginManager==null) {
				_log.Fatal("IPluginManager isn't set!");
				throw new ArgumentNullException("IPluginManager isn't set!");
			}
			_pluginManager = pluginManager;

			_cmlCore = pluginManager.GetPlugin(CmlPlugins.CMLCoreGuid) as ICMLCore;
			Debug.Assert(_cmlCore != null);
			if (_cmlCore == null) {
				_log.Fatal("CML Core plugin wasn't found");
				throw new ArgumentNullException("CML Core plugin wasn't found!");
			}

			_inSimHandler = _cmlCore.InSimHandler;
			Debug.Assert(_inSimHandler!=null);
			if (_inSimHandler==null) {
				_log.Fatal("InSimHandler isn't set!");
				throw new ArgumentNullException("InSimHandler isn't set!");
			}

			_messaging = pluginManager.GetPlugin(CmlPlugins.MessagingGuid) as IMessaging;
			Debug.Assert(_inSimHandler != null);
			if (_inSimHandler == null) {
				_log.Fatal("IMessaging plugin wasn't found!");
				throw new ArgumentNullException("IMessaging plugin wasn't found!");
			}

			_aiDrivers = new List<IInSimDriverAi>(8);
		}

		~CarDriverManagerBase()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "CarDriverManager, call dispose before the application is closed!");
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
					Debug.Assert(_activeDrivers==null);
					Debug.Assert(_inactiveDrivers==null);
					Debug.Assert(_cars==null);

					if (_logDebug) {
						if (_activeDrivers!=null || _inactiveDrivers!=null || _cars!=null) {
							_log.Error("Drivers or cars weren't deleted!");
						}
						_log.Debug("Disposed");
					}
				}
				_disposed = true;
			}
		}

		public event EventHandler<DriverStateEventArgs>			DriverStateChanged;
		public event EventHandler<AiDriverStateChangedEventArgs> AiDriverStateChanged;
		public event EventHandler<CarStateChangedEventArgs> CarStateChanged;

		public IInSimCar GetCarByLFSName(String name)
		{
			name = name.ToLower();

			if (_activeDrivers!=null) {
				foreach(InSimDriver activeDriver in _activeDrivers.Values) {
					if (activeDriver.LfsName == name) {
						Debug.Assert(!activeDriver.InSimCar.IsEmpty, "Each driver has to have a car!");
						return activeDriver.InSimCar;
					}
				}
			}

			if (_inactiveDrivers!=null) {
				foreach(InSimDriver inactiveDriver in _inactiveDrivers.Values) {
					if (inactiveDriver.LfsName == name) {
						Debug.Assert(!inactiveDriver.InSimCar.IsEmpty, "Each driver has to have a car!");
						return inactiveDriver.InSimCar;
					}
				}
			}

			return null;
		}

		public IInSimCar GetCarByPlayerID(Byte id)
		{
			if (_activeDrivers.ContainsKey(id)) {
				Debug.Assert(!_activeDrivers[id].InSimCar.IsEmpty, "Each driver has to have a car!");
				return _activeDrivers[id].InSimCar;
			}

			return null;
		}

		public IInSimCar GetCarByConnectionID(Byte id)
		{
			InSimDriver driver = GetDriverByConnectionIDInternal(id);

			if (driver != null) {
				Debug.Assert(!driver.InSimCar.IsEmpty, "Each driver has to have a car!");
				return driver.InSimCar;
			}

			return null;
			
		}

		public IInSimDriver GetDriverByConnectionID(Byte id) {
			return GetDriverByConnectionIDInternal(id);
		}

		public IList<IInSimCar> Cars
		{
			get { return (IList<IInSimCar>)_cars.AsReadOnly(); }
		}

		public IList<IInSimDriver> Drivers
		{
			get {
				if (_allDrivers == null) {
					GenerateListOfAllConnectedDriversOnServer();
				}
				return _allDrivers;
			}
		}

		public IList<IInSimDriverAi> GetAiDrivers()
		{
			return _aiDrivers.AsReadOnly();
		}

		public void AddAiDriver()
		{
			_cmlCore.SendCommand(LfsCommands.AddAiDriver());
		}

		public void RemoveAllAiDrivers()
		{
			if (_logDebug) {
				StringBuilder removedAiDriversNames = new StringBuilder(256);
				removedAiDriversNames.Append("Removing all AI drivers: ");
				foreach (InSimDriverAi aiDriver in _aiDrivers) {
					removedAiDriversNames.Append(aiDriver.Name);
					removedAiDriversNames.Append(", ");
					_cmlCore.SendCommand(LfsCommands.Spectate(aiDriver));
				}
			_log.Debug(removedAiDriversNames.ToString());
			} else {
				foreach (InSimDriverAi aiDriver in _aiDrivers) {
					_cmlCore.SendCommand(LfsCommands.Spectate(aiDriver));
				}
			}
		}

		public bool IsAiDriver(Byte id)
		{
			foreach (InSimDriverAi aiDriver in _aiDrivers) {
				if (aiDriver.PlayerId == id) {
					return true;
				}
			}
			return false;
		}

		internal static CarDriverManagerBase CreateCarDriverManager(IPluginManager pluginManager, CMLMode mode)
		{
			switch (mode) {
				case CMLMode.Simple: {
					return new CarDriverManagerSimple(pluginManager);
				} //break;

				case CMLMode.Extended: {
					return new CarDriverManagerExtended(pluginManager);
				} //break;
			}

			Debug.Assert(false, String.Format("Unsupported CML mode! '{0}'", mode.ToString()));
			_log.Fatal(String.Format("Unsupported CML mode! '{0}'", mode.ToString()));
			throw new ArgumentException("Mode isn't supported");
		}

		internal bool Activate()
		{
			Debug.Assert(_activeDrivers==null);
			_activeDrivers = new Dictionary<Byte, InSimDriver>(32);

			Debug.Assert(_inactiveDrivers==null);
			_inactiveDrivers = new Dictionary<Byte, InSimDriver>(64);

			Debug.Assert(_cars==null);
			_cars = new List<InSimCar>(32);

			InitializeFromSetting();
			InjectCarDriverManIntoControlFactory();
			BindEvents();
			RequesConnectionAndPlayerInfo();

			return true;
		}

		internal bool Deactivate()
		{
			UnbindEvents();
			DisposeDriversAndCars();

			return true;
		}

		private void BindEvents()
		{
			Debug.Assert(_inSimHandler!=null);

			_inSimHandler.RaceTrackConnection		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackNewConnectionHandler(_inSimHandler_RaceTrackConnection);
			_inSimHandler.RaceTrackConnectionLeave	+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackConnectionLeaveHandler(_inSimHandler_RaceTrackConnectionLeave);
			_inSimHandler.RaceTrackPlayer			+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerHandler(_inSimHandler_RaceTrackPlayer);
			_inSimHandler.RaceTrackPlayerLeave		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerLeaveHandler(_inSimHandler_RaceTrackPlayerLeave);
			_inSimHandler.RaceTrackPlayerRename		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerRenameHandler(_inSimHandler_RaceTrackPlayerRename);
			_inSimHandler.RaceTrackCarTakeover		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackCarTakeoverHandler(_inSimHandler_RaceTrackCarTakeover);
			_inSimHandler.RaceTrackPlayerPits		+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerPitsHandler(_inSimHandler_RaceTrackPlayerPits);
		}

		private void UnbindEvents()
		{
			Debug.Assert(_inSimHandler!=null);

			_inSimHandler.RaceTrackConnection		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackNewConnectionHandler(_inSimHandler_RaceTrackConnection);
			_inSimHandler.RaceTrackConnectionLeave	-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackConnectionLeaveHandler(_inSimHandler_RaceTrackConnectionLeave);
			_inSimHandler.RaceTrackPlayer			-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerHandler(_inSimHandler_RaceTrackPlayer);
			_inSimHandler.RaceTrackPlayerLeave		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerLeaveHandler(_inSimHandler_RaceTrackPlayerLeave);
			_inSimHandler.RaceTrackPlayerRename		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerRenameHandler(_inSimHandler_RaceTrackPlayerRename);
			_inSimHandler.RaceTrackCarTakeover		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackCarTakeoverHandler(_inSimHandler_RaceTrackCarTakeover);
			_inSimHandler.RaceTrackPlayerPits		-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerPitsHandler(_inSimHandler_RaceTrackPlayerPits);
		}

		private void _inSimHandler_RaceTrackConnection(InSimHandler sender, RaceTrackConnection e)
		{
			if (e.ConnectionId==0 && String.IsNullOrEmpty(e.UserName) && e.IsRemote==false && e.IsAdmin==true) {
				return;
			}

			InSimDriver newDriver = new InSimDriver(e.UserName, e.PlayerName, e.ConnectionId);
			// Check if driver doesn't exist
			Debug.Assert(!_inactiveDrivers.ContainsKey(newDriver.ConnectionId));
			if (_inactiveDrivers.ContainsKey(newDriver.ConnectionId)) {
				_log.Error(String.Format("Driver already exists'{0}'", newDriver.LfsName));
				newDriver.Dispose();
				return;
			}

			_inactiveDrivers.Add(newDriver.ConnectionId, newDriver);
			GenerateListOfAllConnectedDriversOnServer();

			GetOrCreateCar(newDriver);
			SetDriverState(newDriver, DriverState.Connected);
		}

		private void _inSimHandler_RaceTrackConnectionLeave(InSimHandler sender, RaceTrackConnectionLeave e)
		{
			InSimDriver driver = GetDriverByConnectionIDInternal(e.ConnectionId);
			Debug.Assert(driver!=null, String.Format("Driver is leaving and he doesn't exist? ConectionId: '{0}'", e.ConnectionId));

			if (driver==null) {
				_log.Fatal(String.Format("Driver is leaving and he doesn't exist? ConectionId: '{0}'", e.ConnectionId));
				//throw new Exception(String.Format("Driver is leaving and he doesn't exist? ConectionId: '{0}'", e.ConnectionId));
				return;
			}

			// Wtf? Player is still active?
			Debug.Assert(driver.PlayerId==null, String.Format("Driver '{0}' is leaving and is still active?", driver.LfsName));
			if (driver.PlayerId!=null) {
				_log.Error(String.Format("Driver '{0}' is leaving and is still active?", driver.LfsName));
				DeactivateDriver(driver);
			}

			Debug.Assert(_inactiveDrivers.ContainsKey(driver.ConnectionId));
			if (_inactiveDrivers.ContainsKey(driver.ConnectionId)) {
				SetDriverState(driver, DriverState.Disconnecting);
				_inactiveDrivers.Remove(driver.ConnectionId);
			}

			if (!driver.InSimCar.IsEmpty) {
				RemoveCar(driver);
			}

			GenerateListOfAllConnectedDriversOnServer();
			driver.Dispose();
		}

		private void _inSimHandler_RaceTrackPlayer(InSimHandler sender, RaceTrackPlayer e)
		{
			if (e.AI) {
				foreach (IInSimDriverAi aiId in _aiDrivers) {
					if (aiId.PlayerId == e.PlayerId) {
						return;
					}
				}
				InSimDriverAi aiDriver = new InSimDriverAi(e.PlayerName, e.PlayerId);
				_aiDrivers.Add(aiDriver);
				OnAiDriverStateChanged(aiDriver, true);
				if (_logDebug) {
					_log.DebugFormat("AI driver added '{0}'", aiDriver.Name);
				}
				return;
			}

			if (_activeDrivers.ContainsKey(e.PlayerId)) {		// Driver can be active when he leaves the garage.

				InSimDriver activeDriver = _activeDrivers[e.PlayerId];
				Debug.Assert(activeDriver.PlayerId == e.PlayerId);

				Debug.Assert(!_inactiveDrivers.ContainsKey(activeDriver.ConnectionId));
				if (_inactiveDrivers.ContainsKey(activeDriver.ConnectionId)) {
					_log.Fatal(String.Format("Driver '{0}' can't active and inactive simultaneously!", _activeDrivers[e.PlayerId].LfsName));
					throw new Exception(String.Format("Driver '{0}' can't active and inactive simultaneously!", _activeDrivers[e.PlayerId].LfsName));
				}

				Debug.Assert(activeDriver.InSimCar.ActiveDriver == activeDriver);
				if (activeDriver.InSimCar.ActiveDriver != activeDriver) {
					_log.Fatal(String.Format("Car actual driver '{0}' is different from driver '{1}'!", activeDriver.InSimCar.ActiveDriver.LfsName, activeDriver.LfsName));
					throw new Exception(String.Format("Car actual driver '{0}' is different from driver '{1}'!", activeDriver.InSimCar.ActiveDriver.LfsName, activeDriver.LfsName));
				}

				SetCarState(activeDriver.InSimCar, CarState.OnTrack);

				CheckCarRestrictionsAndSendToSpectate(activeDriver, e);

				return;
			}

			Debug.Assert(_inactiveDrivers.ContainsKey(e.ConnectionId));
			if (!_inactiveDrivers.ContainsKey(e.ConnectionId)) {
				_log.Fatal(String.Format("Driver '{0}' wasn't found in inactive drivers!", InSimDriver.GetPlayerNameWithoutColors(e.PlayerName)));
				throw new Exception(String.Format("Driver '{0}' wasn't found in inactive drivers!", InSimDriver.GetPlayerNameWithoutColors(e.PlayerName)));
			}

			InSimDriver driver = _inactiveDrivers[e.ConnectionId];

			Debug.Assert(driver.State == DriverState.Spectating || driver.State==DriverState.Connected);
			ActivateDriver(driver, e.PlayerId, e);
		}

		private void _inSimHandler_RaceTrackPlayerLeave(InSimHandler sender, RaceTrackPlayerLeave e)
		{
			foreach(InSimDriverAi aiDriver in _aiDrivers) {
				if (aiDriver.PlayerId == e.PlayerId) {
					_aiDrivers.Remove(aiDriver);
					OnAiDriverStateChanged(aiDriver, false);

					if (_logDebug) {
						_log.DebugFormat("AI driver removed '{0}'", aiDriver.Name);
					}
					return;
				}
			}

			Debug.Assert(_activeDrivers.ContainsKey(e.PlayerId));
			if (!_activeDrivers.ContainsKey(e.PlayerId)) {
				_log.Fatal(String.Format("Driver playerId: '{0}' wasn't active!", e.PlayerId));
				//throw new Exception(String.Format("Driver playerId: '{0}' wasn't active!", e.PlayerId));
				return;
			}

			InSimDriver driver = _activeDrivers[e.PlayerId];

			Debug.Assert(!_inactiveDrivers.ContainsKey(driver.ConnectionId));
			if (_inactiveDrivers.ContainsKey(driver.ConnectionId)) {
				_log.Fatal(String.Format("Driver '{0}' is already inactivated!", driver.LfsName));
				throw new Exception(String.Format("Driver '{0}' is already inactivated!", driver.LfsName));
			}

			Debug.Assert(driver.ActivePlayerId == e.PlayerId);
			DeactivateDriver(driver);
		}

		private void _inSimHandler_RaceTrackCarTakeover(InSimHandler sender, RaceTrackCarTakeover e)
		{
			InSimDriver oldDriver = GetDriverByConnectionIDInternal(e.OldConnectionId);
			InSimDriver newDriver = GetDriverByConnectionIDInternal(e.NewConnectionId);

			Debug.Assert(oldDriver!=null, "Old driver wasn't found!");
			if (oldDriver==null) {
				_log.Fatal("Old driver wasn't found!");
				throw new Exception("Old driver wasn't found!");
			}

			Debug.Assert(newDriver!=null, "New driver wasn't found!");
			if (newDriver==null) {
				_log.Fatal("New driver wasn't found!");
				throw new Exception("New driver wasn't found!");
			}

			if (_logDebug) {
				_log.Debug(String.Format("Driver '{0}', try to take over a car from driver '{1}'", newDriver.LfsName, oldDriver.LfsName));
			}

			SwitchDriversInCar(oldDriver, newDriver, e.PlayerId);
		}

		private void _inSimHandler_RaceTrackPlayerRename(InSimHandler sender, RaceTrackPlayerRename e)
		{
			InSimDriver driver = GetDriverByConnectionIDInternal(e.ConnectionId);

			Debug.Assert(driver!=null);
			if (driver==null) {
				return;
			}

			driver.NickName = e.PlayerName;
		}

		private void _inSimHandler_RaceTrackPlayerPits(InSimHandler sender, RaceTrackPlayerPits e)
		{
			foreach(InSimDriverAi aiDriver in _aiDrivers) {
				if (aiDriver.PlayerId == e.PlayerId) {
					return;
				}
			}

			Debug.Assert(_activeDrivers.ContainsKey(e.PlayerId));
			if (!_activeDrivers.ContainsKey(e.PlayerId)) {
				_log.Fatal(String.Format("Driver with Id: '{0}' is in Pit, but isn't active!", e.PlayerId));
				throw new Exception(String.Format("Driver with Id: '{0}' is in Pit, but isn't active!", e.PlayerId));
			}

			InSimDriver driver = _activeDrivers[e.PlayerId];

			Debug.Assert(driver.InSimCar.State == CarState.OnTrack);
			SetCarState(driver.InSimCar, CarState.InGarage);
		}

		private InSimDriver GetDriverByConnectionIDInternal(Byte connectionId)
		{
			foreach(InSimDriver activeDriver in _activeDrivers.Values) {
				if (activeDriver.ConnectionId==connectionId) {
#if DEBUG
					if (_inactiveDrivers.ContainsKey(connectionId)) {
						Debug.Assert(false, String.Format("Driver '{0}' can't be active and inactive in the same time!", activeDriver.LfsName));
						_log.Error(String.Format("Driver '{0}' can't be active and inactive in the same time!", activeDriver.LfsName));
					}
#endif
					return activeDriver;
				}
			}

			if (_inactiveDrivers.ContainsKey(connectionId)) {
				return _inactiveDrivers[connectionId];
			}

			_log.DebugFormat("Driver with connection Id {0} doesn't exist!", connectionId);
			Debug.Assert(false, "Driver with connection Id doesn't exist!");
			return null;
		}

		protected void SetDriverState(InSimDriver driver, DriverState newDriverState)
		{
			Debug.Assert(driver!=null);

			DriverState oldState = driver.State;
			driver.State = newDriverState;

			if (_logDebug) {
				_log.DebugFormat("Driver '{0} state change from '{1} to '{2}", driver.LfsName, oldState, newDriverState);
			}

			if (DriverStateChanged!=null) {
				DriverStateChanged(this, new DriverStateEventArgs(driver, oldState, newDriverState));
			}
		}

		protected void SetCarState(InSimCar car, CarState newCarState)
		{
			Debug.Assert(car!=null);

			CarState oldState = car.State;
			car.State = newCarState;

			if (_logDebug) {
				_log.DebugFormat("Car number '{0} state change from '{1} to '{2}", car.Number, oldState, newCarState);
			}

			if (CarStateChanged!=null) {
				CarStateChanged(this, new CarStateChangedEventArgs(car, oldState, newCarState));
			}
		}

		protected void OnAiDriverStateChanged(IInSimDriverAi aiDriver, bool added)
		{
			Debug.Assert(aiDriver!=null);

			if (AiDriverStateChanged!=null) {
				AiDriverStateChanged(this, new AiDriverStateChangedEventArgs(aiDriver, _aiDrivers.Count, added));
			}
		}

		private void GenerateListOfAllConnectedDriversOnServer()
		{
			lock(this) {
				InSimDriver[] driversArray = new InSimDriver[_activeDrivers.Count + _inactiveDrivers.Count];
				if (_activeDrivers.Values.Count > 0 || _inactiveDrivers.Values.Count > 0) {
					_activeDrivers.Values.CopyTo(driversArray, 0);
					_inactiveDrivers.Values.CopyTo(driversArray, _activeDrivers.Values.Count);
					_allDrivers = Array.AsReadOnly<IInSimDriver>(driversArray);
				} else {
					_allDrivers = new List<IInSimDriver>(0);
				}
			}
		}

		protected void CheckCarRestrictionsAndSendToSpectate(InSimDriver driver, RaceTrackPlayer e)
		{
			
			List<String> reasons = new List<String>(0);
			if (!CheckCarRestrictions(driver.InSimCar, reasons, e)) {
				if (reasons != null) {
					foreach (String reason in reasons) {
						if (!String.IsNullOrEmpty(reason)) {
							_messaging.SendMessageToDriver(driver, reason);
						}
					}
				}
				_inSimHandler.SendMessage(LfsCommands.Spectate(driver));
			}
		}

		private void RequesConnectionAndPlayerInfo() {
			_inSimHandler.RequestConnectionInfo();
			_inSimHandler.RequestPlayerInfo();
		}

		// Buttons needs CarDriveMnaged, but there is cyclic dependancy. Inject it is ok. It C&D initialized later (after messaging)
		private void InjectCarDriverManIntoControlFactory() {
			if (_pluginManager == null) {
				return;
			}

			PluginControlFactory controlFactory = _pluginManager.GetPlugin(CmlPlugins.ControlFactoryGuid) as PluginControlFactory;
			Debug.Assert(controlFactory != null);
			if (controlFactory == null) {
				_log.Fatal("IControlFactory plugin wasn't found!");
				throw new ArgumentNullException("IControlFactory plugin wasn't found!");
			}

			controlFactory.CarAndDriverManager = this;
		}
	}
}
