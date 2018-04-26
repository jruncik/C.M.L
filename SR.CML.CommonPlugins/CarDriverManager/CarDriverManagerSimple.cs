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

using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Events;

using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.CarDriverManager
{
	internal class CarDriverManagerSimple : CarDriverManagerBase
	{
		private static Int32 _carsCount = 1;

		internal CarDriverManagerSimple(IPluginManager pluginManager) : base(pluginManager)
		{
			_log		= LogManager.GetLogger(typeof(CarDriverManagerSimple));
			_logDebug	= _log.IsDebugEnabled;
		}

		internal override void InitializeFromSetting()
		{
			Debug.Assert(_cars.Count==0);
		}

		protected override void GetOrCreateCar(InSimDriver driver)
		{
			InSimCar newCar = new InSimCar(_carsCount++);

			// Check if car with driver doesn't exist
			foreach (InSimCar car in _cars) {
				if (car.ContainsDriver(driver.LfsName)) {
					Debug.Assert(false, String.Format("Car with player '{0}' already exist!", driver.LfsName));
					_log.Fatal(String.Format("Car with player '{0}' already exist!", driver.LfsName));
					throw new Exception(String.Format("Car with player '{0}' already exist!", driver.LfsName));
				}
			}

			newCar.AddDriver(driver);
			Debug.Assert(driver.InSimCar == newCar);

			_cars.Add(newCar);
		}

		protected override void RemoveCar(InSimDriver driver)
		{
			InSimCar car = driver.InSimCar;
			Debug.Assert(car!=null);
			if (driver.InSimCar.IsEmpty) {
				_log.Fatal(String.Format("Driver '{0}' without car!", driver.LfsName));
				throw new Exception(String.Format("Driver '{0}' without car!", driver.LfsName));
			}

			if(car!=null) {
				car.RemoveDriver(driver);

				if (car.Drivers.Count==0) {
					_cars.Remove(car);
					car.Dispose();
				}
			} else {
				_log.Error("null car can't be removed!");
			}
		}

		protected override void ActivateDriver(InSimDriver driver, Byte playerId, RaceTrackPlayer e)
		{
			Debug.Assert(_inactiveDrivers.ContainsKey(driver.ConnectionId));
			if (!_inactiveDrivers.ContainsKey(driver.ConnectionId)) {
				_log.Fatal(String.Format("Driver '{0}' can't be activated, he isn't in inactive drivers!", driver.LfsName));
				throw new Exception(String.Format("Driver '{0}' can't be activated, he isn't in inactive drivers!", driver.LfsName));
			}

			_inactiveDrivers.Remove(driver.ConnectionId);

			driver.Activate(playerId);

			InSimCar car = driver.InSimCar;

			car.ActivateDriver(driver);

			Debug.Assert(driver.ActivePlayerId == playerId);
			Debug.Assert(!_activeDrivers.ContainsKey(driver.ActivePlayerId));
			if (_activeDrivers.ContainsKey(driver.ActivePlayerId)) {
				_log.Fatal(String.Format("Driver '{0}' is already activated!", driver.LfsName));
				throw new Exception(String.Format("Driver '{0}' is already activated!", driver.LfsName));
			}
			_activeDrivers.Add(driver.ActivePlayerId, driver);

			SetDriverState(driver, DriverState.InCar);
			SetCarState(car, CarState.OnTrack);

			CheckCarRestrictionsAndSendToSpectate(driver, e);
		}

		protected override void DeactivateDriver(InSimDriver driver)
		{
			Debug.Assert(_activeDrivers.ContainsKey(driver.ActivePlayerId));
			if (!_activeDrivers.ContainsKey(driver.ActivePlayerId)) {
				_log.Fatal(String.Format("Driver '{0}' can't be deactivated, he isn't active!", driver.LfsName));
				throw new Exception(String.Format("Driver '{0}' can't be deactivated, he isn't active!", driver.LfsName));
			}

			InSimCar car = driver.InSimCar;
			Debug.Assert(car.State == CarState.OnTrack || car.State == CarState.InGarage);

			_activeDrivers.Remove(driver.ActivePlayerId);

			SetCarState(car, CarState.LeavingTrack);

			driver.Deactivate();
			car.DeactivateDriver(driver);

			SetDriverState(driver, DriverState.Spectating);

			Debug.Assert(!_inactiveDrivers.ContainsKey(driver.ConnectionId));
			if (_inactiveDrivers.ContainsKey(driver.ConnectionId)) {
				_log.Fatal(String.Format("Driver '{0}' is already deactivated!", driver.LfsName));
				throw new Exception(String.Format("Driver '{0}' is already deactivated!", driver.LfsName));
			}
			_inactiveDrivers.Add(driver.ConnectionId, driver);
		}

		protected override void SwitchDriversInCar(InSimDriver oldDriver, InSimDriver newDriver, Byte newPlayerId)
		{
			DeactivateDriver(oldDriver);			// synchronize drivers active state with LFS
			SetDriverState(oldDriver, DriverState.Spectating);

			ActivateDriver(newDriver, newPlayerId, null);	// Doesn't change the car. Let new driver in his origin car...
			SetDriverState(newDriver, DriverState.InCar);

			_messaging.SendMessageToDriver(newDriver, "You can't take other car!");
			_inSimHandler.SendMessage(LfsCommands.Spectate(newDriver));			// In this mode switching the drivers is prohibited!
		}

		protected override void DisposeDriversAndCars()
		{
			if (_activeDrivers != null) {
				foreach (InSimDriver activeDriver in _activeDrivers.Values) {
					Debug.Assert(activeDriver.PlayerId != null);
					if (!activeDriver.InSimCar.IsEmpty) {
						if (_cars != null) {
							Debug.Assert(_cars.Contains(activeDriver.InSimCar));
							activeDriver.Deactivate();
							activeDriver.InSimCar.DeactivateDriver(activeDriver);
							_cars.Remove(activeDriver.InSimCar);
							activeDriver.InSimCar.Dispose();
						}
					}
					SetDriverState(activeDriver, DriverState.Disconnecting);
					activeDriver.Dispose();
				}
				_activeDrivers.Clear();
				_activeDrivers = null;
			}

			if (_inactiveDrivers != null) {
				foreach (InSimDriver inactiveDriver in _inactiveDrivers.Values) {
					Debug.Assert(inactiveDriver.PlayerId == null);
					if (!inactiveDriver.InSimCar.IsEmpty) {
						if (_cars != null) {
							Debug.Assert(_cars.Contains(inactiveDriver.InSimCar));
							_cars.Remove(inactiveDriver.InSimCar);
							inactiveDriver.InSimCar.Dispose();
						}
					}
					SetDriverState(inactiveDriver, DriverState.Disconnecting);
					inactiveDriver.Dispose();
				}
				_inactiveDrivers.Clear();
				_inactiveDrivers = null;
			}

			if (_cars != null) {
				Debug.Assert(_cars.Count==0);
				foreach (InSimCar car in _cars) {
					Debug.Assert(car.ActiveDriver == null);
					SetCarState(car, CarState.LeavingTrack);
					car.Dispose();
				}
				_cars.Clear();
				_cars = null;
			}
		}

		protected override bool CheckCarRestrictions(InSimCar driverscar, List<String> reasons, RaceTrackPlayer e)
		{
			//if (_carsAndDriversConfiguration==null) {
			//    return true;
			//}

			//if (e != null) {
			//    if (e.AbsEnabled) {
			//        reasons.Add("ABS isn't alowed");
			//        return false;
			//    }

			//    String carName = e.CarName.ToUpper();

			//    if (carName == "UFR") {
			//        if (e.HandicapIntakeRestriction != (Decimal)0.24) {
			//            reasons.Add("Please set intake restriction to 24%.");
			//            return false;
			//        }
			//    }

			//    if (carName == "XFR") {
			//        if (e.HandicapIntakeRestriction != (Decimal)0.22) {
			//            reasons.Add("Please set intake restriction to 22%.");
			//            return false;
			//        }
			//    }
			//}
			return true;
		}
	}
}
