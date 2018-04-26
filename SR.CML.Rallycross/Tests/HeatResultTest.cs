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


using NUnit.Framework;

namespace SR.CML.Rallycross.Tests
{
	[TestFixture]
	public class HeatResultTest
	{
		private List<HeatResult> _heatResults;

		[SetUp]
		public void InitTest()
		{
			_heatResults = new List<HeatResult>();

			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(2, 45, 0), 7, false));
			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(0, 25, 0), 9, true));
			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(1, 0, 0), 10, false));
			_heatResults.Add(HeatResult.Empty);
			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(0, 50, 0), 10, false));
			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(0, 30, 0), 8, false));
			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(0, 30, 0), 10, true));
			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(1, 15, 0), 10, false));
			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(0, 25, 0), 10, true));
			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(1, 15, 0), 10, false));
			_heatResults.Add(HeatResult.CreateHeatResult(new TimeSpan(1, 25, 0), 9, false));
		}

		[TearDown]
		public void Cleanup()
		{
			_heatResults.Clear();
			_heatResults = null;
		}

		[Test]
		public void ComparisonTest()
		{
			SortResultsAndCalculatePositions();

			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(0, 50, 0), 10, false), 0));
			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(1, 0, 0), 10, false), 1));
			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(1, 15, 0), 10, false), 2));
			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(1, 15, 0), 10, false), 3));
			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(1, 25, 0), 9, false), 4));
			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(0, 30, 0), 8, false), 5));
			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(2, 45, 0), 7, false), 6));
			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(0, 25, 0), 10, true), 7));
			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(0, 30, 0), 10, true), 8));
			Assert.True(CheckHeatResult(HeatResult.CreateHeatResult(new TimeSpan(0, 25, 0), 9, true), 9));
			Assert.True(CheckHeatResult(HeatResult.Empty, 10));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ComparisonTestException()
		{
			HeatResult heatResult = HeatResult.CreateHeatResult(new TimeSpan(0, 50, 0), 10, false);
			heatResult.CompareTo(String.Empty);
		}

		#region Helpers

		private void SortResultsAndCalculatePositions()
		{
			_heatResults.Sort();
			Int32 position = 1;
			foreach (HeatResult heatResult in _heatResults) {
				heatResult.Points = position++;
			}
		}

		private bool CheckHeatResult(HeatResult result, Int32 index)
		{
			bool cmpResult = true;
			HeatResult tmpResult = _heatResults[index];

			cmpResult &= tmpResult.Laps		== result.Laps;
			cmpResult &= tmpResult.Points	== index + 1;
			cmpResult &= tmpResult.Time		== result.Time;
			cmpResult &= tmpResult.IsDnf		== result.IsDnf;
			cmpResult &= tmpResult.IsEmpty	== result.IsEmpty;

			return cmpResult;
		}
		#endregion
	}
}
