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
using System.Timers;
using System.Collections.Generic;

using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Enums;

using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.Controls
{
	class CountDown : ICountDown
	{
		private static ILog _log = LogManager.GetLogger(typeof(CountDown));
		private static bool _logDebug = _log.IsDebugEnabled;

		private bool _disposed = false;
		private ControlFactory _controlFactory = null;
		private ILabel _backgroundLabel = null;
		private ILabel _foregrounglabel = null;
		private Timer _timer = null;
		private Int32 _ticks = 30;
		private Int32 _countDownCounter = 30;
		protected Object _tag = null;

		internal CountDown(ControlFactory controlFactory)
		{
			Debug.Assert(controlFactory != null);
			if (controlFactory == null) {
				_log.Fatal("ControlFactory isn't set!");
				throw new ArgumentNullException("ControlFactory isn't set!");
			}

			_controlFactory = controlFactory;

			_backgroundLabel = _controlFactory.CreateLabel();
			_foregrounglabel = _controlFactory.CreateLabel();

			_backgroundLabel.Color = ButtonColor.Transparent;
			_backgroundLabel.TextAlignment = ButtonTextAlignment.Center;
			_backgroundLabel.Text = "^0•";

			_foregrounglabel.Color = ButtonColor.Transparent;
			_foregrounglabel.TextAlignment = ButtonTextAlignment.Center;
			_foregrounglabel.Text = String.Format("^7{0}", _ticks.ToString());

			_timer = new Timer(1000.0);
			_timer.Enabled = false;
			_timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
		}

		~CountDown()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "CountDown, call dispose before the application is closed!");
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

					if (Disposing != null) {
						Disposing(this, new EventArgs());
					}

					if (_timer != null) {
						_timer.Stop();
						_timer.Elapsed -= new ElapsedEventHandler(_timer_Elapsed);
						_timer.Dispose();
						_timer = null;
					}

					if (_backgroundLabel != null) {
						_backgroundLabel.Delete();
						_backgroundLabel = null;
					}

					if (_foregrounglabel != null) {
						_foregrounglabel.Delete();
						_foregrounglabel = null;
					}
				}
				_disposed = true;
			}
		}

		#region IControl

		public event EventHandler<EventArgs> Disposing;

		public bool Visible
		{
			get { return _foregrounglabel.Visible; }
			set
			{
				if (value != _foregrounglabel.Visible) {
					_foregrounglabel.Visible = value;
					_backgroundLabel.Visible = value;
				}
			}
		}

		public Byte Left
		{
			get { return _foregrounglabel.Left; }
			set
			{
				_foregrounglabel.Left = value;
			}
		}

		public Byte Right
		{
			get { return _foregrounglabel.Right; }
			set { _foregrounglabel.Right = value; }
		}

		public Byte Top
		{
			get { return _foregrounglabel.Top; }
			set { _foregrounglabel.Top = value; }
		}

		public Byte Bottom
		{
			get { return _foregrounglabel.Bottom; }
			set { _foregrounglabel.Bottom = value; }
		}

		public Byte Width
		{
			get { return _foregrounglabel.Width; }
			set
			{
				_foregrounglabel.Width = value;
				_backgroundLabel.Width = (Byte)(4 * value);
				_backgroundLabel.Left = (Byte)((_foregrounglabel.Left + (_foregrounglabel.Width / 2)) - _backgroundLabel.Width / 2);
			}
		}

		public Byte Height
		{
			get { return _foregrounglabel.Height; }
			set
			{
				_foregrounglabel.Height = value;
				_backgroundLabel.Height = (Byte)(4 * value);
				_backgroundLabel.Top = (Byte)((_foregrounglabel.Top + (_foregrounglabel.Height / 2)) - _backgroundLabel.Height / 2 - (_backgroundLabel.Height * 0.0375));
			}
		}

		public ButtonColor Color
		{
			get { return ButtonColor.Transparent; }
			set { }
		}

		public ButtonTextColor TextColor
		{
			get { return ButtonTextColor.LightGrey; }
			set { }
		}

		public ButtonTextAlignment TextAlignment
		{
			get { return ButtonTextAlignment.Center; }
			set { }
		}

		public Object Tag {
			get { return _tag; }
			set { _tag = value; }
		}

		public bool IsEmpty
		{
			get { return false; }
		}

		public void Show()
		{
			_backgroundLabel.Show();
			_foregrounglabel.Show();
		}

		public void Hide()
		{
			_backgroundLabel.Hide();
			_foregrounglabel.Hide();
		}

		public void Delete()
		{
			if (_disposed) {
				return;
			}

			if (_logDebug) {
				_log.Debug("Deleting CountDown");
			}
			Hide();
			Dispose();
		}

		#endregion

		#region ICountDown
		public event EventHandler<EventArgs> Elapsed;

		public Int32 Ticks
		{
			get { return _ticks; }
			set
			{
				_timer.Stop();
				_ticks = value;
			}
		}

		public Byte BackgroundColor
		{
			get { return 0; }
			set { }
		}

		public Byte ForegroundColor
		{
			get { return 0; }
			set { }
		}

		public void Start()
		{
			_timer.Enabled = true;
			_timer.Start();

			_countDownCounter = _ticks;
			_foregrounglabel.Text = String.Format("^7{0}", _countDownCounter.ToString());

			if (_logDebug) {
				_log.Debug("Countdown started");
			}
			Show();
		}

		public Boolean Active {
			get {
				return _timer.Enabled;
			}
		}

		#endregion

		private void Stop()
		{
			_timer.Stop();
			_timer.Enabled = false;

			if (_logDebug) {
				_log.Debug("Countdown stopped");
			}
		}

		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			--_countDownCounter;

			if (_logDebug) {
				_log.DebugFormat("Countdown tick '{0}'", _countDownCounter);
			}

			if (_countDownCounter == 0) {
				Stop();
				Hide();
				_countDownCounter = _ticks;

				if (Elapsed != null) {
					Elapsed(this, new EventArgs());
				}
				return;
			}

			if (_countDownCounter > 15) {
				_foregrounglabel.Text = String.Format("^7{0}", _countDownCounter.ToString());
			} else if (_countDownCounter > 5) {
				_foregrounglabel.Text = String.Format("^3{0}", _countDownCounter.ToString());
			} else {
				_foregrounglabel.Text = String.Format("^1{0}", _countDownCounter.ToString());
			}
		}

	}
}
