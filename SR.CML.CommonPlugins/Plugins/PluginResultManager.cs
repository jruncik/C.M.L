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

using SR.CML.CommonPlugins.Controls;
using SR.CML.CommonPlugins.CarDriverManager;
using SR.CML.CommonPlugins.Results;

using FullMotion.LiveForSpeed.InSim;
using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.Plugins
{
	[Plugin("6301C0A4-8FFE-4baf-9569-07753A268AAC",
		"RaceDirector", "Race director",
		new String[3]{	"536550BD-BE60-4fd2-91B7-098FCF43F5B1",		// CMLCoreGuid
						"863E4F77-94E9-43fb-8A46-323220837A0D",		// MessagingGuid
						"F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3"})]	// CarDriverManagerGuid

	internal class PluginResultManager : PluginBase, IResultManager
	{
		private ResultManager _resultManager;

		public PluginResultManager()
		{
			_log			= LogManager.GetLogger(typeof(PluginResultManager));
			_logDebug		= _log.IsDebugEnabled;
			_resultManager	= null;
		}

		protected override void InitializeInternal()
		{
			if (_resultManager==null) {
				_resultManager = new ResultManager(_pluginManager);
			}
		}

		protected override void ActivateInternal()
		{
			if (_resultManager.Activate()) {
				_resultManager.AllCarsFinished	+= new EventHandler<EventArgs>(_resultManager_AllCarsFinished);
				_resultManager.CarFinished		+= new EventHandler<CarFinishedEventArgs>(_resultManager_CarFinished);
			}
		}

		protected override void DeactivateInternal()
		{
			_resultManager.AllCarsFinished	-= new EventHandler<EventArgs>(_resultManager_AllCarsFinished);
			_resultManager.CarFinished		-= new EventHandler<CarFinishedEventArgs>(_resultManager_CarFinished);
			_resultManager.Deactivate();
		}

		protected override void DisposeInternal()
		{
			if (_resultManager!=null) {
				_resultManager.Dispose();
				_resultManager = null;
			}
		}

		#region IResultManager Members

		public event EventHandler<EventArgs> AllCarsFinished;

		public event EventHandler<CarFinishedEventArgs> CarFinished;

		public IRaceResult GetResult(IInSimCar car)
		{
			return _resultManager.GetResult(car);
		}

		public IList<IRaceResult> GetSortedRaceResults()
		{
			return _resultManager.GetSortedRaceResults();
		}

		public IList<IRaceResult> GetSortedQualifyResults()
		{
			return _resultManager.GetSortedQualifyResults();
		}

		public void ClearResults()
		{
			_resultManager.ClearResults();
		}

		#endregion

		private void _resultManager_CarFinished(object sender, CarFinishedEventArgs e)
		{
			if (CarFinished!=null) {
				CarFinished(sender, e);
			}
		}

		private void _resultManager_AllCarsFinished(object sender, EventArgs e)
		{
			if (AllCarsFinished!=null) {
				AllCarsFinished(sender, e);
			}
		}
	}
}
