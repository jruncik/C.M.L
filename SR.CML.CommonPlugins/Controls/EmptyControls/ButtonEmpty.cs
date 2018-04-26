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
	internal class ButtonEmpty : ControlEmpty, IButton
	{
		public event EventHandler<ButtonClickEventArgs> Click;

		internal ButtonEmpty()
		{
			_log = LogManager.GetLogger(typeof(ButtonEmpty));
			_logDebug = _log.IsDebugEnabled;
		}

		~ButtonEmpty()
		{
		}

		public Char AccessKey
		{
			get { return Char.MinValue; }
			set { }
		}

		public String Text
		{
			get { return String.Empty; }
			set { }
		}

		private void SuppressWarning() {
			if (Click == null) {
			}
		}
	}
}
