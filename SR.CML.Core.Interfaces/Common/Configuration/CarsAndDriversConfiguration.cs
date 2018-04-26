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
using System.Xml.Serialization;
using System.Collections.Generic;


namespace SR.CML.Common
{
	[Serializable]
	[XmlInclude(typeof(List<CarType>))]
	public class CarsAndDriversConfiguration
	{
		private List<Car> _cars;
		[XmlArray("Cars")]
		[XmlArrayItem("Car", typeof(Car))]
		public List<Car> Cars {
			get { return _cars; }
			set { _cars = value; }
		}

		private List<CarType> _carTypes;
		[XmlArray("CarTypes")]
		[XmlArrayItem("CarType", typeof(CarType))]
		public List<CarType> CarTypes {
			get { return _carTypes; }
			set { _carTypes = value; }
		}

		public CarsAndDriversConfiguration() {
			_cars		= new List<Car>();
			_carTypes	= new List<CarType>();
		}

		public void AddCar(Car car) {
			if (_cars.Contains(car)) {
				return;
			}
			_cars.Add(car);
			AddCarType(car.CarType);
		}

		public void RemoveCar(Car car) {
			if (!_cars.Contains(car)) {
				return;
			}
			_cars.Remove(car);
			RemoveCarType(car.CarType);
		}

		public void AddCarType(CarType carType) {
			if (_carTypes.Contains(carType)) {
				return;
			}
			_carTypes.Add(carType);
		}

		public void RemoveCarType(CarType carType) {
			if (!_carTypes.Contains(carType)) {
				return;
			}

			foreach (Car car in _cars) {
				if (car.CarType == carType) {
					return;
				}
			}
			_carTypes.Remove(carType);
		}
	}
}
