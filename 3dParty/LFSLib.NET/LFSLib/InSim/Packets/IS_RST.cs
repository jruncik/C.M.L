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
using System.Runtime.InteropServices;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim.Packets
{

  // In LFS there is a list of connections AND a list of players in the race
  // Some packets are related to connections, some players, some both

  // If you are making a multiplayer InSim program, you must maintain two lists
  // You should use the unique identifier UCID to identify a connection

  // Each player has a unique identifier PLID from the moment he joins the race, until he
  // leaves.  It's not possible for PLID and UCID to be the same thing, for two reasons :

  // 1) there may be more than one player per connection if AI drivers are used
  // 2) a player can swap between connections, in the case of a driver swap (IS_TOC)

  // When all players are cleared from race (e.g. /clear) LFS sends this IS_TINY

  // ReqI : 0
  // SubT : TINY_CLR		(CLear Race)

  // When a race ends (return to game setup screen) LFS sends this IS_TINY

  // ReqI : 0
  // SubT : TINY_REN  	(Race ENd)

  // You can instruct LFS host to cancel a vote using an IS_TINY

  // ReqI : 0
  // SubT : TINY_VTC		(VoTe Cancel)

  /// <summary>
  /// Race STart
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_RST : ILfsInSimPacket
  {
    public byte Size;		// 28
    public Enums.ISP Type;		// ISP_RST
    public byte ReqI;		// 0 unless this is a reply to an TINY_RST request
    public byte Zero;

    public byte RaceLaps;	// 0 if qualifying
    public byte QualMins;	// 0 if race
    public byte NumP;		// number of players in race
    public byte Spare;

    public Support.Char6 Track; //[6];	// short track name
    public Enums.Weather Weather;
    public Enums.Wind Wind;

    public Flags.RaceFlags Flags;		// race flags (must pit, can reset, etc - see below)
    public ushort NumNodes;	// total number of nodes in the path
    public ushort Finish;		// node index - finish line
    public ushort Split1;		// node index - split 1
    public ushort Split2;		// node index - split 2
    public ushort Split3;		// node index - split 3

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

  // To request an IS_RST packet at any time, send this IS_TINY :

  // ReqI : non-zero		(returned in the reply)
  // SubT : TINY_RST		(request an IS_RST)
}
