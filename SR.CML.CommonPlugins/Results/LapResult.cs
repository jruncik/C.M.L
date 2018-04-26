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

using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.Results
{
	internal class LapResult : ILapResult
	{
		private static ILog		_log		= LogManager.GetLogger(typeof(LapResult));
		private static bool		_logDebug	= _log.IsDebugEnabled;

		private static readonly LapResult _empty = new LapResult();
		internal static LapResult Empty {
			get { return LapResult._empty; }
		}

		private RaceResult		_raceResult;
		private Int32			_lap;
		private List<TimeSpan>	_splits = new List<TimeSpan>(0);
		private TimeSpan		_lapTime;

		private LapResult()
		{
			_lap		= -1;
			_lapTime	= TimeSpan.Zero;
		}

		internal LapResult(RaceResult raceResult, Int32 lap)
		{
			Debug.Assert(raceResult!=null);
			_raceResult	= raceResult;
			_lap		= lap;
			_lapTime	= TimeSpan.Zero;
			if (_raceResult.ResultManager != null) { // Only for NUnit Test can be null.
				_splits = new List<TimeSpan>(_raceResult.ResultManager.SplitsCount);
				for (Byte i = 0; i < _raceResult.ResultManager.SplitsCount; ++i) {
					_splits.Add(TimeSpan.Zero);
				}
			}
			_log.DebugFormat("New lap result for car {0} created. Lap {1}", _raceResult.Car.Number, _lap);
		}

		#region ILapResult
		public bool Pitted
		{
			get { return _raceResult.Pits.Contains(_lap); }
		}
		
		public Int32 Lap
		{
			get { return _lap; }
		}

		public IList<TimeSpan> Splits
		{
			get { return _splits.AsReadOnly() as IList<TimeSpan>; }
		}

		public TimeSpan Time
		{
			get { return _lapTime; }
			internal set {
				if (_raceResult.Finished) {
					return;
				}
				_lapTime = value;
				_log.DebugFormat("lap time for car {0} added. Lap {1}, Time {2}", _raceResult.Car.Number, _lap, _lapTime);
			}
		}
		#endregion

		internal void SetSplit(Int32 splitIndex, TimeSpan splitTime)
		{
			if (_raceResult.Finished) {
				return;
			}

			if (_splits.Count<splitIndex) {
				_log.ErrorFormat("Split index out of range for car {0} added. Index {1}, Splittime {2}", _raceResult.Car.Number, splitIndex, splitTime);
				return;
			}
			_splits[splitIndex-1] = splitTime;
			_log.DebugFormat("Split index for car {0} added. Index {1}, Splittime {2}", _raceResult.Car.Number, splitIndex, splitTime);
		}

		internal void SetSplitAfterCrash(Int32 splitIndex, TimeSpan splitTime)
		{
			if (_raceResult.Finished) {
				return;
			}

			if (_splits.Count<splitIndex) {
				return;
			}
			_splits[splitIndex-1] = splitTime;
		}

	}
}
