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
  // Pit Work Flags

  [Flags()]
  enum PSE : uint
  {
    NOTHING = 1,		// bit 0 (1)
    STOP = 2,			// bit 1 (2)
    FR_DAM = 4,			// bit 2 (4)
    FR_WHL = 8,			// etc...
    LE_FR_DAM = 16,
    LE_FR_WHL = 32,
    RI_FR_DAM = 64,
    RI_FR_WHL = 128,
    RE_DAM = 256,
    RE_WHL = 512,
    LE_RE_DAM = 1024,
    LE_RE_WHL = 2048,
    RI_RE_DAM = 4096,
    RI_RE_WHL = 8192,
    BODY_MINOR = 16384,
    BODY_MAJOR = 32768,
    SETUP = 65536,
    REFUEL = 131072,
  }
}
