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
  /// LAP time
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_LAP : ILfsInSimPacket
  {
    public byte Size;		// 20
    public Enums.ISP Type;		// ISP_LAP
    public byte ReqI;		// 0
    public byte PLID;		// player's unique id

    public uint LTime;	// lap time (ms)
    public uint ETime;	// total time (ms)

    public ushort LapsDone;	// laps completed
    public Flags.PIF Flags;		// player flags

    public byte Sp0;
    public Enums.Penalty Penalty;	// current penalty value (see below)
    public byte NumStops;	// number of pit stops
    public byte Sp3;

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

}
