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
  // Penalty reasons

  /// <summary>
  /// Reason for assessed Penalty
  /// </summary>
  public enum PenaltyReason : byte
  {
    /// <summary>
    /// unknown or cleared penalty
    /// </summary>
    Unknown,		// 0 - unknown or cleared penalty
    /// <summary>
    /// penalty given by admin
    /// </summary>
    Admin,			// 1 - penalty given by admin
    /// <summary>
    /// wrong way driving
    /// </summary>
    WrongWay,		// 2 - wrong way driving
    /// <summary>
    /// starting before green light
    /// </summary>
    FalseStart,	// 3 - starting before green light
    /// <summary>
    /// speeding in pit lane
    /// </summary>
    Speeding,		// 4 - speeding in pit lane
    /// <summary>
    /// stop-go pit stop too short
    /// </summary>
    StopShort,	// 5 - stop-go pit stop too short
    /// <summary>
    /// compulsory stop is too late
    /// </summary>
    StopLate,		// 6 - compulsory stop is too late
    //Num
  }
}
