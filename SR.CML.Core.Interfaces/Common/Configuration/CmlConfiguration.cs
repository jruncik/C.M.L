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
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

using log4net;

using System.Diagnostics;

namespace SR.CML.Common
{
	[Serializable]
	public class CmlConfiguration
	{
		private static ILog _log = LogManager.GetLogger(typeof(CmlConfiguration));

		private Host _host;
		public Host Host {
			get { return _host; }
			set { _host = value; }
		}

		private EventConfiguration _eventConfiguration;
		public EventConfiguration EventConfiguration {
			get { return _eventConfiguration; }
			set { _eventConfiguration = value; }
		}

		private CarsAndDriversConfiguration _carsAndDriversConfiguration;
		public CarsAndDriversConfiguration CarsAndDriversConfiguration {
			get { return _carsAndDriversConfiguration; }
			set { _carsAndDriversConfiguration = value; }
		}

		private CzLeagueConfig _czLeagueConfig;
		public CzLeagueConfig CzLeagueConfig {
			get { return _czLeagueConfig; }
			set { _czLeagueConfig = value; }
		}

		public CmlConfiguration() {
			_host							= new Host();
			_czLeagueConfig					= new CzLeagueConfig();
			_eventConfiguration				= new EventConfiguration();
			_carsAndDriversConfiguration	= new CarsAndDriversConfiguration();
		}

		public static CmlConfiguration CreateFromFile(String fileName) {
			String xmlConfig = String.Empty;
			using (StreamReader reader = new StreamReader(fileName)) {
				xmlConfig = reader.ReadToEnd();
			}
			return CreateFromString(xmlConfig);
		}
		public static CmlConfiguration CreateFromString(String xmlConfig) {
			if (String.IsNullOrEmpty(xmlConfig)) {
				throw new ArgumentException("Xml configuration is empty");
			}

			return CmlConfiguration.Deserialize(xmlConfig);
		}

		public void SaveToFile(String fileName) {
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.Write(SaveToSring());
				writer.Flush();
			}
		}

		public String SaveToSring() {

			StringWriter	output	= new StringWriter(new StringBuilder());
			String			ret		= String.Empty;

			XmlSerializer serializer = new XmlSerializer(this.GetType());
			serializer.Serialize(output, this);

			return RemoveXmlDef(output.ToString());
		}

		private static String RemoveXmlDef(String ret) {
			ret = ret.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", String.Empty);
			ret = ret.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", String.Empty);
			ret = ret.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", String.Empty).Trim();
			return ret;
		}

		private static CmlConfiguration Deserialize(String xmlConfig) {

			try {
				XmlSerializer serializer = new XmlSerializer(typeof(CmlConfiguration));
				CmlConfiguration configuration = null;

				using (StringReader stringReader = new StringReader(xmlConfig)) {
					using (XmlTextReader xmlReader = new XmlTextReader(stringReader)) {
						configuration = serializer.Deserialize(xmlReader) as CmlConfiguration;
					}
				}

				configuration.BindCarTypesTocar(configuration.CarsAndDriversConfiguration);
				configuration.BindDriversToCar(configuration.CarsAndDriversConfiguration);

				return configuration;

			} catch (Exception ex) {
				_log.Fatal(ex);
				throw ex;
			}
		}

		private void BindCarTypesTocar(CarsAndDriversConfiguration carsAndDriversConfiguration) {
			Dictionary<Guid, CarType> carTypes = new Dictionary<Guid, CarType>();

			foreach (CarType carType in carsAndDriversConfiguration.CarTypes) {
				carTypes.Add(carType.Id, carType);
			}

			foreach (Car car in carsAndDriversConfiguration.Cars) {
				if (!carTypes.ContainsKey(car.CarTypeId)) {
					_log.ErrorFormat("CarType id '{0)' doesn't exist for car '{1}'", car.CarTypeId, car.Number);
				}

				car.CarType = carTypes[car.CarTypeId];
			}
		}

		private void BindDriversToCar(CarsAndDriversConfiguration carsAndDriversConfiguration) {
			foreach (Car car in carsAndDriversConfiguration.Cars) {
				foreach (Driver driver in car.Drivers) {
					driver.Car = car;
				}
			}
		}

	}
}
