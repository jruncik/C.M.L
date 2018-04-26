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
  /// <summary>
  /// Type of Tyre in use
  /// </summary>
  public enum Tyre : byte
  {
    /// <summary>
    /// R1 Compound
    /// </summary>
    R1,			// 0
    /// <summary>
    /// R2 Compound
    /// </summary>
    R2,			// 1
    /// <summary>
    /// R3 Compound
    /// </summary>
    R3,			// 2
    /// <summary>
    /// R4 Compound
    /// </summary>
    R4,			// 3
    /// <summary>
    /// High Performance Road Car Tyre
    /// </summary>
    RoadSuper,	// 4
    /// <summary>
    /// Regular Road Car Tyre
    /// </summary>
    RoadNormal,	// 5
    /// <summary>
    /// Combination Off- and On-road Type
    /// </summary>
    Hybrid,		// 6
    /// <summary>
    /// Off-Road Tyre
    /// </summary>
    Knobbly,		// 7
    //Num,
    /// <summary>
    /// Tyre was not changed from last state
    /// </summary>
    NotChanged = 255,
  }
  //const int NOT_CHANGED = 255;
}
