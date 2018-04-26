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
using FullMotion.LiveForSpeed.InSim.Events;

using System.Diagnostics;

namespace SR.CML.ReactionTester
{
	internal class ObstacleInfo : IDisposable
	{
		internal event EventHandler<ObstacleStateEventArgs> StateChanged;

		private const Int32 shift = 5;

		private bool			_disposed = false;
		private double			_tolerance = 0.3;
		private ICMLCore		_cmlCore;
		private IInSimDriver	_obstacleDriver;
		private Vector			_position;
		private Vector			_lastUpdatedPosition;
		private IButton			_button;
		private bool			_started;
		private	ObstacleState	_state;

		internal ObstacleInfo(ICMLCore cmlCore, double tolerance)
		{
			Debug.Assert(cmlCore!=null);

			_cmlCore				= cmlCore;
			_obstacleDriver			= null;
			_position				= null;
			_lastUpdatedPosition	= null;
			_state					= ObstacleState.Undefined;
			_tolerance				= tolerance;

			_cmlCore.CarDriverManager.DriverStateChanged += new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
			_cmlCore.InSimHandler.RaceTrackMultiCarInfo += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackMultiCarInfoHandler(InSimHandler_RaceTrackMultiCarInfo);
		}

		~ObstacleInfo()
		{
			Debug.Assert(_disposed, "ObstacleInfo, call dispose before the application is closed!");
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
					_cmlCore.CarDriverManager.DriverStateChanged -= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
					_cmlCore.InSimHandler.RaceTrackMultiCarInfo -= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackMultiCarInfoHandler(InSimHandler_RaceTrackMultiCarInfo);
					if (_button	!=null) {
						_button.Delete();
						_button = null;
					}
				}
				_disposed = true;
			}
		}

		private void CarDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e)
		{
			if (e.Driver.NickName.ToLower() == "obstacle") {

				if (_obstacleDriver==null) {
					_obstacleDriver = e.Driver;
				}

				switch (e.NewState) {
					case DriverState.OnTrack : {
						Debug.Assert(_button==null);
						_button = _cmlCore.ControlFactory.CreateButton(e.Driver);
						_button.Left	= 80;
						_button.Top		= 10;
						_button.Width	= 40;
						_button.Height	= 14;
						_button.Color	= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;
						_button.Text	= "^2Start";

						_button.Click += new EventHandler<ButtonClickEventArgs>(_button_Click);
						_button.Show();
					} break;

					case DriverState.Connected : {
					} break;

					default : {
						if (_button!=null) {
							_button.Delete();
							_button = null;
						}

						if (_obstacleDriver!=null && e.Driver.LfsName==_obstacleDriver.LfsName) {
							_obstacleDriver = null;
						}
					} break;
				}
			}
		}

		private	void InSimHandler_RaceTrackMultiCarInfo(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackMultiCarInfo e)
		{
			if (_obstacleDriver==null) {
				_position				= null;
				_lastUpdatedPosition	= null;
				_started				= false;
				return;
			}

			Vector location = null;
			double	x = 0.0;
			double	y = 0.0;
			double	distance = 0.0;

			foreach(CarInfo carInfo in e.CarInfo) {

				location = carInfo.Location;

				if (carInfo.PlayerId == _obstacleDriver.PlayerId) {

					if (_position==null) {
						_lastUpdatedPosition	= location;
						_started				= false;
					} else {
						x = location.X - _position.X;
						y = location.Y - _position.Y;

						distance = Math.Sqrt(x*x + y*y) / 65536.0;

						if (!_started && (distance>=_tolerance)) {
							_button.Show();
							_button.Text = "^3Wating for break";
							_started = true;
							OnStateChanged(ObstacleState.Moved);						
						}
					}
				}
			}
		}

		internal void TestDriveFinishedBreaking()
		{
			OnStateChanged(ObstacleState.FinishedBreaking);
			_button.Show();
			_button.Text = "^3Activate next Run";
		}

		private void _button_Click(object sender, ButtonClickEventArgs e)
		{
			if (_position==null) {
				_position = _lastUpdatedPosition;
 				_button.Hide();
				OnStateChanged(ObstacleState.Ready);
			} else {
				if (_state == ObstacleState.FinishedBreaking) {
					OnStateChanged(ObstacleState.NextRun);
				}
				_position	= null;
				_started	= false;
				_button.Show();
				_button.Text = "^2Ready";
			}
		}

		private void OnStateChanged(ObstacleState state)
		{
			if (StateChanged!=null) {
				StateChanged(this, new ObstacleStateEventArgs(state, _state));
			}
			_state = state;
		}

	}
}
