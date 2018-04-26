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

using FullMotion.LiveForSpeed.InSim;

using System.Diagnostics;


namespace SR.CML.Core.InSimCommon
{
	// PluginID "F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3"
	public interface ICarDriverManager : IDisposable
	{
		event EventHandler<DriverStateEventArgs>			DriverStateChanged;
		event EventHandler<AiDriverStateChangedEventArgs>	AiDriverStateChanged;
		event EventHandler<CarStateChangedEventArgs>		CarStateChanged;

		IInSimCar GetCarByLFSName(String name);
		IInSimCar GetCarByPlayerID(Byte playerId);
		IInSimCar GetCarByConnectionID(Byte connectinId);

		IInSimDriver GetDriverByConnectionID(Byte connectinId);

		IList<IInSimDriverAi> GetAiDrivers();
		void AddAiDriver();
		void RemoveAllAiDrivers();
		bool IsAiDriver(Byte driverId);

		IList<IInSimCar> Cars
		{
			get;
		}

		IList<IInSimDriver> Drivers
		{
			get;
		}
	}
}
