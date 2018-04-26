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

using SR.CML.Core.InSimCommon;

using FullMotion.LiveForSpeed.InSim;
using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.CarDriverManager
{
	internal class InSimCar : IInSimCar
	{
		private static readonly InSimCar _empty = new InSimCarEmpty();
		private class InSimCarEmpty : InSimCar
		{
			~InSimCarEmpty() {
			}

			public override IInSimDriver ActiveDriver
			{
				get { return InSimDriver.Empty; }
			}

			public override bool IsEmpty
			{
				get { return true; }
			}

			internal override void AddDriver(InSimDriver driver)
			{
				throw new InvalidOperationException("Empty car doesn't offer any functionality!");
			}

			internal override void RemoveDriver(InSimDriver driver)
			{
				throw new InvalidOperationException("Empty car doesn't offer any functionality!");
			}

			internal override void ActivateDriver(InSimDriver driver)
			{
				throw new InvalidOperationException("Empty car doesn't offer any functionality!");
			}

			internal override void DeactivateDriver(InSimDriver driver)
			{
				throw new InvalidOperationException("Empty car doesn't offer any functionality!");
			}

			internal override bool ContainsDriver(String lfsName)
			{
				return false;
			}

			internal override bool IsDriverInCrew(String lfsName)
			{
				return false;
			}
		}

		private static ILog		_log		= LogManager.GetLogger(typeof(InSimCar));
		private static bool		_logDebug	= _log.IsDebugEnabled;

		private bool				_disposed		= false;
		private IInSimDriver		_actualDriver	= InSimDriver.Empty;	// active driver
		private List<IInSimDriver>	_drivers		= null;					// car crew
		private Int32				_number			= 0;
		private CarState			_state			= CarState.Undefined;

		private List<String> _allowedDrivers = null;		// allowed drivers in car crew
		internal List<String> AllowedDrivers
		{
			get { return _allowedDrivers; }
		}

		public static InSimCar Empty
		{
			get { return _empty; }
		}

		private InSimCar()
		{
			_number = -1;
		}

		internal InSimCar(Int32 number)
		{
			_number			= number;
			_drivers		= new List<IInSimDriver>(0);
			_allowedDrivers	= new List<String>(0);

			if (_logDebug) {
				_log.Debug(String.Format("New car '{0}'created", _number));
			}
		}

		~InSimCar()
		{
			if (!IsEmpty) {
				if (_logDebug) {
					_log.Debug("Wasn't disposed before the application was closed!");
				}
				Debug.Assert(_disposed, "InSimCar, call dispose before the application is closed!");
			}
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
					Debug.Assert(_actualDriver.IsEmpty);
					if (!_actualDriver.IsEmpty) {
						_log.Error(String.Format("Disposing car number '{0}' with active driver '{1}'", _number, _actualDriver.LfsName));
					}

					foreach (InSimDriver driver in _drivers) {
						Debug.Assert(driver.InSimCar==this || driver.InSimCar.IsEmpty);
						if (driver.InSimCar!=this && !driver.InSimCar.IsEmpty) {
							_log.Error(String.Format("Car number '{0}' contains driver '{1}' which is activated in other car number '{2}' ", _number, driver.LfsName, driver.InSimCar.Number));
						}
						driver.InSimCar = InSimCar.Empty;
					}

					if (_logDebug) {
						_log.Debug(String.Format("Disposed car number '{0}'", _number));
					}
				}
				_disposed = true;
			}
		}

		#region IInSimCar

		public Int32 Number
		{
			get { return _number; }
		}

		public virtual IInSimDriver ActiveDriver
		{
			get { return _actualDriver; }
		}

		public IList<IInSimDriver> Drivers
		{
			get { return _drivers.AsReadOnly(); }
		}

		public CarState State {
			get { return _state; }
			internal set {_state = value; }
		}

		public virtual bool IsEmpty
		{
			get { return false; }
		}
		#endregion

		internal virtual void AddDriver(InSimDriver driver)
		{
			Debug.Assert(driver!=null);
			Debug.Assert(driver.InSimCar.IsEmpty);

			if (!_drivers.Contains(driver)) {
				_drivers.Add(driver);
			}

			driver.InSimCar = this;

			if (_logDebug) {
				_log.Debug(String.Format("Driver: '{0}' added in car '{1}' crew", driver.LfsName, _number));
			}
		}

		internal virtual void RemoveDriver(InSimDriver driver)
		{
			Debug.Assert(driver!=null);
			Debug.Assert(driver.InSimCar==this);

			Debug.Assert(_drivers.Contains(driver));
			if (_drivers.Contains(driver)) {
				_drivers.Remove(driver);
			} else {
				_log.Debug(String.Format("Removing driver: '{0}' from car '{1}' which isn't crew!", driver.LfsName, _number));
			}

			driver.InSimCar = InSimCar.Empty;

			if (_logDebug) {
				_log.Debug(String.Format("Driver: '{0}' removed from car '{1}' crew", driver.LfsName, _number));
			}
		}

		internal virtual void ActivateDriver(InSimDriver driver)
		{
			Debug.Assert(_actualDriver.IsEmpty);
			if (!_actualDriver.IsEmpty) {
				_log.Error(String.Format("Other driver '{0}' is activated. Driver '{1}' can't be activated", _actualDriver.LfsName, driver.LfsName));
				return;
			}

			Debug.Assert(_drivers.Contains(driver));
			Debug.Assert(driver.InSimCar == this);
			Debug.Assert(driver.PlayerId != null);

			_actualDriver = driver;

			if (_logDebug) {
				_log.Debug(String.Format("Driver: '{0}' get in car number '{1}'", _actualDriver.LfsName, _number));
			}
		}

		internal virtual void DeactivateDriver(InSimDriver driver)
		{
			Debug.Assert(driver.InSimCar == this);
			if (_actualDriver.IsEmpty) {
				Debug.Assert(false);
				if (_logDebug) {
					_log.Debug(String.Format("Car number '{1}' is without activeDriver. Driver '{1}' can't be deactivated", _number, driver.LfsName));
				}
				return;
			}

			Debug.Assert(_drivers.Contains(driver));
			Debug.Assert(_actualDriver==driver);
			Debug.Assert(driver.PlayerId == null);

			if (_logDebug) {
				_log.Debug(String.Format("Driver '{0}' get out from car number '{1}'", _actualDriver.LfsName, _number));
			}
			_actualDriver = InSimDriver.Empty;
		}

		internal virtual bool ContainsDriver(String lfsName)
		{
			lfsName = lfsName.ToLower();
			foreach (InSimDriver driver in _drivers) {
				if (driver.LfsName == lfsName) {
					return true;
				}
			}

			return false;
		}

		internal virtual bool IsDriverInCrew(String lfsName)
		{
			lfsName = lfsName.ToLower();
			foreach (String driver in _allowedDrivers) {
				if (driver == lfsName) {
					return true;
				}
			}

			return false;
		}
	}
}
