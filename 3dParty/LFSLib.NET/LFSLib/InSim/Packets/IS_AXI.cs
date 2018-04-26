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

namespace FullMotion.LiveForSpeed.InSim.Packets
{
  // When all objects are cleared from a layout, LFS sends this IS_TINY :

  // ReqI : 0
  // SubT : TINY_AXC		(AutoX Cleared)

  // You can request information about the current layout with this IS_TINY :

  // ReqI : non-zero		(returned in the reply)
  // SubT : TINY_AXI		(AutoX Info)

  // The information will be sent back in this packet (also sent when a layout is loaded) :

  struct IS_AXI : ILfsInSimPacket
  {
    public byte Size;       // 40
    public Enums.ISP Type;		// ISP_AXI
    public byte ReqI;		// 0 unless this is a reply to an TINY_AXI request
    public byte Zero;

    public byte AXStart;	// autocross start position
    public byte NumCP;		// number of checkpoints
    public ushort NumO;		// number of objects

    public byte[] LName;	// [32] //the name of the layout last loaded (if loaded locally)

    public IS_AXI(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      position++;
      Zero = 0;
      AXStart = bytes[position++];
      NumCP = bytes[position++];
      NumO = (ushort)BitConverter.ToInt16(bytes, position);
      position += 2;
      LName = new byte[32];
      Array.Copy(bytes, position, LName, 0, 32);
    }

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion

  }
}
