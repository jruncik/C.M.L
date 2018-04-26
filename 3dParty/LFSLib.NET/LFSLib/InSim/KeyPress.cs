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

namespace FullMotion.LiveForSpeed.InSim
{
	/// <summary>
	/// A keypress object to send to LFS
	/// </summary>
	public class KeyPress
	{
		#region Member Variables ######################################################################
		private char key = '\0';
		private bool ctrlOn = false;
		private bool shiftOn = false;
		#endregion

		#region Constructors ##########################################################################
		/// <summary>
		/// Create a new keypress without any modifier keys
		/// </summary>
		/// <param name="key"></param>
		public KeyPress(char key)
		{
			this.key = key;
		}

		/// <summary>
		/// Create a blank keypress (useful if one keypress object is to be re-used
		/// </summary>
		public KeyPress()
		{
		}

		/// <summary>
		/// Create a fully defined key press object
		/// </summary>
		/// <param name="key"></param>
		/// <param name="ctrlOn"></param>
		/// <param name="shiftOn"></param>
		public KeyPress(char key, bool ctrlOn, bool shiftOn)
		{
			this.key = key;
			this.ctrlOn = ctrlOn;
			this.shiftOn = shiftOn;
		}
		#endregion

		#region Properties ############################################################################
		/// <summary>
		/// The key to be sent
		/// </summary>
		public char Key
		{
			get { return key; }
			set { key = value; }
		}
		/// <summary>
		/// Is the Control Key pressed?
		/// </summary>
		public bool CtrlOn
		{
			get { return ctrlOn; }
			set { ctrlOn = value; }
		}
		/// <summary>
		/// Is the shift key pressed?
		/// </summary>
		public bool	ShiftOn
		{
			get { return shiftOn; }
			set { shiftOn = value; }
		}
		#endregion
	}
}
