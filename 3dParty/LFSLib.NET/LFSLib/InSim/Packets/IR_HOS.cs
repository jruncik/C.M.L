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
  struct IR_HOS : ILfsInSimPacket
  {
	public byte Size;		// 4 + NumHosts * 40
	public Enums.ISP Type;		// IRP_HOS
	public byte ReqI;		// 0
	public byte NumHosts;	// Number of hosts described in this packet

	public Support.HInfo[]	Info; //[6];	// Host info for every host in the Relay. 1 to 6 of these in a IR_HOS

    public IR_HOS(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      NumHosts = bytes[position++];
      Info = new Support.HInfo[NumHosts];
      for (int i = 0; i < NumHosts; i++)
      {
        byte[] hostInfoBytes = new byte[Support.HInfo.SIZE];
        Array.Copy(bytes, position, hostInfoBytes, 0, Support.HInfo.SIZE);
        position += Support.HInfo.SIZE;
        Info[i] = new Support.HInfo(hostInfoBytes);
      }
    }

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }
}
