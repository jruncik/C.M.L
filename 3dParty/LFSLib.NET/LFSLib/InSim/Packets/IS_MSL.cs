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
  /// MSg Local - message to appear on local computer only
  /// </summary>
  struct IS_MSL : IClientLfsInSimPacket
  {
    public byte Size;		// 132
    public Enums.ISP Type;		// ISP_MSL
    public byte ReqI;		// 0
    public Enums.Sound Sound;		// sound effect (see Message Sounds below)

    public byte[] Msg; //[128];	// last byte must be zero

    public IS_MSL(string message, Enums.Sound soundEffect)
    {
      Size = 132;
      Type = Enums.ISP.MSL;
      ReqI = 0;
      Sound = soundEffect;
      Msg = CharHelper.GetBytes(message,128,true);
    }

    public IS_MSL(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      Sound = (Enums.Sound)bytes[position++];
      Msg = new byte[128];
      Array.Copy(bytes, position, Msg, 0, 128);
    }

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion

    #region IClientLfsInSimPacket Members

    public byte RequestId
    {
      get { return ReqI; }
    }

    public byte[] GetBytes()
    {
      byte[] bytes = new byte[Size];

      int position = 0;

      bytes[position++] = Size;
      bytes[position++] = (byte)Type;
      bytes[position++] = ReqI;
      bytes[position++] = (byte)Sound;

      Array.Copy(this.Msg, 0, bytes, position, this.Msg.Length);

      return bytes;
    }

    #endregion
  }
}
