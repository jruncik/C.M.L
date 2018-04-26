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

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class ActivatingNextGroupEventArgs : EventArgs
	{
		private Group _finishedGrupe;
		internal Group FinishedGrupe
		{
			get { return _finishedGrupe; }
		}

		private Int32 _heatIndex;
		public Int32 HeatIndex
		{
			get { return _heatIndex; }
		}

		private bool _newHeatWillBeActivated;
		public bool NewHeatWillBeActivated
		{
			get { return _newHeatWillBeActivated; }
		}

		internal ActivatingNextGroupEventArgs(Group finishedGrupe, Int32 heatIndex, bool newHeatWillBeActivated)
		{
			_finishedGrupe			= finishedGrupe;
			_heatIndex				= heatIndex;
			_newHeatWillBeActivated	= newHeatWillBeActivated;
		}
	}
}
