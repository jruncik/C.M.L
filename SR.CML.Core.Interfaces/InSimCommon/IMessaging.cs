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
	//PluginID 863E4F77-94E9-43fb-8A46-323220837A0D
	public interface IMessaging : IDisposable
	{
		/// <summary>
		/// Send standard message to all drivers
		/// </summary>
		/// <param name="message">message text</param>
		void SendMessageToAll(String message);

		/// <summary>
		/// Send standard message to active driver in car
		/// </summary>
		/// <param name="car">InSim car</param>
		/// <param name="message">message text</param>
		void SendMessageToCar(IInSimCar car, String message);

		/// <summary>
		/// Send standard message to driver
		/// </summary>
		/// <param name="driver">InSim driver</param>
		/// <param name="message">message text</param>
		void SendMessageToDriver(IInSimDriver driver, String message);

		/// <summary>
		/// Display RCM message to all drivers
		/// </summary>
		/// <param name="message">Message for drivers</param>
		/// <param name="seconds">Display time. 0 - permanent</param>
		void SendRaceControlMessageToAll(String message, Int16 seconds);

		/// <summary>
		/// Display RCM mesage for concrete car (active driver)
		/// </summary>
		/// <param name="car">InSim Car</param>
		/// <param name="message">Message for driver</param>
		/// <param name="seconds">Display time. 0 - permanent</param>
		void SendRaceControlMessageToCar(IInSimCar car, String message, Int16 seconds);

		/// <summary>
		/// Display custom mesage to all drivers on central Label
		/// </summary>
		/// <param name="message">Message for drivers</param>
		/// <param name="seconds">Display time. 0 - permanent</param>
		void SendControlMessageToAll(String message, Int16 seconds);

		/// <summary>
		/// Display custom mesage for concrete car (active driver) on central Label
		/// </summary>
		/// <param name="car">InSim Car</param>
		/// <param name="message">Message for driver</param>
		/// <param name="seconds">Display time. 0 - permanent</param>
		void SendControlMessageToCar(IInSimCar car, String message, Int16 seconds);
	}
}
