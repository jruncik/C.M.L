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
using FullMotion.LiveForSpeed.InSim.Packets.Support;

namespace FullMotion.LiveForSpeed.InSim
{
	/// <summary>
	/// A simple 3D Vector class
	/// </summary>
	public class Vector
	{
		#region Member Variables ######################################################################
		private Vec vec;
		#endregion

		#region Constructors ##########################################################################
		internal Vector(Vec vec)
		{
			this.vec = vec;
		}
		/// <summary>
		/// Create a new vector object
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector(int x, int y, int z)
		{
			this.vec = new Vec();
			vec.X = x;
			vec.Y = y;
			vec.Z = z;
		}
		#endregion

		#region Properties ############################################################################
		/// <summary>
		/// X axis position in the game world. 65536 = 1 meter
		/// </summary>
		public int X
		{
			get { return vec.X; }
		}
		/// <summary>
		/// Y axis position in the game world. 65536 = 1 meter
		/// </summary>
		public int Y
		{
			get { return vec.Y; }
		}
		/// <summary>
		/// Z axis position in the game world. 65536 = 1 meter
		/// </summary>
		public int Z
		{
			get { return vec.Z; }
		}

		internal Vec Vec
		{
			get { return this.vec; }
		}
		#endregion
	}
}
