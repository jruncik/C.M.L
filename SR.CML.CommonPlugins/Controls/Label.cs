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
	internal class Label : Control, ILabel
	{
		private static ILabel _empty = null;

		internal static ILabel Empty
		{
			get
			{
				if (_empty==null) {
					_empty = new LabelEmpty();
				}
				return _empty;
			}
		}

		protected Label()
		{
		}

		internal Label(ControlFactory controlFactory, Byte buttonId, Byte connectionId) : base (controlFactory, buttonId, connectionId)
		{
			if (_log==null) {
				_log		= LogManager.GetLogger(typeof(Label));
				_logDebug	= _log.IsDebugEnabled;
			}

			Debug.Assert(_lfsButton!=null);
			_lfsButton.Clickable = false;

			Debug.Assert(_lfsButton.ButtonType == FullMotion.LiveForSpeed.InSim.Enums.ButtonType.Label);

			if (_logDebug) {
				_log.Debug(String.Format("Label with Id: '{0}', connectionId: '{1}' created", _lfsButton.ButtonId, _connectionId));
			}
		}

		~Label()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "Label, call dispose before the application is closed!");
			Dispose(false);
		}

		#region ILabel

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

		#endregion
	}
}
