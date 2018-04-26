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
  // MULTIPLAYER NOTIFICATION
  // ========================

  // LFS will send this packet when a host is started or joined :

  /// <summary>
  /// InSim Multi
  /// </summary>
  struct IS_ISM : ILfsInSimPacket
  {
    public byte Size;		// 40
    public Enums.ISP Type;		// ISP_ISM
    public byte ReqI;		// usually 0 / or if a reply : ReqI as received in the TINY_ISM
    public byte Zero;

    public Enums.ServerType Host;		// 0 = guest / 1 = host
    public byte Sp1;
    public byte Sp2;
    public byte Sp3;

    public byte[] HName; //[32];	// the name of the host joined or started

    public IS_ISM(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      position++;
      Zero = 0;
      Host = (Enums.ServerType)bytes[position++];
      position++;
      Sp1 = 0;
      position++;
      Sp2 = 0;
      position++;
      Sp3 = 0;
      HName = new byte[32];
      Array.Copy(bytes, position, HName, 0, 32);
    }

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

  // On ending or leaving a host, LFS will send this IS_TINY :

  // ReqI : 0
  // SubT : TINY_MPE		(MultiPlayerEnd)

  // To request an IS_ISM packet at any time, send this IS_TINY :

  // ReqI : non-zero		(returned in the reply)
  // SubT : TINY_ISM		(request an IS_ISM)

  // NOTE : If LFS is not in multiplayer mode, the host name in the ISM will be empty.
}
