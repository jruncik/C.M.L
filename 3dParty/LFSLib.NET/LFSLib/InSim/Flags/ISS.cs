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
  // ISS state flags
  [Flags()]
  enum ISS : ushort
  {
    GAME = 1,             // in game (or MPR)
    REPLAY = 2,           // in SPR
    PAUSED = 4,           // paused
    SHIFTU = 8,           // SHIFT+U mode
    SHIFTU_HIGH = 16,     // HIGH view
    SHIFTU_FOLLOW = 32,   // following car
    SHIFTU_NO_OPT = 64,   // SHIFT+U buttons hidden
    SHOW_2D = 128,        // showing 2d display
    FRONT_END = 256,      // entry screen
    MULTI = 512,          // multiplayer mode
    MPSPEEDUP = 1024,     // multiplayer speedup option
    WINDOWED = 2048,      // LFS is running in a window
    SOUND_MUTE = 4096,    // sound is switched off
    VIEW_OVERRIDE = 8192, // override user view
    VISIBLE = 16384,      // InSim buttons visible
  }
}
