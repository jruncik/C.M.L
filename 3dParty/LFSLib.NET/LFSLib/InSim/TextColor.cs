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
  /// <summary>
  /// Helper class for accessing LFS color control sequences for changing colors mid-string
  /// </summary>
  public class TextColor
  {
    /// <summary>
    /// Black
    /// </summary>
    public static readonly string Black = "^0";

    /// <summary>
    /// Red
    /// </summary>
    public static readonly string Red = "^1";
    /// <summary>
    /// Green
    /// </summary>
    public static readonly string Green = "^2";
    /// <summary>
    /// Yellow
    /// </summary>
    public static readonly string Yellow = "^3";
    /// <summary>
    /// Blue
    /// </summary>
    public static readonly string Blue = "^4";
    /// <summary>
    /// Cyan
    /// </summary>
    public static readonly string Cyan = "^5";
    /// <summary>
    /// Light Blue
    /// </summary>
    public static readonly string LightBlue = "^6";

    /// <summary>
    /// White
    /// </summary>
    public static readonly string White = "^7";

    /// <summary>
    /// Return to default text color
    /// </summary>
    public static readonly string Default = "^8";

    private TextColor()
    {
    }
  }
}