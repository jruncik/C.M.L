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
  /// Type of Lap Information
  /// </summary>
  public enum LapInfoType
  {
    /// <summary>
    /// Actual Lap Count
    /// </summary>
    Lap,
    /// <summary>
    /// Race Length in Hours
    /// </summary>
    Hour,
    /// <summary>
    /// Practice Session
    /// </summary>
    Practice
  }

  /// <summary>
  /// LapInfo encapsulates the race setups' definition of the race length
  /// </summary>
  public class LapInfo
  {
    int value = 0;
    LapInfoType type;

    internal LapInfo(ushort lap)
    {
      if (lap == 0)
      {
        type = LapInfoType.Practice;
      }
      else if (lap < 100)
      {
        value = lap;
        type = LapInfoType.Lap;
      }
      else if (lap < 191)
      {
        value = (lap - 100) * 10 + 100;
        type = LapInfoType.Lap;
      }
      else
      {
        value = lap - 190;
        type = LapInfoType.Hour;
      }
    }

    /// <summary>
    /// Type of information represented by <see cref="Value"/>
    /// </summary>
    public LapInfoType Type { get { return type; } }

    /// <summary>
    /// Value of the Lap Info Object
    /// </summary>
    public int Value { get { return value; } }
  }
}
