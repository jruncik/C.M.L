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

namespace FullMotion.LiveForSpeed.InSim.Packets.Support
{
  // CAR TRACKING PACKETS - car position info sent at constant intervals
  // ====================

  // IS_NLP - compact, all cars in 1 variable sized packet
  // IS_MCI - detailed, max 8 cars per variable sized packet

  // To receive IS_NLP or IS_MCI packets at a specified interval :

  // 1) Set the Interval field in the IS_ISI (InSimInit) packet
  // 2) Set one of the flags ISF_NLP or ISF_MCI in the IS_ISI packet

  // If ISF_NLP flag is set, one IS_NLP packet is sent...

  [StructLayout(LayoutKind.Sequential)]
  struct NodeLap // Car info in 6 bytes - there is an array of these in the NLP (below)
  {
    public const int SIZE = 6;

    public ushort Node;		// current path node
    public ushort Lap;		// current lap
    public byte PLID;		// player's unique id
    public byte Position;	// current race position : 0 = unknown, 1 = leader, etc...

    public NodeLap(byte[] bytes)
    {
      Int32 size = bytes.Length;
      IntPtr pnt = Marshal.AllocHGlobal(size);
      try
      {
        GCHandle pin = GCHandle.Alloc(pnt, GCHandleType.Pinned);
        try
        {
          Marshal.Copy(bytes, 0, pnt, size);
          this = (NodeLap)Marshal.PtrToStructure(pnt, typeof(NodeLap));
        }
        finally
        {
          pin.Free();
        }
      }
      finally
      {
        Marshal.FreeHGlobal(pnt);
      }
    }
  }

}
