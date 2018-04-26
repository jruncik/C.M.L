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
  // User Values (for UserType byte)


  /// <summary>
  /// User Values
  /// </summary>
  public enum UserType : byte
  {
    /// <summary>
    /// System message
    /// </summary>
    System,			// 0 - system message
    /// <summary>
    /// Normal visible user message
    /// </summary>
    User,			// 1 - normal visible user message
    /// <summary>
    /// Hidden message starting with special prefix defined in InSim initialization
    /// </summary>
    Prefix,			// 2 - hidden message starting with special prefix (see ISI)
    /// <summary>
    /// Hidden message typed on local pc with /o command
    /// </summary>
    O,				// 3 - hidden message typed on local pc with /o command
    //Num
  }

  // NOTE : Typing "/o MESSAGE" into LFS will send an IS_MSO with UserType = MSO_O
}
