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

namespace FullMotion.LiveForSpeed.InSim.Packets.Support
{
  class StateModHelper
  {
    public static StateModHelper FollowCar
    {
      get { return new StateModHelper(Flags.ISS.SHIFTU_FOLLOW); }
    }

    public static StateModHelper ShiftUButtonsHidden
    {
      get { return new StateModHelper(Flags.ISS.SHIFTU_NO_OPT); }
    }

    public static StateModHelper Show2D
    {
      get { return new StateModHelper(Flags.ISS.SHOW_2D); }
    }

    public static StateModHelper MultiplayerSpeedup
    {
      get { return new StateModHelper(Flags.ISS.MPSPEEDUP); }
    }

    public static StateModHelper Mute
    {
      get { return new StateModHelper(Flags.ISS.SOUND_MUTE); }
    }

    public Flags.ISS flag;

    private StateModHelper(Flags.ISS flag)
    {
      this.flag = flag;
    }

    public Flags.ISS Flag { get { return flag; } }
  }
}
