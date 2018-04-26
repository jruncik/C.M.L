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
  // DIRECT camera control
  // ---------------------

  // A Camera Position Packet can be used for LFS to report a camera position and state.
  // An InSim program can also send one to set LFS camera position in game or SHIFT+U mode.

  // Type : "Vec" : 3 ints (X, Y, Z) - 65536 means 1 metre

  /// <summary>
  /// Cam Pos Pack - Full camera packet (in car OR SHIFT+U mode)
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_CPP : IClientLfsInSimPacket
  {
    public byte Size;		// 32
    public Enums.ISP Type;		// ISP_CPP
    public byte ReqI;		// instruction : 0 / or reply : ReqI as received in the TINY_SCP
    public byte Zero;

    public Support.Vec Pos;		// Position vector

    public ushort H;			// heading - 0 points along Y axis
    public ushort P;			// pitch   - 0 means looking at horizon
    public ushort R;			// roll    - 0 means no roll

    public byte ViewPLID;	// Unique ID of viewed player (0 = none)
    public Enums.View InGameCam;	// InGameCam (as reported in StatePack)

    public float FOV;		// 4-byte float : FOV in radians

    public ushort Time;		// Time to get there (0 means instant + reset)
    public Flags.ISS StateFlags;		// ISS state flags (see below)

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

  // The ISS state flags that can be set are :

  // ISS_SHIFTU			- in SHIFT+U mode
  // ISS_SHIFTU_HIGH		- HIGH view
  // ISS_SHIFTU_FOLLOW	- following car
  // ISS_VIEW_OVERRIDE	- override user view

  // On receiving this packet, LFS will set up the camera to match the values in the packet,
  // including switching into or out of SHIFT+U mode depending on the ISS_SHIFTU flag.

  // If ISS_SHIFTU is not set, then ViewPLID and InGameCam will be used.

  // If ISS_VIEW_OVERRIDE is set, the in-car view Heading Pitch and Roll will be taken
  // from the values in this packet.  Otherwise normal in-game control will be used.

  // Position vector (Vec Pos) - in SHIFT+U mode, Pos can be either relative or absolute.

  // If ISS_SHIFTU_FOLLOW is set, it's a following camera, so the position is relative to
  // the selected car.  Otherwise, the position is absolute, as used in normal SHIFT+U mode.

  // NOTE : Set InGameCam or ViewPLID to 255 to leave that option unchanged.}
}
