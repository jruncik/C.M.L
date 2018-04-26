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
  // Penalty values (VALID means the penalty can now be cleared)

  /// <summary>
  /// Penalty issued by Race Control
  /// </summary>
  public enum Penalty : byte
  {
    /// <summary>
    /// No Penalty
    /// </summary>
    None,		// 0		
    /// <summary>
    /// Drive Through Penalty
    /// </summary>
    DriveThrough,			// 1
    /// <summary>
    /// Drive Through Penalty (has been completed)
    /// </summary>
    DriveThroughValid,	// 2
    /// <summary>
    /// Stop And Go Penalty
    /// </summary>
    StopGo,			// 3
    /// <summary>
    /// Stop And Go Penalty (has been completed)
    /// </summary>
    StopGoValid,	// 4
    /// <summary>
    /// 30 Second Time Penalty
    /// </summary>
    Time30,			// 5
    /// <summary>
    /// 45 Second Time Penalty
    /// </summary>
    time45,			// 6
    //Num
  }
}
