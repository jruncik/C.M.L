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
	enum TINY : byte  // the fourth byte of IS_TINY packets is one of these
	{
		NONE,		//  0					: see "maintaining the connection"
		VER,		//  1 - info request	: get version
		CLOSE,		//  2 - instruction		: close insim
		PING,		//  3 - ping request	: external progam requesting a reply
		REPLY,		//  4 - ping reply		: reply to a ping request
		VTC,		//  5 - info			: vote cancelled
		SCP,		//  6 - info request	: send camera pos
		SST,		//  7 - info request	: send state info
		GTH,		//  8 - info request	: get time in hundredths (i.e. SMALL_RTP)
		MPE,		//  9 - info			: multi player end
		ISM,		// 10 - info request	: get multiplayer info (i.e. ISP_ISM)
		REN,		// 11 - info			: race end (return to game setup screen)
		CLR,		// 12 - info			: all players cleared from race
		NCN,		// 13 - info			: get all connections
		NPL,		// 14 - info			: get all players
		RES,		// 15 - info			: get all results
		NLP,		// 16 - info request	: send an IS_NLP packet
		MCI,		// 17 - info request	: send an IS_MCI packet
		REO,		// 18 - info request	: send an IS_REO packet
		RST,		// 19 - info request	: send an IS_RST packet
		AXI,		// 20 - info request	: send an IS_AXI
		AXC,		// 21 - info			: autocross cleared
	}

}
