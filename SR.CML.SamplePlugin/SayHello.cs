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

namespace SR.CML.SamplePlugin
{
	[Plugin("58FD59FF-5B43-4f4b-94F1-7D1801E8815D",
			"SayHello", "Display information about split times",
			new String[5]{"536550BD-BE60-4fd2-91B7-098FCF43F5B1", "F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3",
						  "AFACAC8A-B63D-4006-940B-4C31999ECE37", "6301C0A4-8FFE-4baf-9569-07753A268AAC",
						  "27253220-5A44-4d8f-A1D6-D52A0FF060FB"})]

	public class SayHello : IPlugin
	{
		private bool			_disposed			= false;
		private PluginState		_state				= PluginState.Undefined;
		private IPluginManager	_pluginManager		= null;
		private IResultManager	_resultManager		= null;
		private IRaceDirector	_raceDirector		= null;

		public SayHello()
		{
		}

		~SayHello()
		{
			Debug.Assert(_disposed, "SayHello, call dispose before the application is closed!");
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
				}
				_disposed = true;
			}
		}

		#region IPlugin

		public PluginState State
		{
			get { return _state; }
		}

		public void Initialize(IPluginManager pluginManager)
		{
			Debug.Assert(_state == PluginState.Undefined);
			Debug.Assert(pluginManager != null);

			_pluginManager = pluginManager;

			_raceDirector = _pluginManager.GetPlugin(CmlPlugins.RaceDirectorGuid) as IRaceDirector;
			Debug.Assert(_raceDirector!=null);

			_resultManager = _pluginManager.GetPlugin(CmlPlugins.ResultManagerGuid) as IResultManager;
			Debug.Assert(_resultManager!=null);

			_state = PluginState.Initialized;
		}

		public void Activate()
		{
			if (_state == PluginState.Activated) {
				return;
			}

			_resultManager.AllCarsFinished	+= new EventHandler<EventArgs>(ResultManager_AllCarsFinished);
			_resultManager.CarFinished		+= new EventHandler<CarFinishedEventArgs>(ResultManager_CarFinished);

			_raceDirector.StateChanged		+= new EventHandler<RaceStateChangedEventArgs>(RaceDirector_StateChanged);

			_state = PluginState.Activated;
		}

		public void Deactivate()
		{
			_resultManager.AllCarsFinished	-= new EventHandler<EventArgs>(ResultManager_AllCarsFinished);
			_resultManager.CarFinished		-= new EventHandler<CarFinishedEventArgs>(ResultManager_CarFinished);

			_raceDirector.StateChanged		-= new EventHandler<RaceStateChangedEventArgs>(RaceDirector_StateChanged);

			_state = PluginState.Deactivated;
		}

		#endregion

		private void ResultManager_CarFinished(object sender, CarFinishedEventArgs e)
		{
			Debug.WriteLine(String.Format("ResultManager: Car '{0}' finished the race", e.InSimCar.Number));
		}

		private void ResultManager_AllCarsFinished(object sender, EventArgs e)
		{
			Debug.WriteLine("ResultManager: All car finished the race");
		}

		private void RaceDirector_StateChanged(object sender, RaceStateChangedEventArgs e)
		{
			Debug.WriteLine(String.Format("RaceDirector: StateChanged - Old: '{0}', New: {1}", e.OldRaceSate, e.NewRaceSate));
		}
	}
}
