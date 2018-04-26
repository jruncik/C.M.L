﻿/* ------------------------------------------------------------------------- *
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

namespace SR.CML.Core.InSimCommon
{
	// pluginID {6301C0A4-8FFE-4baf-9569-07753A268AAC}
	public interface IResultManager : IDisposable
	{
		event EventHandler<EventArgs> AllCarsFinished;
		event EventHandler<CarFinishedEventArgs> CarFinished;

		IRaceResult GetResult(IInSimCar car);
		IList<IRaceResult> GetSortedRaceResults();
		IList<IRaceResult> GetSortedQualifyResults();

		void ClearResults();
	}
}
