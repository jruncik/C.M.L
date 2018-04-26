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

using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class Results : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(Results));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool								_disposed				= false;
		private Dictionary<String, DriverResult>	_results				= null;
		private Rallycross							_rallycross				= null;

		internal Results(Rallycross rallycross)
		{
			Debug.Assert(rallycross!=null);
			if (rallycross==null) {
				_log.Fatal("Rallycross is null!");
				throw new ArgumentNullException("Rallycross is null!");
			}

			_rallycross	= rallycross;
			_results	= new Dictionary<String,DriverResult>();
		}

		~Results()
		{
			Debug.Assert(_disposed, "Results, call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!_disposed) {
				if (disposing) {
					if (_results!=null) {
						_results.Clear();
						_results = null;
					}
				}
				_disposed = true;
			}
		}

		internal void Initialize()
		{
			foreach(DriverInfo driverInfo in _rallycross.Drivers) {
				_results.Add(driverInfo.LfsUserName, new DriverResult(driverInfo.LfsUserName));
			}
		}

		internal void SetResult(Int32 heatIndex, String lfsDriverName, HeatResult heatResult)
		{
			if (String.IsNullOrEmpty(lfsDriverName)) {
				return;
			}

			Debug.Assert(_results.ContainsKey(lfsDriverName));
			if (!_results.ContainsKey(lfsDriverName)) {
				_log.FatalFormat("Results for driver '{0}' doesn't exists!", lfsDriverName);
				throw new ArgumentException(String.Format("Results for driver '{0}' doesn't exists!", lfsDriverName));
			}

			_results[lfsDriverName].SetHeatResult(heatResult, heatIndex);
		}

		internal void CalculateHeatResults(Int32 heatIndex)
		{
			List<HeatResult>	heatResults	= new List<HeatResult>(_results.Count);
			HeatResult			heatResult	= null;
			Int32				maxlapsDone = -1;

			foreach(DriverResult driverResult in _results.Values) {
				heatResult	= driverResult.GetHeatResult(heatIndex);
				maxlapsDone	= Math.Max(maxlapsDone, heatResult.Laps);
				heatResults.Add(heatResult);
			}

			heatResults.Sort();

			Int32 points			= heatResults.Count;
			Int32 minLapsForPoints	= (Int32)Math.Round((double)maxlapsDone * 0.75);

			foreach(HeatResult heatResultTmp in heatResults) {
				heatResultTmp.Points = 0;
				if (heatResultTmp.IsEmpty) {
					continue;
				}
				if (heatResultTmp.IsDnf && heatResultTmp.Laps<minLapsForPoints) {
					continue;
				}
				heatResultTmp.Points = points;
			}
		}

		internal IList<DriverResult> GetSortedResultsByPoints()
		{
			List<DriverResult> sortedResults = new List<DriverResult>(_rallycross.Drivers.Count);
			foreach(DriverResult resultDriver in _results.Values) {
				sortedResults.Add(resultDriver);
			}

			//sortedResults.Sort();

			return sortedResults;
		}
	}
}
