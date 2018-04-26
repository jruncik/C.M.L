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

using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;

using System.Diagnostics;

namespace SR.CML.ReactionTester
{
	internal class TestedDriverInfo
	{
		internal event EventHandler<BreakEventArgs> Breaking;

		private bool			_disposed = false;
		private ICMLCore		_cmlCore;
		private bool			_breaking = false;

		internal TestedDriverInfo(ICMLCore cmlCore)
		{
			Debug.Assert(cmlCore!=null);

			_cmlCore = cmlCore;
			_cmlCore.InSimHandler.GaugeUpdated += new FullMotion.LiveForSpeed.OutGauge.OutGaugeHandler.GaugeEvent(InSimHandler_GaugeUpdated);
		}

		~TestedDriverInfo()
		{
			Debug.Assert(_disposed, "TestedDriverInfo, call dispose before the application is closed!");
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
					_cmlCore.InSimHandler.GaugeUpdated -= new FullMotion.LiveForSpeed.OutGauge.OutGaugeHandler.GaugeEvent(InSimHandler_GaugeUpdated);
				}
				_disposed = true;
			}
		}

		protected void InSimHandler_GaugeUpdated(object sender, FullMotion.LiveForSpeed.OutGauge.Events.Gauge gauge)
		{
			if (gauge.Brake>0!=_breaking) {
				_breaking = gauge.Brake>0;
			
				if (Breaking!=null) {
					Breaking(this, new BreakEventArgs(_breaking));
				}
			}
		}
	}
}
