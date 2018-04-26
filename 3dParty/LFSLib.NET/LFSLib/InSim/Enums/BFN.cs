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

namespace FullMotion.LiveForSpeed.InSim.Enums
{
  enum BFN : byte// the fourth byte of IS_BFN packets is one of these
  {
    DEL_BTN,		//  0 - instruction     : delete one button (must set ClickID)
    CLEAR,			//  1 - instruction		: clear all buttons made by this insim instance
    USER_CLEAR,		//  2 - info            : user cleared this insim instance's buttons
    REQUEST,		//  3 - user request    : SHIFT+B or SHIFT+I - request for buttons
  };

  // NOTE : BFN_REQUEST allows the user to bring up buttons with SHIFT+B or SHIFT+I
}
