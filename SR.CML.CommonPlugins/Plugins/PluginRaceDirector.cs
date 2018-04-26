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
	[Plugin("AFACAC8A-B63D-4006-940B-4C31999ECE37",
		"RaceDirector", "Race director",
		new String[4]{	"536550BD-BE60-4fd2-91B7-098FCF43F5B1",		// CMLCoreGuid
						"863E4F77-94E9-43fb-8A46-323220837A0D",		// MessagingGuid
						"F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3",		// CarDriverManagerGuid
						"6301C0A4-8FFE-4baf-9569-07753A268AAC"})]	// ResultManagerGuid

	internal class PluginRaceDirector : PluginBase, IRaceDirector
	{
		private RaceDirector _raceDiretor;

		public PluginRaceDirector()
		{
			_log		= LogManager.GetLogger(typeof(PluginRaceDirector));
			_logDebug	= _log.IsDebugEnabled;
			_raceDiretor	= null;
		}

		protected override void InitializeInternal()
		{
			if (_raceDiretor==null) {
				_raceDiretor = new RaceDirector(_pluginManager);
			}
		}

		protected override void ActivateInternal()
		{
			_raceDiretor.Activate();
			_raceDiretor.StateChanged += new EventHandler<RaceStateChangedEventArgs>(_raceDiretor_StateChanged);
		}

		protected override void DeactivateInternal()
		{
			_raceDiretor.StateChanged -= new EventHandler<RaceStateChangedEventArgs>(_raceDiretor_StateChanged);
			_raceDiretor.Deactivate();
		}

		protected override void DisposeInternal()
		{
			if (_raceDiretor!=null) {
				_raceDiretor.Dispose();
				_raceDiretor = null;
			}
		}

		#region IRaceDirector Members

		public event EventHandler<RaceStateChangedEventArgs> StateChanged;

		public new RaceState State
		{
			get { return _raceDiretor.State; }
		}

		public void SetGrid(string[] driversOrder, bool fillEmptySpaceByUnusedDrivers)
		{
			_raceDiretor.SetGrid(driversOrder, fillEmptySpaceByUnusedDrivers);
		}

		public void StartRace()
		{
			_raceDiretor.StartRace();
		}

		public void StartRace(int laps)
		{
			_raceDiretor.StartRace(laps);
		}

		public void StartRace(TimeSpan time)
		{
			_raceDiretor.StartRace(time);
		}

		public void StartQualify(int minuts)
		{
			_raceDiretor.StartQualify(minuts);
		}

		public void StartPractice()
		{
			_raceDiretor.StartPractice();
		}

		#endregion

		private void _raceDiretor_StateChanged(object sender, RaceStateChangedEventArgs e)
		{
			if (StateChanged!=null) {
				StateChanged(sender, e);
			}
		}
	}
}
