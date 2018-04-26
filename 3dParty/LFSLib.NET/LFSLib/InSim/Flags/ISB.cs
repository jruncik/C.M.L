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
  [Flags()]
  enum ISB : byte
  {
    // BStyle byte : style flags for the button

    //C1 = 1,		// you can choose a standard
    //C2 = 2,		// interface colour using
    //C4 = 4,		// these 3 lowest bits - see below
    COLOR_OPP = 7,
    CLICK = 8,		// click this button to send IS_BTC
    LIGHT = 16,		// light button
    DARK = 32,		// dark button
    LEFT = 64,		// align text to left
    RIGHT = 128,	// align text to right
  }
  // colour 0 : light grey		(not user editable)
  // colour 1 : title colour		(default:yellow)
  // colour 2 : unselected text	(default:black)
  // colour 3 : selected text		(default:white)
  // colour 4 : ok				(default:green)
  // colour 5 : cancel			(default:red)
  // colour 6 : text string		(default:pale blue)
  // colour 7 : unavailable		(default:grey)
}
