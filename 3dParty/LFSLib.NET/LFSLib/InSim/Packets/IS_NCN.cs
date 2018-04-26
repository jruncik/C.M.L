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
  /// New ConN
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_NCN : ILfsInSimPacket
  {
    public byte Size;		// 56
    public Enums.ISP Type;		// ISP_NCN
    public byte ReqI;		// 0 unless this is a reply to a TINY_NCN request
    public byte UCID;		// new connection's unique id (0 = host)

    public Support.Char24 UName; //[24];	// username
    public Support.Char24 PName; //[24];	// nickname

    public byte Admin;		// 1 if admin
    public byte Total;		// number of connections including host
    public Flags.NCN Flags;		// bit 2 : remote
    public byte Sp3;

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }
}
