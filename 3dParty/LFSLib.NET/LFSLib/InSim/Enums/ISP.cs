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
  // ENUMERATIONS FOR PACKET TYPES
  // =============================

  enum ISP : byte// the second byte in all packets is one of these
  {
    NONE,		//  0					: not used
    ISI,		//  1 - instruction		: insim initialise
    VER,		//  2 - info			: version info
    TINY,		//  3 - both ways		: multi purpose
    SMALL,		//  4 - both ways		: multi purpose
    STA,		//  5 - info			: state info
    SCH,		//  6 - instruction		: single character
    SFP,		//  7 - instruction		: state flags pack
    SCC,		//  8 - instruction		: set car camera
    CPP,		//  9 - both ways		: cam pos pack
    ISM,		// 10 - info			: start multiplayer
    MSO,		// 11 - info			: message out
    III,		// 12 - info			: hidden /i message
    MST,		// 13 - instruction		: type message or /command
    MTC,		// 14 - instruction		: message to a connection
    MOD,		// 15 - instruction		: set screen mode
    VTN,		// 16 - info			: vote notification
    RST,		// 17 - info			: race start
    NCN,		// 18 - info			: new connection
    CNL,		// 19 - info			: connection left
    CPR,		// 20 - info			: connection renamed
    NPL,		// 21 - info			: new player (joined race)
    PLP,		// 22 - info			: player pit (keeps slot in race)
    PLL,		// 23 - info			: player leave (spectate - loses slot)
    LAP,		// 24 - info			: lap time
    SPX,		// 25 - info			: split x time
    PIT,		// 26 - info			: pit stop start
    PSF,		// 27 - info			: pit stop finish
    PLA,		// 28 - info			: pit lane enter / leave
    CCH,		// 29 - info			: camera changed
    PEN,		// 30 - info			: penalty given or cleared
    TOC,		// 31 - info			: take over car
    FLG,		// 32 - info			: flag (yellow or blue)
    PFL,		// 33 - info			: player flags (help flags)
    FIN,		// 34 - info			: finished race
    RES,		// 35 - info			: result confirmed
    REO,		// 36 - both ways		: reorder (info or instruction)
    NLP,		// 37 - info			: node and lap packet
    MCI,		// 38 - info			: multi car info
    MSX,		// 39 - instruction		: type message
    MSL,		// 40 - instruction		: message to local computer
    CRS,		// 41 - info			: car reset
    BFN,		// 42 - both ways		: delete buttons / receive button requests
    AXI,		// 43 - info			: autocross layout information
    AXO,		// 44 - info			: hit an autocross object
    BTN,		// 45 - instruction		: show a button on local or remote screen
    BTC,		// 46 - info			: sent when a user clicks a button
    BTT,		// 47 - info			: sent after typing into a button
    IRP_HLR	=	252,	// Send : To request a hostlist
    IRP_HOS	=	253,	// Receive : Hostlist info
    IRP_SEL	=	254,	// Send : To select a host
    IRP_ERR	=	255,	// Receive : An error number


  };
}
