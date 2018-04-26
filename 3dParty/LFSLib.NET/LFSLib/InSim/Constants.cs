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

namespace FullMotion.LiveForSpeed.InSim
{
  class Constants
  {

    // The recommended area for most buttons is defined by :

    public const byte IS_X_MIN = 0;
    public const byte IS_X_MAX = 110;

    public const byte IS_Y_MIN = 30;
    public const byte IS_Y_MAX = 170;
  
    // If you draw buttons in this area, the area will be kept clear to
    // avoid overlapping LFS buttons with your InSim program's buttons.
    // Buttons outside that area will not have a space kept clear.
    // You can also make buttons visible in all screens - see below.

  }
}
