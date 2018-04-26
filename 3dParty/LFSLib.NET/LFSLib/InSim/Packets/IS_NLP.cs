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
  /// Node and Lap Packet - variable size
  /// </summary>
  struct IS_NLP : ILfsInSimPacket
  {
    public byte Size;		// 4 + NumP * 6 (PLUS 2 if needed to make it a multiple of 4)
    public Enums.ISP Type;		// ISP_NLP
    public byte ReqI;		// 0 unless this is a reply to an TINY_NLP request
    public byte NumP;		// number of players in race

    public Support.NodeLap[] Info; //[32];	// node and lap of each player, 1 to 32 of these (NumP)

    public IS_NLP(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      NumP = bytes[position++];
      Info = new Support.NodeLap[NumP];
      for (int i = 0; i < NumP; i++)
      {
        byte[] nodeLapBytes = new byte[Support.NodeLap.SIZE];
        Array.Copy(bytes, position, nodeLapBytes, 0, Support.NodeLap.SIZE);
        position += Support.NodeLap.SIZE;
        Info[i] = new Support.NodeLap(nodeLapBytes);
      }
    }

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

  // If ISF_MCI flag is set, a set of IS_MCI packets is sent...
}
