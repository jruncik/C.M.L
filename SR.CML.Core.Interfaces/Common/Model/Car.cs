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
using System.Xml.Serialization;
using System.Diagnostics;

namespace SR.CML.Common
{
	[Serializable]
	public class Car
	{
		private CarType _carType;
		[XmlIgnore]
		public CarType CarType {
			get { return _carType; }
			set {
				_carType = value;
				_carTypeId = _carType.Id;
			}
		}

		private Guid _carTypeId = Guid.Empty;
		public Guid CarTypeId {
			get { return _carTypeId; }
			set { _carTypeId = value; }
		}

		private List<Driver> _drivers;
		[XmlArray("Drivers")]
		[XmlArrayItem("Driver", typeof(Driver))]
		public List<Driver> Drivers {
			get { return _drivers; }
			set { _drivers = value; }
		}

		private Int32 _number;
		public Int32 Number {
			get { return _number; }
			set { _number = value; }
		}

		public Car()
		{
			_drivers = new List<Driver>(1);
		}

		public void AddDriver(Driver driver) {
			if (_drivers.Contains(driver)) {
				return;
			}

			_drivers.Add(driver);
			driver.Car = this;
		}

		public void RemoveDriver(Driver driver) {
			if (!_drivers.Contains(driver)) {
				return;
			}

			_drivers.Remove(driver);
			driver.Car = null;
		}

	}
}
