/* ------------------------------------------------------------------------ *
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

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class DriverResult
	{
		private static ILog _log		= LogManager.GetLogger(typeof(DriverResult));
		private static bool _logDebug	= _log.IsDebugEnabled;

		//private ICarDriverManager	_carDriverManager;
		private Int32				_heatsCount;

		private String _lfsName;
		internal String Lfsname
		{
			get { return _lfsName; }
		}

		private List<HeatResult> _heatResults;
		internal IList<HeatResult> HeatResults
		{
			get { return _heatResults.AsReadOnly(); }
		}

		internal DriverResult(String lfsName)
		{
			Debug.Assert(!String.IsNullOrEmpty(lfsName));
			if (String.IsNullOrEmpty(lfsName)) {
				_log.Fatal("LfsName is null or empty");
			}

			_lfsName			= lfsName;
			_heatsCount			= 4 + 2;		// Heats 1, 2, 3, 4 + Final A, B
			_heatResults		= new List<HeatResult>(_heatsCount);
			for (Int32 i=0; i<_heatsCount; ++i) {
				_heatResults.Add(HeatResult.Empty);
			}
		}

		internal void SetHeatResult(HeatResult heatResult, Int32 heatIndex)
		{
			Debug.Assert(heatResult!=null);
			if (heatResult==null) {
				_log.Fatal("Heat result is null");
				return;
			}

			Debug.Assert(heatIndex>=0 && heatIndex<_heatResults.Count);
			if (heatIndex<0 || heatIndex>=_heatResults.Count) {
				_log.Fatal("GetHeatResult - index out of range");
				return;
			}

			_heatResults[heatIndex] = heatResult;
		}

		internal HeatResult GetHeatResult(Int32 heatIndex)
		{
			Debug.Assert(heatIndex>=0 && heatIndex<_heatResults.Count);
			if (heatIndex<0 || heatIndex>=_heatResults.Count) {
				_log.Fatal("GetHeatResult - index out of range");
				return HeatResult.Empty;
			}

			return _heatResults[heatIndex];
		}

	}
}
