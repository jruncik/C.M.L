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
  // Setting states

  // These states can be set by a special packet :

  // ISS_SHIFTU_FOLLOW	- following car
  // ISS_SHIFTU_NO_OPT	- SHIFT+U buttons hidden
  // ISS_SHOW_2D			- showing 2d display
  // ISS_MPSPEEDUP		- multiplayer speedup option
  // ISS_SOUND_MUTE		- sound is switched off

  /// <summary>
  /// State Flags Pack
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_SFP : IClientLfsInSimPacket
  {
    public byte Size;		// 8
    public Enums.ISP Type;		// ISP_SFP
    public byte ReqI;		// 0
    public byte Zero;

    public Flags.ISS Flag;		// the state to set
    public byte OffOn;		// 0 = off / 1 = on
    public byte Sp3;		// spare

    public IS_SFP(Flags.ISS flag, bool on)
    {
      Size = 8;
      Type = Enums.ISP.SFP;
      ReqI = 0;
      Zero = 0;
      Flag = flag;
      OffOn = (byte)((on) ? 1 : 0);
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

  // Other states must be set by using keypresses or messages (see below)


}
