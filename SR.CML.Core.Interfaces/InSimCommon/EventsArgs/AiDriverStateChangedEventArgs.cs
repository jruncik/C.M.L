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

namespace SR.CML.Core.InSimCommon
{
	public class AiDriverStateChangedEventArgs : EventArgs
	{
		private IInSimDriverAi _aiDriver;
		public IInSimDriverAi AiDriver
		{
			get { return _aiDriver; }
		}

		private Int32 _aiDriversCount;
		public Int32 AiDriversCount
		{
			get { return _aiDriversCount; }
		}

		private bool _added;
		public bool Added
		{
			get { return _added; }
		}

		public AiDriverStateChangedEventArgs(IInSimDriverAi aiDriver, Int32 aiDriversCount, bool added)
		{
			_aiDriver		= aiDriver;
			_aiDriversCount	= aiDriversCount;
			_added			= added;
		}
	}
}
