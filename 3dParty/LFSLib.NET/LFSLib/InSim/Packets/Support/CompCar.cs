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
  [StructLayout(LayoutKind.Sequential)]
  struct CompCar // Car info in 28 bytes - there is an array of these in the MCI (below)
  {
    public const int SIZE = 28;

    public ushort Node;		// current path node
    public ushort Lap;		// current lap
    public byte PLID;		// player's unique id
    public byte Position;	// current race position : 0 = unknown, 1 = leader, etc...
    public Flags.CCI Info;		// flags and other info - see below
    public byte Sp3;
    public int X;			// X map (65536 = 1 metre)
    public int Y;			// Y map (65536 = 1 metre)
    public int Z;			// Z alt (65536 = 1 metre)
    public ushort Speed;		// speed (32768 = 100 m/s)
    public ushort Direction;	// direction of car's motion : 0 = world y direction, 32768 = 180 deg
    public ushort Heading;	// direction of forward axis : 0 = world y direction, 32768 = 180 deg
    public short AngVel;		// signed, rate of change of heading : (16384 = 360 deg/s)

    public CompCar(byte[] bytes)
    {
      Int32 size = bytes.Length;
      IntPtr pnt = Marshal.AllocHGlobal(size);
      try
      {
        GCHandle pin = GCHandle.Alloc(pnt, GCHandleType.Pinned);
        try
        {
          Marshal.Copy(bytes, 0, pnt, size);
          this = (CompCar)Marshal.PtrToStructure(pnt, typeof(CompCar));
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

  // NOTE 1) Info byte - the bits in this byte have the following meanings :
  // NOTE 2) Heading : 0 = world y axis direction, 32768 = 180 degrees, anticlockwise from above
  // NOTE 3) AngVel  : 0 = no change in heading,    8192 = 180 degrees per second anticlockwise
}
