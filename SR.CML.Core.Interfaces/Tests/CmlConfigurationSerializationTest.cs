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
using System.Xml.Serialization;
using System.Collections.Generic;
using NUnit.Framework;

using SR.CML.Common;

using System.Diagnostics;

namespace SR.CML.Core.Interfaces.Tests
{
	[TestFixture]
	public class CmlConfigurationSerializationTest
	{
		private CmlConfiguration	_config		= new CmlConfiguration();
		private static String		FILE_NAME	= @"test.cmlcfg";

		[SetUp]
		public void InitTest()
		{
			DeleteTestFile();

			_config = new CmlConfiguration();

			_config.Host.AdminisratorPassword	= "adminPassword";
			_config.Host.InSimPort				= 29999;
			_config.Host.IP						= "127.0.0.1";

			_config.EventConfiguration.EventType		= EventType.Race;
			_config.EventConfiguration.Laps				= 25;
			_config.EventConfiguration.ServerPassword	= "serverPassword";
			_config.EventConfiguration.Track			= LfsTrack.BL1;
			_config.EventConfiguration.Wind				= LfsWind.Off;
			_config.EventConfiguration.Wheather			= LfsWheather.Day;

			CarType ufrBaby = new CarType("Baby UFR", LfsCarType.UFR);
			ufrBaby.CarRestriction.HandicapIntakeRestriction = 24;

			CarType xfrBaby = new CarType("Baby XFR", LfsCarType.XFR);
			xfrBaby.CarRestriction.HandicapIntakeRestriction = 22;

			_config.CarsAndDriversConfiguration.AddCarType(ufrBaby);
			_config.CarsAndDriversConfiguration.AddCarType(xfrBaby);

			GenerateDrivers(ufrBaby, "UFR", 1);

			GenerateDrivers(xfrBaby, "XFR", 20);

		}

		[TearDown]
		public void Cleanup()
		{
			_config = null;
			DeleteTestFile();
		}

		[Test]
		public void SerializeToFile() {
			_config.SaveToFile(FILE_NAME);
			Assert.True(File.Exists(FILE_NAME));
		}

		[Test]
		public void SerializeAndDeserializeFromFile() {
			_config.SaveToFile(FILE_NAME);
			CmlConfiguration configDeserialized = CmlConfiguration.CreateFromFile(FILE_NAME);

			Assert.AreEqual(_config, configDeserialized);
		}


		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
		public void DeserializeFromNonExistingFile() {
			CmlConfiguration configDeserialized = CmlConfiguration.CreateFromFile("xxx_doesnt_exist_xxx.cmlcfg");
		}

		private void GenerateDrivers(CarType ufrBaby, String driverPrefix, Int32 carInitialNumber) {
			for (Int32 i = 0; i < 10; ++i) {
				Car car = new Car();
				car.CarType = ufrBaby;
				car.Number = i;

				Driver driver = new Driver();
				driver.LfsName = String.Format("{0}_Driver_{1}", driverPrefix, i + carInitialNumber);

				car.AddDriver(driver);

				_config.CarsAndDriversConfiguration.AddCar(car);
			}
		}

		private static void DeleteTestFile() {
			if (File.Exists(FILE_NAME)) {
				File.Delete(FILE_NAME);
			}
		}
	}
}
