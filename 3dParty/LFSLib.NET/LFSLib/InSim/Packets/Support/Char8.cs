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
using FullMotion.LiveForSpeed.Util;

namespace FullMotion.LiveForSpeed.InSim.Packets.Support
{
  [StructLayout(LayoutKind.Sequential)]
  struct Char8
  {
    byte c0;
    byte c1;
    byte c2;
    byte c3;
    byte c4;
    byte c5;
    byte c6;
    byte c7;

    public Char8(string value)
    {
      byte[] bytes = CharHelper.GetBytes(value, 8, true);
      c0 = bytes[0];
      c1 = bytes[1];
      c2 = bytes[2];
      c3 = bytes[3];
      c4 = bytes[4];
      c5 = bytes[5];
      c6 = bytes[6];
      c7 = bytes[7];
    }
    public string Value
    {
      get
      {
        return CharHelper.GetString(new byte[] { c0, c1, c2, c3, c4, c5, c6, c7 });
      }
      set
      {
        byte[] bytes = CharHelper.GetBytes(value, 8, true);
        c0 = bytes[0];
        c1 = bytes[1];
        c2 = bytes[2];
        c3 = bytes[3];
        c4 = bytes[4];
        c5 = bytes[5];
        c6 = bytes[6];
        c7 = bytes[7];
      }
    }
  }
}
