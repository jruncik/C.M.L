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
using SR.CML.Core.InSimCommon;

namespace SR.CML.Core.InSimCommon
{
	public class CarStateChangedEventArgs : EventArgs
	{
		private CarState _oldState;
		public CarState OldState
		{
			get { return _oldState; }
		}

		private CarState _newState;
		public CarState NewState
		{
			get { return _newState; }
		}

		private IInSimCar _car;
		public IInSimCar Car
		{
			get { return _car; }
		}

		public CarStateChangedEventArgs(IInSimCar car, CarState oldState, CarState newState)
		{
			_car		= car;
			_oldState	= oldState;
			_newState	= newState;
		}
	}
}
