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
  // IS_CCH : Camera CHange

  // To track cameras you need to consider 3 points

  // 1) The default camera : VIEW_DRIVER
  // 2) Player flags : CUSTOM_VIEW means VIEW_CUSTOM at start or pit exit
  // 3) IS_CCH : sent when an existing driver changes camera

  /// <summary>
  /// Camera CHange
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_CCH : ILfsInSimPacket
  {
    public byte Size;		// 8
    public Enums.ISP Type;		// ISP_CCH
    public byte ReqI;		// 0
    public byte PLID;		// player's unique id

    public Enums.View Camera;		// view identifier (see below)
    public byte Sp1;
    public byte Sp2;
    public byte Sp3;

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

}
