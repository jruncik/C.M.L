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
	internal class CarDriverManagerExtended : CarDriverManagerBase
	{
		internal CarDriverManagerExtended(IPluginManager pluginManager) : base (pluginManager)
		{
			_log		= LogManager.GetLogger(typeof(CarDriverManagerExtended));
			_logDebug	= _log.IsDebugEnabled;
		}

		internal override void InitializeFromSetting()
		{
			InSimCar newCar = null;

			for (Int32 i = 0; i<1; ++i) {
				newCar = new InSimCar(i);
				newCar.AllowedDrivers.Add("babyonwheels");
				newCar.AllowedDrivers.Add("wilibaldjahoda");

				_cars.Add(newCar);
			}

		}

		protected override void GetOrCreateCar(InSimDriver driver)
		{
			InSimCar driversCar = null;

			foreach (InSimCar car in _cars) {
				if (car.IsDriverInCrew(driver.LfsName)) {
					if (driversCar == null) {
						driversCar = car;
					} else {
						Debug.Assert(false, String.Format("Driver '{0}' can drive only one car!", driver.LfsName));
						_log.Fatal(String.Format("Driver '{0}' can drive only one car!", driver.LfsName));
						throw new Exception(String.Format("Driver '{0}' can drive only one car!", driver.LfsName));
					}
				}
			}

			if (driversCar != null) {
				driversCar.AddDriver(driver);
				Debug.Assert(driver.InSimCar == driversCar);
			}
		}

		protected override void RemoveCar(InSimDriver driver)
		{
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

			List<String>	reasons			= new List<String>(0);
			bool			sendToSpectate	= false;
			InSimCar		car				= driver.InSimCar;

			if (car!=null) {
				if (car.ActiveDriver!=null) {
					if (car.ActiveDriver!=driver) {	// Other driver drives the car
						if (_logDebug) {
							_log.Debug(String.Format("Driver '{0}' can get in car. Other driver '{1}' uses it!", driver.LfsName, car.ActiveDriver.LfsName));
						}
						sendToSpectate = true;
						reasons.Add("Other driver uses car!");
					}
				}

				if (!sendToSpectate) {
					sendToSpectate = CheckCarRestrictions(driver.InSimCar, reasons, e);
				}

				if (!sendToSpectate) {
					car.ActivateDriver(driver);
				}
			} else {
				if (_logDebug) {
					_log.Debug(String.Format("Driver '{0}' isn't in any car!", driver.LfsName));
				}
				sendToSpectate = true;
				reasons.Add("There isn't car for you!");
			}

			Debug.Assert(driver.ActivePlayerId == playerId);
			Debug.Assert(!_activeDrivers.ContainsKey(driver.ActivePlayerId));
			if (_activeDrivers.ContainsKey(driver.ActivePlayerId)) {
				_log.Fatal(String.Format("Driver '{0}' is already activated!", driver.LfsName));
				throw new Exception(String.Format("Driver '{0}' is already activated!", driver.LfsName));
			}
			_activeDrivers.Add(driver.ActivePlayerId, driver);

			if (sendToSpectate) {
				if (reasons!=null) {
					foreach (String reason in reasons) {
						if (!String.IsNullOrEmpty(reason)) {
							_messaging.SendMessageToDriver(driver, reason);
						}
					}
				}
				_inSimHandler.SendMessage(LfsCommands.Spectate(driver));
			} else {
				SetDriverState(driver, DriverState.InCar);
				SetCarState(car, CarState.OnTrack);
			}
		}

		protected override void DeactivateDriver(InSimDriver driver)
		{
			Debug.Assert(_activeDrivers.ContainsKey(driver.ActivePlayerId));
			if (!_activeDrivers.ContainsKey(driver.ActivePlayerId)) {
				_log.Fatal(String.Format("Driver '{0}' can't be deactivated, he isn't active!", driver.LfsName));
				throw new Exception(String.Format("Driver '{0}' can't be deactivated, he isn't active!", driver.LfsName));
			}

			_activeDrivers.Remove(driver.ActivePlayerId);

			driver.Deactivate();

			InSimCar car = driver.InSimCar;

			if (car!=null) {
				if (car.ActiveDriver==driver) {
					Debug.Assert(car.State == CarState.OnTrack || car.State == CarState.InGarage);

					SetCarState(car, CarState.LeavingTrack);

					car.DeactivateDriver(driver);

					SetDriverState(driver, DriverState.Spectating);
				}
			}

			Debug.Assert(!_inactiveDrivers.ContainsKey(driver.ConnectionId));
			if (_inactiveDrivers.ContainsKey(driver.ConnectionId)) {
				_log.Fatal(String.Format("Driver '{0}' is already deactivated!", driver.LfsName));
				throw new Exception(String.Format("Driver '{0}' is already deactivated!", driver.LfsName));
			}
			_inactiveDrivers.Add(driver.ConnectionId, driver);
		}

		protected override void SwitchDriversInCar(InSimDriver oldDriver, InSimDriver newDriver, Byte newPlayerId)
		{
			InSimCar oldDriversCar = oldDriver.InSimCar;
			InSimCar newDriversCar = newDriver.InSimCar;

			DeactivateDriver(oldDriver);					// synchronize drivers active state with LFS
			SetDriverState(oldDriver, DriverState.Spectating);

			ActivateDriver(newDriver, newPlayerId, null);	// Doesn't't change the car. Let new driver in his origin car...
			SetDriverState(newDriver, DriverState.InCar);

			if (!oldDriversCar.IsDriverInCrew(newDriver.LfsName)) {
				if (_logDebug) {
					_log.Debug(String.Format("Driver '{0}' isn't team member of car '{1}!", newDriver.LfsName, oldDriversCar.Number));
				}

				_messaging.SendMessageToDriver(newDriver, "You are not team member");
				_inSimHandler.SendMessage(LfsCommands.Spectate(newDriver));
			}
		}

		protected override void DisposeDriversAndCars()
		{
			if (_activeDrivers != null) {
				foreach (InSimDriver activeDriver in _activeDrivers.Values) {
					Debug.Assert(activeDriver.PlayerId != null);

					SetCarState(activeDriver.Car as InSimCar, CarState.LeavingTrack);

					activeDriver.Deactivate();
					activeDriver.InSimCar.DeactivateDriver(activeDriver);
					SetDriverState(activeDriver, DriverState.Disconnecting);
					activeDriver.Dispose();
				}
				_activeDrivers.Clear();
				_activeDrivers = null;
			}

			if (_inactiveDrivers != null) {
				foreach (InSimDriver inactiveDriver in _inactiveDrivers.Values) {
					Debug.Assert(inactiveDriver.PlayerId == null);
					SetDriverState(inactiveDriver, DriverState.Disconnecting);
					inactiveDriver.Dispose();
				}
				_inactiveDrivers.Clear();
				_inactiveDrivers = null;
			}

			if (_cars != null) {
				foreach (InSimCar car in _cars) {
					Debug.Assert(car.ActiveDriver == null);
					Debug.Assert(car.State == CarState.LeavingTrack);
					car.Dispose();
				}
				_cars.Clear();
				_cars = null;
			}
		}

		protected override bool CheckCarRestrictions(InSimCar driverscar, List<String> reasons, RaceTrackPlayer e)
		{
			if (_carsAndDriversConfiguration == null) {
				return true;
			}

			return false;
		}
	}
}
