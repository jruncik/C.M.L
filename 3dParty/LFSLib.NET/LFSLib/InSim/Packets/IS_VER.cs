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
  // VERSION REQUEST
  // ===============

  // It is advisable to request version information as soon as you have connected, to
  // avoid problems when connecting to a host with a later or earlier version.  You will
  // be sent a version packet on connection if you set ReqI in the IS_ISI packet.

  // This version packet can be sent on request :

  /// <summary>
  /// VERsion
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_VER : ILfsInSimPacket
  {
    public byte Size;			// 20
    public Enums.ISP Type;			// ISP_VERSION
    public byte ReqI;			// ReqI as received in the request packet
    public byte Zero;

    public Support.Char8 Version; //[8];		// LFS version, e.g. 0.3G
    public Support.Char6 Product; //[6];		// Product : DEMO or S1
    public ushort InSimVer;		// InSim Version : increased when InSim packets change

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

  // To request an InSimVersion packet at any time, send this IS_TINY :

  // ReqI : non-zero		(returned in the reply)
  // SubT : TINY_VER		(request an IS_VER)
}
