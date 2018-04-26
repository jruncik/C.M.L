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

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class HeatResult : IComparable
	{
		private static HeatResult _empty;
		internal static HeatResult Empty
		{
			get
			{
				if (_empty==null) {
					_empty = CreateDNFHeatResult();
					_empty.IsEmpty = true;
				}
				return _empty;
			}
		}

		private bool _dnf;
		public bool IsDnf
		{
			get { return _dnf; }
		}

		private TimeSpan _time;
		internal TimeSpan Time
		{
			get { return _time; }
		}

		private Int32 _laps;
		internal Int32 Laps
		{
			get { return _laps; }
		}

		private Int32 _points;
		internal Int32 Points
		{
			get { return _points; }
			set { _points = value; }
		}

		private bool _isEmpty;
		internal bool IsEmpty
		{
			get { return _isEmpty; }
			private set { _isEmpty = value; }
		}

		private HeatResult(TimeSpan time, Int32 laps, bool dnf)
		{
			_time		= time;
			_points		= 0;
			_laps		= laps;
			_dnf		= dnf;
			_isEmpty	= false;
		}

		internal static HeatResult CreateHeatResult(TimeSpan time, Int32 laps, bool dnf)
		{
			return new HeatResult(time, laps, dnf);
		}

		internal static HeatResult CreateDNFHeatResult()
		{
			return new HeatResult(TimeSpan.Zero, 0, true);
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			HeatResult other = obj as HeatResult;
			if (other == null) {
				throw new ArgumentException("object is not a HeatResult");
			}

			if (_isEmpty && !other._isEmpty) {
				return 1;
			}

			if (other._isEmpty && !_isEmpty) {
				return -1;
			}

			if (_dnf && !other._dnf) {
				return 1;
			}

			if (other._dnf && !_dnf) {
				return -1;
			}

			if (_laps > other._laps) {
				return -1;
			}

			if (_laps < other._laps) {
				return 1;
			}

			if (_time > other._time) {
				return 1;
			}

			if (_time < other._time) {
				return -1;
			}

			Debug.Assert (_time == other._time && _laps == other._laps);
			return 0;
		}

		#endregion
	}
}
