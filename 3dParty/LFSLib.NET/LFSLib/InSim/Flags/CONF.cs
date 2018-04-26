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

namespace FullMotion.LiveForSpeed.InSim.Flags
{
  // Confirmation flags
  [Flags()]
  enum CONF : byte
  {
    MENTIONED = 1,
    CONFIRMED = 2,
    PENALTY_DT = 4,
    PENALTY_SG = 8,
    PENALTY_30 = 16,
    PENALTY_45 = 32,
    DID_NOT_PIT = 64,

    DISQ	=(PENALTY_DT | PENALTY_SG | DID_NOT_PIT),
    TIME	=(PENALTY_30 | PENALTY_45),
  }
}
