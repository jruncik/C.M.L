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
  /// New PLayer joining race (if PLID already exists, then leaving pits)
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_NPL : ILfsInSimPacket
  {
    public byte Size;		// 76
    public Enums.ISP Type;		// ISP_NPL
    public byte ReqI;		// 0 unless this is a reply to an TINY_NPL request
    public byte PLID;		// player's newly assigned unique id

    public byte UCID;		// connection's unique id
    public Flags.PlayerType PType;		// bit 0 : female / bit 1 : AI / bit 2 : remote
    public Flags.PIF Flags;		// player flags

    public Support.Char24 PName; //[24];	// nickname
    public Support.Char8 Plate; //[8];	// number plate - NO ZERO AT END!

    public Support.Char4 CName; //[4];	// car name
    public Support.Char16 SName; //[16];	// skin name - MAX_CAR_TEX_NAME
    public Support.Tyres Tyres; //[4];	// compounds

    public byte H_Mass;		// added mass (kg)
    public byte H_TRes;		// intake restriction
    public byte Model;
    public Flags.Passengers Pass;		// passengers byte

    public int Spare;

    public Flags.SETF SetF;
    public byte NumP;		// number in race (same when leaving pits, 1 more if new)
    public byte Sp2;
    public byte Sp3;

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }
}
