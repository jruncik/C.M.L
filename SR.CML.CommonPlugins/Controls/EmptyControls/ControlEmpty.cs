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
	internal class ControlEmpty : IControl
	{

		protected static ILog	_log		= null;
		protected static bool	_logDebug	= false;

		public event EventHandler<EventArgs> Disposing;

		internal ControlEmpty()
		{
		}

		~ControlEmpty()
		{
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
		}

		public bool Visible
		{
			get { return false; }
			set { }
		}

		public Byte Left
		{
			get { return 0; }
			set { }
		}

		public Byte Right
		{
			get { return 0; }
			set { }
		}

		public Byte Top
		{
			get { return 0; }
			set { }
		}

		public Byte Bottom
		{
			get { return 0; }
			set { }
		}

		public Byte Width
		{
			get { return 0; }
			set { }
		}

		public Byte Height
		{
			get { return 0; }
			set { }
		}

		public ButtonColor Color
		{
			get { return ButtonColor.Dark; }
			set { }
		}

		public ButtonTextColor TextColor
		{
			get { return ButtonTextColor.Unavailable; }
			set { }
		}

		public ButtonTextAlignment TextAlignment
		{
			get { return ButtonTextAlignment.Left; }
			set { }
		}

		public Object Tag {
			get { return null; }
			set { }
		}

		public void Show()
		{
			Debug.Assert(false);
			_log.Error("Show on Empty control doesn't offer any functionality!");
		}

		public void Hide()
		{
			Debug.Assert(false);
			_log.Error("Hide on Empty control doesn't offer any functionality!");
		}

		public void Delete()
		{
			Debug.Assert(false);
			_log.Error("Delete on Empty control doesn't offer any functionality!");
		}

		public bool IsEmpty
		{
			get { return true; }
		}

		private void SuppressWarning() {
			if (Disposing == null) {
			}
		}
	}
}
