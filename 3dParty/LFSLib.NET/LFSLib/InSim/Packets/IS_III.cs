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
  /// <summary>
  /// InsIm Info - /i message from user to host's InSim
  /// </summary>
  struct IS_III : ILfsInSimPacket
  {
    public byte Size;		// 72
    public Enums.ISP Type;		// ISP_III
    public byte ReqI;		// 0
    public byte Zero;

    public byte UCID;		// connection's unique id (0 = host)
    public byte PLID;		// player's unique id (if zero, use UCID)
    public byte Sp2;
    public byte Sp3;

    public byte[] Msg; //[64];

    public IS_III(byte[] bytes)
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

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

}
