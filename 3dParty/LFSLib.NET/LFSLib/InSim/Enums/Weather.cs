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
	/// In Game Weather
	/// </summary>
	public enum Weather : byte
	{
		/// <summary>
		/// Day
		/// </summary>
		Day = 0,
		/// <summary>
		/// Evening
		/// </summary>
		Evening = 1,
		/// <summary>
		/// Dusk Overcast
		/// </summary>
		DuskOvercast = 2,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWeather3 = 3,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWeather4 = 4,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWeather5 = 5,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWeather6 = 6,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWeather7 = 7,
    ///// <summary>
    ///// Spare, so we don't throw an exception if LFS adds another before we notice
    ///// </summary>
    //SpareWeather8 = 8,
	}
}
