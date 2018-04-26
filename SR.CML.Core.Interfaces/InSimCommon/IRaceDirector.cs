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
	// PluginID {AFACAC8A-B63D-4006-940B-4C31999ECE37}
	public interface IRaceDirector : IDisposable
	{
		event EventHandler<RaceStateChangedEventArgs> StateChanged;

		RaceState State
		{
			get;
		}

		void SetGrid(String[] driversOrder, bool ensureEmptySpaceOnGrid);

		void StartRace();
		void StartRace(Int32 laps);
		void StartRace(TimeSpan time);

		void StartQualify(Int32 minuts);

		void StartPractice();
	}
}
