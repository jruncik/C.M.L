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
using System.Runtime.InteropServices;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim.Packets
{
  // STATE REPORTING AND REQUESTS
  // ============================

  // LFS will send a StatePack any time the info in the StatePack changes.

  /// <summary>
  /// STAte
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_STA : ILfsInSimPacket
  {
    public byte Size;			// 28
    public Enums.ISP Type;			// ISP_STA
    public byte ReqI;			// ReqI if replying to a request packet
    public byte Zero;

    public float ReplaySpeed;	// 4-byte float - 1.0 is normal speed

    public Flags.ISS Flags;			// ISS state flags (see below)
    public Enums.View InGameCam;		// Which type of camera is selected (see below)
    public byte ViewPLID;		// Unique ID of viewed player (0 = none)

    public byte NumP;			// Number of players in race
    public byte NumConns;		// Number of connections including host
    public byte NumFinished;	// Number finished or qualified
    public Enums.RaceState RaceInProg;		// 0 - no race / 1 - race / 2 - qualifying

    public byte QualMins;
    public byte RaceLaps;		// see "RaceLaps" near the top of this document
    public byte Spare2;
    public byte Spare3;

    public Support.Char6 Track; //[6];		// short name for track e.g. FE2R
    public Enums.Weather Weather;		// 0,1,2...
    public Enums.Wind Wind;			// 0=off 1=weak 2=strong

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

  // InGameCam is the in-game selected camera mode (which is
  // still selected even if LFS is actually in SHIFT+U mode).
  // For InGameCam's values, see "View identifiers" below.

  // To request a StatePack at any time, send this IS_TINY :

  // ReqI : non-zero		(returned in the reply)
  // SubT : TINY_SST		(Send STate)
}
