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

namespace FullMotion.LiveForSpeed.InSim.Enums
{
	/// <summary>
	/// Wind sent in StatePack
	/// </summary>
	public enum Wind : byte
	{
		/// <summary>
		/// No Wind
		/// </summary>
		Off = 0,
		/// <summary>
		/// Weak Wind
		/// </summary>
		Weak = 1,
		/// <summary>
		/// Strong Wind
		/// </summary>
		Strong = 2,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWind3 = 3,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWind4 = 4,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWind5 = 5,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWind6 = 6,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWind7 = 7,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWind8 = 8
	}
}
