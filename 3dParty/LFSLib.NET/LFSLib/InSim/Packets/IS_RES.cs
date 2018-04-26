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
  /// <summary>
  /// RESult (qualify or confirmed finish)
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_RES : ILfsInSimPacket
  {
    public byte Size;		// 84
    public Enums.ISP Type;		// ISP_RES
    public byte ReqI;		// 0 unless this is a reply to a TINY_RES request
    public byte PLID;		// player's unique id (0 = player left before result was sent)

    public Support.Char24 UName; //[24];	// username
    public Support.Char24 PName; //[24];	// nickname
    public Support.Char8 Plate; //[8];	// number plate - NO ZERO AT END!
    public Support.Char4 CName; //[4];	// skin prefix

    public uint TTime;	// race time (ms)
    public uint BTime;	// best lap (ms)

    public byte SpA;
    public byte NumStops;	// number of pit stops
    public Flags.CONF Confirm;	// confirmation flags : disqualified etc - see below
    public byte SpB;

    public ushort LapsDone;	// laps completed
    public Flags.PIF Flags;		// player flags : help settings etc - see below

    public byte ResultNum;	// finish or qualify pos (0 = win / 255 = not added to table)
    public byte NumRes;		// total number of results (qualify doesn't always add a new one)
    public ushort PSeconds;	// penalty time in seconds (already included in race time)

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }
}
