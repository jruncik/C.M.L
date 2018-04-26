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
using SR.CML.Core.InSimCommon.Controls;
using FullMotion.LiveForSpeed.InSim;

namespace SR.CML.Core.InSimCommon
{
	// PluginId 536550BD-BE60-4fd2-91B7-098FCF43F5B1
	public interface ICMLCore : IDisposable
	{
		event EventHandler<EventArgs> ConnectionStateChanged;

		InSimHandler InSimHandler
		{
			get;
		}

		bool IsConnected
		{
			get;
		}

		void SendCommand(String command);
	}
}
