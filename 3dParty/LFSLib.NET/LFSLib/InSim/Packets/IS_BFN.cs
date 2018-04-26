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
  // BUTTONS
  // =======

  // You can make up to 240 buttons appear on the host or guests (ID = 0 to 239).
  // You should set the ISF_LOCAL flag (in IS_ISI) if your program is not a host control
  // system, to make sure your buttons do not conflict with any buttons sent by the host.

  // LFS can display normal buttons in these four screens :

  // - main entry screen
  // - game setup screen
  // - in game
  // - SHIFT+U mode

  // To delete one button or clear all buttons, send this packet :

  /// <summary>
  /// Button FunctioN - delete buttons / receive button requests
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_BFN : IClientLfsInSimPacket
  {
    public byte Size;		// 8
    public Enums.ISP Type;		// ISP_BFN
    public byte ReqI;		// 0
    public Enums.BFN SubT;		// subtype, from BFN_ enumeration (see below)

    public byte UCID;		// connection to send to or from (0 = local / 255 = all)
    public byte ClickID;	// ID of button to delete (if SubT is BFN_DEL_BTN)
    public byte Inst;		// used internally by InSim
    public byte Sp3;

    public IS_BFN(Enums.BFN type, byte ucid, byte buttonId)
    {
      Size = 8;
      Type = Enums.ISP.BFN;
      ReqI = 0;
      SubT = type;

      UCID = ucid;
      ClickID = buttonId;
      Inst = 0;
      Sp3 = 0;
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
      return PacketFactory.GetBytesSequentially(this, Size);
    }

    #endregion
  }
}
