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

namespace FullMotion.LiveForSpeed
{
	/// <summary>
	/// Current Version of the Library
	/// </summary>
	public class LibVersion
	{
		/// <summary>
		/// The version number, currently 0.18b
		/// </summary>
		public const string NUM = "0.18b";

		/// <summary>
		/// The release date of this version, currently 02/28/2008
		/// </summary>
		public const string DATE = "02/28/2008";

		/// <summary>
		/// The LFS InSim Version Serial number this Library is meant to interact with, currently 4
		/// </summary>
		public const ushort INSIM_SERIAL = 4;
	}
}
