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
using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.Controls
{
	internal class Button : Control, IButton
	{
		private Char	_accessKey;

		private static IButton _empty = null;
		internal static IButton Empty
		{
			get {
				if (_empty==null) {
					_empty = new ButtonEmpty();
				}

				return _empty;
			}
		}

		protected Button()
		{
		}

		internal Button(ControlFactory controlFactory, Byte buttonId, Byte connectionId) : base (controlFactory, buttonId, connectionId)
		{
			_log		= LogManager.GetLogger(typeof(Button));
			_logDebug	= _log.IsDebugEnabled;

			Debug.Assert(_lfsButton!=null);
			_lfsButton.Clickable						= true;
			_lfsButton.InitializeDialogWithButtonText	= false;

			Debug.Assert(_lfsButton.ButtonType == FullMotion.LiveForSpeed.InSim.Enums.ButtonType.Button);

			if (_logDebug) {
				_log.Debug(String.Format("Button with Id: '{0}', connectionId: '{1}' created", _lfsButton.ButtonId, _connectionId));
			}
		}

		~Button()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "Button, call dispose before the application is closed!");
			Dispose(false);
		}

		#region IButton

		public event EventHandler<ButtonClickEventArgs> Click;

		public virtual Char AccessKey
		{
			get { return _accessKey; }
			set { _accessKey = value; }
		}

		public virtual String Text
		{
			get { return _lfsButton.Text; }
			set
			{
				if (value != _lfsButton.Text) {
					_lfsButton.Text = value;
					if (_displayed) {
						UpdateControl();
					}
				}
			}
		}

		internal void OnClick(ButtonClickEventArgs buttonEventArgs)
		{
			if (_logDebug) {
				_log.Debug(String.Format("Button with Id: '{0}', connectionId: '{1}' clicked", _lfsButton.ButtonId, _connectionId));
			}

			if (Click!=null) {
				Click(this, buttonEventArgs);
			}
		}

		#endregion
	}
}
