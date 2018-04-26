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
  // IS_REO : REOrder - this packet can be sent in either direction

  // LFS sends one at the start of every race or qualifying session, listing the start order

  // You can send one to LFS before a race start, to specify the starting order.
  // It may be a good idea to avoid conflict by using /start=fixed (LFS setting).
  // Alternatively, you can leave the LFS setting, but make sure you send your IS_REO
  // AFTER you receive the IS_VTA.  LFS does its default grid reordering at the same time
  // as it sends the IS_VTA (VoTe Action) and you can override this by sending an IS_REO.

  /// <summary>
  /// REOrder (when race restarts after qualifying)
  /// </summary>
  struct IS_REO : IClientLfsInSimPacket
  {
    public byte Size;		// 36
    public Enums.ISP Type;		// ISP_REO
    public byte ReqI;		// 0 unless this is a reply to an TINY_REO request
    public byte NumP;		// number of players in race

    public byte[] PLID; //[32];	// all PLIDs in new order

    public IS_REO(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      NumP = bytes[position++];
      PLID = new byte[32];
      for (int i = 0; i < 32; i++)
      {
        PLID[i] = bytes[position++];
      }
    }

    public IS_REO(byte[] players, byte requestId)
    {
      Size = 36;
      Type = Enums.ISP.REO;
      ReqI = requestId;
      NumP = (byte)players.Length;
      PLID = new byte[32];
      for (int i = 0; i < NumP; i++)
      {
        PLID[i] = players[i];
      }
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
      bytes[position++] = NumP;

      Array.Copy(this.PLID, 0, bytes, position, this.PLID.Length);

      return bytes;
    }

    #endregion
  }

  // To request an IS_REO packet at any time, send this IS_TINY :

  // ReqI : non-zero		(returned in the reply)
  // SubT : TINY_REO		(request an IS_REO)}
}
