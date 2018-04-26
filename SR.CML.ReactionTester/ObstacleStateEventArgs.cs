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

namespace SR.CML.ReactionTester
{
	internal class ObstacleStateEventArgs : EventArgs
	{
		private ObstacleState _newState;
		internal ObstacleState NewState
		{
			get { return _newState; }
		}

		private ObstacleState _oldState;
		internal ObstacleState OldState
		{
			get { return _oldState; }
		}

		internal ObstacleStateEventArgs(ObstacleState newState, ObstacleState oldState) 
		{
			_newState = newState;
			_oldState = oldState;
		}
	}
}
