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
  // On false start or wrong route / restricted area, an IS_PEN packet is sent :

  // False start : OldPen = 0 / NewPen = PENALTY_30 / Reason = PENR_FALSE_START
  // Wrong route : OldPen = 0 / NewPen = PENALTY_45 / Reason = PENR_WRONG_WAY

  // If an autocross object is hit (2 second time penalty) this packet is sent :

  /// <summary>
  /// Autocross object
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_AXO : ILfsInSimPacket
  {
    public byte Size;		// 4
    public Enums.ISP Type;		// ISP_AXO
    public byte ReqI;		// ReqI as received in the IS_BTN
    public byte PLID;		// player's unique id

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

}
