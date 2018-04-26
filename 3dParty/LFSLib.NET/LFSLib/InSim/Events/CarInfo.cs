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
using log4net;
using FullMotion.LiveForSpeed.InSim.Packets.Support;

namespace FullMotion.LiveForSpeed.InSim.Events
{
	/// <summary>
	/// CarInfo is used by the <see cref="RaceTrackMultiCarInfo"/> Event to store information about
	/// each car
	/// </summary>
  public class CarInfo : EventArgs
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private Packets.Support.CompCar compCar;
		private Vector vector;
		#endregion

		#region Constructors ##########################################################################
    internal CarInfo(Packets.Support.CompCar compCar)
		{
			this.compCar = compCar;
			this.vector = new Vector(this.compCar.X,this.compCar.Y,this.compCar.Z);
		}
		#endregion

		#region Properties ############################################################################
    /// <summary>
    /// The lap the car is on
    /// </summary>
    public int Lap
    {
      get
      {
        return compCar.Lap;
      }
    }
    /// <summary>
    /// The node in the current lap the car is at
    /// </summary>
    public int Node
    {
      get
      {
        int node = compCar.Node;
        return node;
      }
    }
		/// <summary>
		/// The player's unique Id
		/// </summary>
		public byte PlayerId
		{
			get { return compCar.PLID; }
		}

		/// <summary>
		/// The location of the car on the Track (65536 = 1m)
		/// </summary>
		public Vector Location
		{
			get { return vector; }
		}

    /// <summary>
    /// Position of the car in the car
    /// </summary>
    public int RacePosition
    {
      get { return (int)compCar.Position; }
    }

		/// <summary>
		/// The speed of the car in meters/second
		/// </summary>
		public float Speed
		{
			get { return compCar.Speed * 100.0f/32768.0f; }
		}
		/// <summary>
		/// The direction of the Car's motion in degrees off the world y axis.
		/// Positive value is anti-clockwise
		/// </summary>
		public float Direction
		{
			get { return compCar.Direction*180/32768; }
		}
		/// <summary>
		/// The heading of the car's forward axis in degrees off the world y axis
		/// Positive value is anti-clockwise
		/// </summary>
		public float Heading
		{
			get { return compCar.Heading*180/32768; }
		}
		/// <summary>
		/// The rate of change in the heading as a signed degree measurement.
		/// 0 is no change in heading, otherwise the degrees per second anti-clockwise
		/// </summary>
		public float AngularVelocity
		{
			get { return compCar.AngVel*180/8192; }
		}
		#endregion
	}
}
