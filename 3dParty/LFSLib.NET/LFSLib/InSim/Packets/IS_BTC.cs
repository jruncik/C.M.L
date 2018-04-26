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
  // Replies : If the user clicks on a clickable button, this packet will be sent :

  /// <summary>
  /// BuTton Click - sent back when user clicks a button
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_BTC : ILfsInSimPacket
  {
    public byte Size;		// 8
    public Enums.ISP Type;		// ISP_BTC
    public byte ReqI;		// ReqI as received in the IS_BTN
    public byte UCID;		// connection that clicked the button (zero if local)

    public byte ClickID;	// button identifier originally sent in IS_BTN
    public byte Inst;		// used internally by InSim
    public Flags.ClickFlags CFlags;		// button click flags - see below
    public byte Sp3;

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

}
