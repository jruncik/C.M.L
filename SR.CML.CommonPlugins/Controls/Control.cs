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
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Enums;

using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.Controls
{
	internal abstract class Control : IControl
	{
		protected static ILog	_log		= null;
		protected static bool	_logDebug	= false;

		protected bool				_disposed		= false;
		protected LfsButton			_lfsButton		= null;
		protected InSimHandler		_inSimHandler	= null;
		protected ControlFactory	_controlFactory	= null;
		protected bool				_displayed		= false;
		protected bool				_visible		= true;
		protected Object			_tag			= null;

		protected Byte _connectionId = 0;
		internal Byte ConnectionId
		{
			get { return _connectionId; }
		}

		protected Control()
		{
			_controlFactory = null;
		}

		internal Control(ControlFactory controlFactory, Byte buttonId, Byte connectionId)
		{
			Debug.Assert(controlFactory!=null);
			if (controlFactory==null) {
				_log.Fatal("ControlFactory isn't set!");
				throw new ArgumentNullException("ControlFactory isn't set!");
			}

			_lfsButton		= new LfsButton(buttonId);
			_controlFactory	= controlFactory;
			_inSimHandler	= _controlFactory.InSimHandler;
			_connectionId	= connectionId;
			_displayed		= false;
			_visible		= true;
		}

		~Control()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "Control, call dispose before the application is closed!");
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

					if (_lfsButton!=null) {
						if (_displayed) {
							Hide();
						}
						if (_logDebug) {
							_log.Debug(String.Format("{0} with Id: '{1}', connectionId: '{2}' disposed", _lfsButton.ButtonType.ToString(), _lfsButton.ButtonId, _connectionId));
						}
						_lfsButton = null;
					}
				}
				_disposed = true;
			}
		}

		#region IControl

		public event EventHandler<EventArgs> Disposing;

		public virtual bool Visible
		{
			get { return _visible; }
			set
			{
				if (_visible!=value) {

					if (_visible) {
						if (_displayed) {
							Hide();
						}
					}

					_visible = value;
				}
			}
		}

		public virtual Byte Left
		{
			get { return _lfsButton.Left; }
			set { _lfsButton.Left = value; }
		}

		public virtual Byte Right
		{
			get
			{
				Debug.Assert((_lfsButton.Left + _lfsButton.Width)<=255);
				return (Byte)(_lfsButton.Left + _lfsButton.Width);
			}

			set
			{
				Debug.Assert((value - _lfsButton.Left)>=0);
				_lfsButton.Width = (Byte)(value - _lfsButton.Left);
			}
		}

		public virtual Byte Top
		{
			get { return _lfsButton.Top; }
			set { _lfsButton.Top = value; }
		}

		public virtual Byte Bottom
		{
			get
			{
				Debug.Assert((_lfsButton.Top + _lfsButton.Height)<=255);
				return (Byte)(_lfsButton.Top + _lfsButton.Height);
			}

			set
			{
				Debug.Assert((value - _lfsButton.Top)>=0);
				_lfsButton.Top = (Byte)(value - _lfsButton.Top);
			}
		}

		public virtual Byte Width
		{
			get { return _lfsButton.Width; }
			set { _lfsButton.Width = value; }
		}

		public virtual Byte Height
		{
			get { return _lfsButton.Height; }
			set { _lfsButton.Height = value; }
		}

		public virtual ButtonColor Color
		{
			get { return _lfsButton.Color; }
			set { _lfsButton.Color = value; }
		}

		public virtual ButtonTextColor TextColor
		{
			get { return _lfsButton.TextColor; }
			set { _lfsButton.TextColor = value; }
		}

		public virtual ButtonTextAlignment TextAlignment
		{
			get { return _lfsButton.TextAlignment; }
			set { _lfsButton.TextAlignment = value; }
		}

		public Object Tag {
			get { return _tag; }
			set { _tag = value; }
		}

		public virtual bool IsEmpty
		{
			get { return false; }
		}

		public virtual void Show()
		{
			if (!_displayed && _visible) {
				_inSimHandler.ShowButton(_lfsButton, _connectionId);
				_displayed = true;
				if (_logDebug) {
					_log.Debug(String.Format("{0} with Id: '{1}', connectionId: '{2}' displayed", _lfsButton.ButtonType.ToString(), _lfsButton.ButtonId, _connectionId));
				}
			}
		}

		public virtual void Hide()
		{
			if (_displayed) {
				Debug.Assert(_visible);
				Debug.Assert(_lfsButton!=null);
				if (_lfsButton!=null) {
					_inSimHandler.DeleteButton(_lfsButton, _connectionId);
					_displayed = false;
					if (_logDebug) {
						_log.Debug(String.Format("{0} Id: '{1}', ConnectionId: '{2}' hidden", _lfsButton.ButtonType.ToString(), _lfsButton.ButtonId, _connectionId));
					}
				}
			}
		}

		public virtual void Delete()
		{
			if (_disposed) {	// Control can be deleted by owner cod or automatically, when driver leaves the game ...
				return;
			}

			if (_logDebug) {
				_log.Debug(String.Format("Deleting {0} Id: '{1}', ConnectionId: '{2}'", _lfsButton.ButtonType.ToString(), _lfsButton.ButtonId, _connectionId));
			}
			Hide();
			_controlFactory.RemoveControl(this);
			Dispose();
		}

		#endregion

		internal Byte ControlId
		{
			get { return _lfsButton.ButtonId; }
		}

		internal void OnDispose()
		{
			if (_logDebug) {
				Debug.Assert(_lfsButton!=null);
				if (_lfsButton!=null) {
					_log.Debug(String.Format("Disposing Id: '{0}', ConnectionId: '{1}'", _lfsButton.ButtonId, _connectionId));
				} else {
					_log.Error(String.Format("Disposing Id: _lfsButton = null, ConnectionId: '{0}'", _connectionId));
				}
			}

			if (Disposing!=null) {
				Disposing(this, new EventArgs());
			}
		}

		protected void UpdateControl()
		{
			if (!_visible || !_displayed) {
				return;
			}

			Byte oldWidth	= _lfsButton.Width;
			Byte oldHeight	= _lfsButton.Height;

			_lfsButton.Width	= 0;
			_lfsButton.Height	= 0;

			_inSimHandler.ShowButton(_lfsButton, _connectionId);

			_lfsButton.Width	= oldWidth;
			_lfsButton.Height	= oldHeight;

			if (_logDebug) {
					_log.Debug(String.Format("{0} Id: '{1}', ConnectionId: '{2}' updated", _lfsButton.ButtonType.ToString(), _lfsButton.ButtonId, _connectionId));
				}
		}
	}
}
