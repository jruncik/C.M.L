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

namespace FullMotion.LiveForSpeed.InSim.Flags
{
  // Player flags
  [Flags()]
  enum PIF : ushort
  {
    SWAPSIDE = 1,
    GC_CUT = 2,
    GC_BLIP = 4,
    AUTOGEARS = 8,
    SHIFTER = 16,
    RESERVED = 32,
    HELP_B = 64,
    AXIS_CLUTCH = 128,
    INPITS = 256,
    AUTOCLUTCH = 512,
    MOUSE = 1024,
    KB_NO_HELP = 2048,
    KB_STABILISED = 4096,
    CUSTOM_VIEW = 8192,
  }
}
