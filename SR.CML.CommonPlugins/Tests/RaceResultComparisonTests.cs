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

using SR.CML.Core.InSimCommon.Controls;
using SR.CML.CommonPlugins.CarDriverManager;
using SR.CML.CommonPlugins.Results;

using NUnit.Framework;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.Tests
{
	[TestFixture]
	public class RaceResultComparisonTests
	{
		private List<IRaceResult> _results;
		private RaceResult first;
		private RaceResult second;
		private RaceResult third;
		private RaceResult fourth;
		private RaceResult fifth;

		InSimCar _car1 = null;
		InSimCar _car2 = null;
		InSimCar _car3 = null;
		InSimCar _car4 = null;
		InSimCar _car5 = null;


		[SetUp]
		public void InitTest() {
			_results = new List<IRaceResult>();

			_car1 = new InSimCar(1);
			_car2 = new InSimCar(2);
			_car3 = new InSimCar(3);
			_car4 = new InSimCar(4);
			_car5 = new InSimCar(5);

			first	= new RaceResult(null, _car1);
			second	= new RaceResult(null, _car2);
			third	= new RaceResult(null, _car3);
			fourth	= new RaceResult(null, _car4);
			fifth	= new RaceResult(null, _car5);

			first.CurrentLap.Time	= new TimeSpan(0, 1, 10);
			second.CurrentLap.Time	= new TimeSpan(0, 1, 5);
			third.CurrentLap.Time	= new TimeSpan(0, 0, 40);
			fourth.CurrentLap.Time	= new TimeSpan(0, 0, 50);

			first.AddNewLap();
			second.AddNewLap();

			first.CurrentLap.Time	= new TimeSpan(0, 1, 15);
			second.CurrentLap.Time	= new TimeSpan(0, 1, 30);

			// crossed final line ...
			first.AddNewLap();
			second.AddNewLap();
			third.AddNewLap();
			fourth.AddNewLap();

			first.Finished	= true;
			second.Finished	= true;
			third.Finished	= true;
			fourth.Finished	= true;
			fifth.Finished	= true;

			_results.Add(second);
			_results.Add(third);
			_results.Add(fifth);
			_results.Add(fourth);
			_results.Add(first);
		}

		[TearDown]
		public void Cleanup() {
			_car1.Dispose();
			_car2.Dispose();
			_car3.Dispose();
			_car4.Dispose();
			_car5.Dispose();

			_results.Clear();
			_results = null;
		}

		[Test]
		public void TestSort() {

			_results.Sort();

			Assert.AreEqual(_results[0], first);
			Assert.AreEqual(_results[1], second);
			Assert.AreEqual(_results[2], third);
			Assert.AreEqual(_results[3], fourth);
			Assert.AreEqual(_results[4], fifth);
		}

		[Test]
		public void TestSortRaceResult() {

			_results.Sort(RaceResult.RaceResultComparer);

			Assert.AreEqual(_results[0], first);
			Assert.AreEqual(_results[1], second);
			Assert.AreEqual(_results[2], third);
			Assert.AreEqual(_results[3], fourth);
			Assert.AreEqual(_results[4], fifth);
		}

		[Test]
		public void TestSortQualifyResult() {

			_results.Sort(RaceResult.QualifyResultComparer);

			Assert.AreEqual(_results[0], third);
			Assert.AreEqual(_results[1], fourth);
			Assert.AreEqual(_results[2], first);
			Assert.AreEqual(_results[3], second);
			Assert.AreEqual(_results[4], fifth);
		}
	}
}
