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
  // IN GAME camera control
  // ----------------------

  // You can set the viewed car and selected camera directly with a special packet
  // These are the states normally set in-game by using the TAB and V keys

  /// <summary>
  /// Set Car Camera - Simplified camera packet (not SHIFT+U mode)
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_SCC : IClientLfsInSimPacket
  {
    public byte Size;		// 8
    public Enums.ISP Type;		// ISP_SCC
    public byte ReqI;		// 0
    public byte Zero;

    public byte ViewPLID;	// UniqueID of player to view
    public Enums.View InGameCam;	// InGameCam (as reported in StatePack)
    public byte Sp2;
    public byte Sp3;

    public IS_SCC(byte playerId, Enums.View camera)
    {
      Size = 8;
      Type = Enums.ISP.SCC;
      ReqI = 0;
      Zero = 0;
      ViewPLID = playerId;
      InGameCam = camera;
      Sp2 = 0;
      Sp3 = 0;
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
      return PacketFactory.GetBytesSequentially(this, Size);
    }

    public byte RequestId
    {
      get { return ReqI; }
    }

    #endregion
  }
}
