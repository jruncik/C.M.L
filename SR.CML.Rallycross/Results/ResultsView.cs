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
using System.Text;
using System.Collections.Generic;

using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class ResultsView
	{
		private static ILog _log		= LogManager.GetLogger(typeof(ResultsView));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private Rallycross _rallycross;

		internal ResultsView(Rallycross rallycross) {
			Debug.Assert(rallycross!=null);
			if (rallycross==null) {
				_log.Fatal("CML core is null");
			}

			_rallycross = rallycross;
		}

		internal void ShowResults(IList<DriverResult> results)
		{
			Debug.Assert(results!=null);
			if (results==null) {
				_log.Fatal("Results are null");
				return;
			}

			StringBuilder driverResultRow = new StringBuilder();

			foreach(DriverResult driverResult in results) {
				driverResultRow.Length = 0;

				driverResultRow.Append(driverResult.Lfsname);
				driverResultRow.Append(" - ");

				bool firstResult = true;
				foreach(HeatResult heatResult in driverResult.HeatResults) {
					if (firstResult) {
						firstResult = false;
						driverResultRow.Append("{Time: ");
					} else {
						driverResultRow.Append(", {Time: ");
					}

					driverResultRow.Append(heatResult.Time);
					driverResultRow.Append(", Laps: ");
					driverResultRow.Append(heatResult.Laps);
					driverResultRow.Append(", Points: ");
					driverResultRow.Append(heatResult.Points);
					driverResultRow.Append(", DNF: ");
					driverResultRow.Append(heatResult.IsDnf);
					driverResultRow.Append(", Empty: ");
					driverResultRow.Append(heatResult.IsEmpty);
					driverResultRow.Append("}");
				}

				Debug.WriteLine(driverResultRow.ToString());
				_log.Debug(driverResultRow.ToString());
			}
		}
	}
}
