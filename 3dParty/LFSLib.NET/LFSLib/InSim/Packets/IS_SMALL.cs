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
  // GENERAL PURPOSE PACKETS - IS_TINY (4 bytes) and IS_SMALL (8 bytes)
  // =======================

  // To avoid defining several packet structures that are exactly the same, and to avoid
  // wasting the ISP_ enumeration, IS_TINY is used at various times when no additional data
  // other than SubT is required.  IS_SMALL is used when an additional integer is needed.

  // IS_SMALL - used for various requests, replies and reports

  /// <summary>
  /// General purpose 8 byte packet
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_SMALL : IClientLfsInSimPacket
  {
    public byte Size;		// always 8
    public Enums.ISP Type;		// always ISP_SMALL
    public byte ReqI;		// 0 unless it is an info request or a reply to an info request
    public Enums.SMALL SubT;		// subtype, from SMALL_ enumeration (e.g. SMALL_SSP)

    public uint UVal;	// value (e.g. for SMALL_SSP this would be the OutSim packet rate)

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    public IS_SMALL(Enums.SMALL subType, byte requestId, uint value)
    {
      Size = 8;
      Type = Enums.ISP.SMALL;
      ReqI = requestId;
      SubT = subType;
      UVal = value;
    }
    #endregion

    #region IClientLfsInSimPacket Members

    public byte[] GetBytes()
    {
      return PacketFactory.GetBytesSequentially(this, Size);
    }

    public byte RequestId
    {
      get { return ReqI; }
    }

    #endregion
  }
}
