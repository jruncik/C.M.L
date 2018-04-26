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
  // SCREEN MODE
  // ===========

  // You can send this packet to LFS to set the screen mode :

  /// <summary>
  /// MODe : send to LFS to change screen mode
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_MOD : IClientLfsInSimPacket
  {
    public byte Size;		// 20
    public Enums.ISP Type;		// ISP_MOD
    public byte ReqI;		// 0
    public byte Zero;

    public int Bits16;		// set to choose 16-bit
    public int RR;			// refresh rate - zero for default
    public int Width;		// 0 means go to window
    public int Height;		// 0 means go to window

    public IS_MOD(int width, int height, int refreshRate, bool screenDepth16)
    {
      Size = 20;
      Type = Enums.ISP.MOD;
      ReqI = 0;
      Zero = 0;
      Bits16 = (screenDepth16) ? 1 : 0;
      RR = refreshRate;
      Width = width;
      Height = height;
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

  // The refresh rate actually selected by LFS will be the highest available rate
  // that is less than or equal to the specified refresh rate.  Refresh rate can
  // be specified as zero in which case the default refresh rate will be used.

  // If Width and Height are both zero, LFS will switch to windowed mode.


}
