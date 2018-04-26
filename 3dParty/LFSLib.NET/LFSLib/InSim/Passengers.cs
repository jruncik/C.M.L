/* ------------------------------------------------------------------------- *
 * Copyright (C) 2005-2008 Arne Claassen
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
  /// Information about the passenger load for a car
  /// </summary>
  public class Passengers
  {
    Flags.Passengers passengers;

    internal Passengers(Flags.Passengers passengers)
    {
      this.passengers = passengers;
    }

    /// <summary>
    /// Front Passenger
    /// </summary>
    public Enums.Passenger Front
    {
      get { return CalcPassenger(Flags.Passengers.FrontOccupied, Flags.Passengers.FrontFemale); }
    }

    /// <summary>
    /// Rear Left Passenger
    /// </summary>
    public Enums.Passenger RearLeft
    {
      get { return CalcPassenger(Flags.Passengers.RearLeft, Flags.Passengers.RearLeftFemale); }
    }

    /// <summary>
    /// Rear Middle Passenger
    /// </summary>
    public Enums.Passenger RearMiddle
    {
      get { return CalcPassenger(Flags.Passengers.RearMiddle, Flags.Passengers.RearMiddleFemale); }
    }

    /// <summary>
    /// Rear Right Passenger
    /// </summary>
    public Enums.Passenger RearRight
    {
      get { return CalcPassenger(Flags.Passengers.RearRight, Flags.Passengers.RearRightFemale); }
    }

    private Enums.Passenger CalcPassenger(Flags.Passengers occupied, Flags.Passengers female)
    {
      if ((passengers & occupied) == occupied)
      {
        if ((passengers & female) == female)
        {
          return Enums.Passenger.Female;
        }
        return Enums.Passenger.Male;
      }
      else
      {
        return Enums.Passenger.Empty;
      }
    }

  }
}
