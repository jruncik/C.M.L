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
  /// MSg eXtended - like MST but longer (not for commands)
  /// </summary>
  struct IS_MSX : IClientLfsInSimPacket
  {
    public byte Size;		// 100
    public Enums.ISP Type;		// ISP_MSX
    public byte ReqI;		// 0
    public byte Zero;

    public byte[] Msg; //[96];	// last byte must be zero

    public IS_MSX(byte[] bytes)
    {
      if (bytes.Length == 96)
      {
        // initializing with string
        Size = 100;
        Type = Enums.ISP.MSX;
        ReqI = 0;
        Zero = 0;
        Msg = bytes;
      }
      else
      {
        int position = 0;
        Size = bytes[position++];
        Type = (Enums.ISP)bytes[position++];
        ReqI = bytes[position++];
        position++;
        Zero = 0;
        Msg = new byte[96];
        Array.Copy(bytes, position, Msg, 0, 96);
      }
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
      byte[] bytes = new byte[Size];

      int position = 0;

      bytes[position++] = Size;
      bytes[position++] = (byte)Type;
      bytes[position++] = ReqI;
      bytes[position++] = Zero;

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
