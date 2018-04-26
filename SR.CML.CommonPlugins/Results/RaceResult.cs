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
	internal class RaceResult : IRaceResult, IComparable
	{
		private static ILog		_log		= LogManager.GetLogger(typeof(RaceResult));
		private static bool		_logDebug	= _log.IsDebugEnabled;

		private IInSimCar		_car;
		private ResultManager	_resultManager;
		private List<Int32>		_pits;
		private List<LapResult>	_lapResuls;
		private Byte			_finalPosition;
		private Int32			_bestLapIndex;
		private Int32			_currentLapIndex;
		private LapResult		_currentLap;
		private TimeSpan		_totalTime;
		private bool			_finished;
		private bool			_dnf;
		private String			_lfsPlayerName;

		internal RaceResult (ResultManager resultManager, IInSimCar car)
		{
			Debug.Assert(car!=null);
			//Debug.Assert(resultManager!=null);
	
			_car				= car;
			_resultManager		= resultManager;
			_pits				= new List<Int32>(1);
			_lapResuls			= new List<LapResult>(32);
			_finalPosition		= 0;
			_currentLapIndex	= 0;
			_bestLapIndex		= -1;
			_finished			= false;
			_dnf				= false;

			_log.DebugFormat("New RaceResult for car {0} created.", _car.Number);

			AddNewLap();
		}

		internal LapResult CurrentLap
		{
			get { return _currentLap; }
		}

		internal ResultManager ResultManager
		{
			get { return _resultManager; }
		}

		private static readonly IComparer<IRaceResult> _raceResultComparer = new RaceResultComparerInternal();
		public static IComparer<IRaceResult> RaceResultComparer {
			get { return RaceResult._raceResultComparer; }
		}

		private static readonly IComparer<IRaceResult> _qualifyResultComparer = new QualifyResultComparerInternal();
		public static IComparer<IRaceResult> QualifyResultComparer {
			get { return RaceResult._qualifyResultComparer; }
		} 

		#region IRaceResult
		public String LfsPlayerName {
			get { return _lfsPlayerName; }
		}

		public IInSimCar Car
		{
			get { return _car; }
		}

		public IList<Int32> Pits
		{
			get { return _pits.AsReadOnly(); }
		}
		
		public Byte Position
		{
			get { return _finalPosition; }
		}

		public ILapResult Bestlap
		{
			get {
				if (_bestLapIndex == -1 && _lapResuls.Count>0) {
					CalculateFullRaceResult();
				}

				if (_bestLapIndex < 0) {
					return LapResult.Empty;
				}

				return _lapResuls[_bestLapIndex];
			}
		}

		public IList<ILapResult> Laps
		{
			get { return _lapResuls.AsReadOnly() as IList<ILapResult>; }
		}

		public Int32 LapsCount
		{
			get { return _lapResuls.Count; }
		}

		public TimeSpan Time
		{
			get {
				if (_totalTime == TimeSpan.Zero && _lapResuls.Count > 0) {
					CalculateFullRaceResult();
				}
				return _totalTime;
			}
		}

		public bool Finished
		{
			get { return _finished; }
			internal set {
				if (value!=_finished) {
					_finished = value;

					if (_finished) {
						_log.DebugFormat("Car {0} finished race.", _car.Number);

						Debug.Assert(_lapResuls.Count>=1);
						if (_lapResuls.Count>=1) {
							LapResult lapRes = _lapResuls[_lapResuls.Count-1];
							Debug.Assert(lapRes.Time == TimeSpan.Zero);
							_lapResuls.RemoveAt(_lapResuls.Count-1);
						}
					}
				}
			}
		}

		public bool DNF
		{
			get { return _dnf; }
			internal set {
				if (!_finished) {
					return;
				}
				_dnf = value;
			}
		}
		#endregion

		#region IComparable
		public int CompareTo(object obj)
		{
			RaceResult other = obj as RaceResult;
			if (other == null) {
				throw new ArgumentException("object is not a RaceResult");
			}

			return _raceResultComparer.Compare(this, other);
		}
		#endregion

		private class RaceResultComparerInternal : IComparer<IRaceResult>
		{
			public int Compare(IRaceResult left, IRaceResult right) {
				if (!left.Finished || !right.Finished) {
					throw new ArgumentException("RaceResult has to be finished!");
				}

				if (left.LapsCount == 0 && left.Time == TimeSpan.Zero) {
					return 1;
				}
				if (right.LapsCount == 0 && right.Time == TimeSpan.Zero) {
					return -1;
				}

				if (left.LapsCount < right.LapsCount) {
					return 1;
				}
				if (left.LapsCount > right.LapsCount) {
					return -1;
				}

				if (left.Time > right.Time) {
					return 1;
				}
				if (left.Time < right.Time) {
					return -1;
				}

				Debug.Assert(left.Time == right.Time && left.LapsCount == right.LapsCount);
				return 0;
			}
		}

		private class QualifyResultComparerInternal : IComparer<IRaceResult>
		{
			public int Compare(IRaceResult left, IRaceResult right) {
				if (!left.Finished || !right.Finished) {
					throw new ArgumentException("RaceResult has to be finished!");
				}

				if (left.LapsCount == 0 && left.Bestlap.Time == TimeSpan.Zero) {
					return 1;
				}
				if (right.LapsCount == 0 && right.Bestlap.Time == TimeSpan.Zero) {
					return -1;
				}

				if (left.Bestlap.Time < right.Bestlap.Time) {
					return -1;
				}
				if (left.Bestlap.Time > right.Bestlap.Time) {
					return 1;
				}

				Debug.Assert(left.Bestlap.Time == right.Bestlap.Time && left.LapsCount == right.LapsCount);
				return 0;
			}
		}

		internal LapResult AddNewLap()
		{
			if (_car.ActiveDriver!=null) {
				_lfsPlayerName = _car.ActiveDriver.LfsName;
			}

			if (_finished) {
				_log.ErrorFormat("AddNewLap: Car {0} finished race!", _car.Number);
				Debug.Assert(false);
				return null;
			}
			_totalTime = TimeSpan.Zero;
			_currentLap = new LapResult(this, _currentLapIndex++);
			_lapResuls.Add(_currentLap);

			_log.DebugFormat("New lap for car {0} added.", _car.Number);

			return _currentLap;
		}

		internal void SetlapTimeAfterCrash(TimeSpan lapTime)
		{
			if (_finished) {
				return;
			}
			throw new NotImplementedException();
		}

		private void CalculateFullRaceResult() {
			TimeSpan bestLapTime = TimeSpan.MaxValue;
			Int32 index = 0;

			_totalTime = TimeSpan.Zero;
			_bestLapIndex = -1;
			_pits.Clear();

			foreach (LapResult lapResult in _lapResuls) {
				if (lapResult.Pitted) {
					_pits.Add(index);
				}

				if (bestLapTime > lapResult.Time) {
					bestLapTime = lapResult.Time;
					_bestLapIndex = index;
				}

				_totalTime += lapResult.Time;
				++index;
			}

			if (_bestLapIndex > -1) {
				_log.DebugFormat("Race results for car {0} calculated. TotalTime {1}, BestLapIndex {2}, BestLapTime {3}, LapsCount {4}", _car.Number, _totalTime, _bestLapIndex, _lapResuls[_bestLapIndex].Time, _lapResuls.Count);
			} else {
				_log.DebugFormat("Race results for car {0} calculated. TotalTime {1}, LapsCount {4}", _car.Number, _totalTime, _lapResuls.Count);
			}
		}
	}
}
