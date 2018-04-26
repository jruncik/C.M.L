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
  // If the TypeIn byte is set in IS_BTN the user can type text into the button
  // In that case no IS_BTC is sent - an IS_BTT is sent when the user presses ENTER

  /// <summary>
  /// BuTton Type - sent back when user types into a text entry button
  /// </summary>
  struct IS_BTT : ILfsInSimPacket
  {
    public byte Size;		// 104
    public Enums.ISP Type;		// ISP_BTT
    public byte ReqI;		// ReqI as received in the IS_BTN
    public byte UCID;		// connection that typed into the button (zero if local)

    public byte ClickID;	// button identifier originally sent in IS_BTN
    public byte Inst;		// used internally by InSim
    public byte TypeIn;		// from original button specification
    public byte Sp3;

    public byte[] Text; //[96];	// typed text, zero to TypeIn specified in IS_BTN

    public IS_BTT(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      UCID = bytes[position++];
      ClickID = bytes[position++];
      Inst = bytes[position++];
      TypeIn = bytes[position++];
      position++;
      Sp3 = 0;
      Text = new byte[96];
      Array.Copy(bytes, position, Text, 0, 96);
    }

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }
}
