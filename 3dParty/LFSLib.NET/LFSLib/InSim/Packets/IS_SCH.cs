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
  /// <summary>
  /// Single CHaracter - send to simulate single character
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_SCH : IClientLfsInSimPacket
  {
    public byte Size;		// 8
    public Enums.ISP Type;		// ISP_SCH
    public byte ReqI;		// 0
    public byte Zero;

    public byte CharB;		// key to press
    public Flags.KeyModifier KeyModifiers;		// bit 0 : SHIFT / bit 1 : CTRL
    public byte Spare2;
    public byte Spare3;

    public IS_SCH(KeyPress keyPress)
    {
      Size = 8;
      Type = Enums.ISP.SCH;
      ReqI = 0;
      Zero = 0;
      CharB = (byte)keyPress.Key;
      KeyModifiers = 0;
      KeyModifiers = (keyPress.CtrlOn) ? KeyModifiers | Flags.KeyModifier.Ctrl : KeyModifiers & ~Flags.KeyModifier.Ctrl;
      KeyModifiers = (keyPress.ShiftOn) ? KeyModifiers | Flags.KeyModifier.Shift : KeyModifiers & ~Flags.KeyModifier.Shift;
      Spare2 = 0;
      Spare3 = 0;
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
