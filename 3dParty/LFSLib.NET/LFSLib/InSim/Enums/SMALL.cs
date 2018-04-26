/* ------------------------------------------------------------------------- *
 * Copyright (C) 2007 Arne Claassen
 *
 * Arne Claassen <lfslib [at] claassen [dot] net>
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
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * ------------------------------------------------------------------------- */
using System;
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim.Enums
{
	enum SMALL : byte // the fourth byte of IS_SMALL packets is one of these
	{
		NONE,		//  0					: not used
		SSP,		//  1 - instruction		: start sending positions
		SSG,		//  2 - instruction		: start sending gauges
		VTA,		//  3 - report			: vote action
		TMS,		//  4 - instruction		: time stop
		STP,		//  5 - instruction		: time step
		RTP,		//  6 - info			: race time packet (reply to GTH)
		NLI,		//  7 - instruction		: set node lap interval
	}
}
