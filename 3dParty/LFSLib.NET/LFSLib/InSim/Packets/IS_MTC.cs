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
using FullMotion.LiveForSpeed.Util;

namespace FullMotion.LiveForSpeed.InSim.Packets
{
  /// <summary>
  /// Msg To Connection - hosts only - send to a connection or a player
  /// </summary>
  struct IS_MTC : IClientLfsInSimPacket
  {
    public byte Size;		// 72
    public Enums.ISP Type;		// ISP_MTC
    public byte ReqI;		// 0
    public byte Zero;

    public byte UCID;		// connection's unique id (0 = host)
    public byte PLID;		// player's unique id (if zero, use UCID)
    public byte Sp2;
    public byte Sp3;

    public byte[] Msg; //[64];	// last byte must be zero

    public IS_MTC(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      position++;
      Zero = 0;
      UCID = bytes[position++];
      PLID = bytes[position++];
      position++;
      Sp2 = 0;
      position++;
      Sp3 = 0;
      Msg = new byte[64];
      Array.Copy(bytes, position, Msg, 0, 64);
    }

    public IS_MTC(byte recipient, string message, bool player)
    {
      Size = 72;
      Type = Enums.ISP.MTC;
      ReqI = 0;
      Zero = 0;
      UCID = (player) ? (byte)0 : recipient;
      PLID = (player) ? recipient : (byte)0;
      Sp2 = 0;
      Sp3 = 0;
      Msg = CharHelper.GetBytes(message, 64, true);
    }

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion

    #region IClientLfsInSimPacket Members

    public byte[] GetBytes()
    {
      byte[] bytes = new byte[72];

      int position = 0;

      bytes[position++] = Size;
      bytes[position++] = (byte)Type;
      bytes[position++] = ReqI;
      bytes[position++] = Zero;
      bytes[position++] = UCID;
      bytes[position++] = PLID;
      bytes[position++] = Sp2;
      bytes[position++] = Sp3;

      Array.Copy(this.Msg, 0, bytes, position, this.Msg.Length);

      return bytes;
    }

    public byte RequestId
    {
      get { return ReqI; }
    }

    #endregion
  }
}
