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
  /// <summary>
  /// InSim Init - packet to initialise the InSim system
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  struct IS_ISI : IClientLfsInSimPacket
  {
    public byte Size;		// 44
    public Enums.ISP Type;		// ISP_ISI
    public byte ReqI;		// If non-zero LFS will send an IS_VER packet
    public byte Zero;		// 0

    public ushort UDPPort;	// Port for UDP replies from LFS (0 to 65535)
    public Flags.ISF Flags;		    // Bit flags for options (see below)

    public byte Sp0;		    // 0
    public byte Prefix;		  // Special host message prefix character
    public ushort Interval;	// Time in ms between NLP or MCI (0 = none)

    public Support.Char16 Admin;	//[16] // Admin password (if set in LFS)
    public Support.Char16 IName;	//[16] // A short name for your program

    public IS_ISI(ushort updPort, Flags.ISF flags, byte prefix, ushort interval, string adminPass, string progName)
    {
      Size = 44;
      Type = Enums.ISP.ISI;
      ReqI = PacketFactory.NextRequestId;
      Zero = 0;
      UDPPort = updPort;
      Flags = flags;
      Sp0 = 0;
      Prefix = prefix;
      Interval = interval;
      Admin = new Support.Char16(adminPass);
      IName = new Support.Char16(progName);
    }
    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion

    #region IClientLfsInSimPacket Members

    public byte[] GetBytes()
    {
      return PacketFactory.GetBytesSequentially(this, Size);
    }

    public byte RequestId
    {
      get { return ReqI; }
    }

    #endregion
  }

  // NOTE 1) UDPPort field when you connect using UDP :

  // zero     : LFS sends all packets to the port of the incoming packet
  // non-zero : LFS sends all packets to the specified UDPPort

  // NOTE 2) UDPPort field when you connect using TCP :

  // zero     : LFS sends NLP / MCI packets using your TCP connection
  // non-zero : LFS sends NLP / MCI packets to the specified UDPPort

  // NOTE 3) Flags field ISF

  // In most cases you should not set both ISF_NLP and ISF_MCI flags
  // because all IS_NLP information is included in the IS_MCI packet.

  // The ISF_LOCAL flag is important if your program creates buttons.
  // It should be set if your program is not a host control system.
  // If set, then buttons are created in the local button area, so
  // avoiding conflict with the host buttons and allowing the user
  // to switch them with SHIFT+B rather than SHIFT+I.

  // NOTE 4) Prefix field, if set when initialising InSim on a host :

  // Messages typed with this prefix will be sent to your InSim program
  // on the host (in IS_MSO) and not displayed on anyone's screen.


  // CLOSING InSim
  // =============

  // You can send this IS_TINY to close the InSim connection to your program :

  // ReqI : 0
  // SubT : TINY_CLOSE	(close this connection)

  // Another InSimInit packet is then required to start operating again.

  // You can shut down InSim completely and stop it listening at all by typing /insim=0
  // into LFS (or send a MsgTypePack to do the same thing).


  // MAINTAINING THE CONNECTION - IMPORTANT
  // ==========================

  // If InSim does not receive a packet for 70 seconds, it will close your connection.
  // To open it again you would need to send another InSimInit packet.

  // LFS will send a blank IS_TINY packet like this every 30 seconds :

  // ReqI : 0
  // SubT : TINY_NONE		(keep alive packet)

  // You should reply with a blank IS_TINY packet :

  // ReqI : 0
  // SubT : TINY_NONE		(has no effect other than resetting the timeout)

  // NOTE : If you want to request a reply from LFS to check the connection
  // at any time, you can send this IS_TINY :

  // ReqI : non-zero		(returned in the reply)
  // SubT : TINY_PING		(request a TINY_REPLY)

  // LFS will reply with this IS_TINY :

  // ReqI : non-zero		(as received in the request packet)
  // SubT : TINY_REPLY	(reply to ping)


  // TRACKING PACKET REQUESTS
  // ========================

  // To request players, connections, results or a single NLP or MCI, send an IS_TINY

  // In each case, ReqI must be non-zero, and will be returned in the reply packet

  // SubT : TINT_NCN - request all connections
  // SubT : TINY_NPL - request all players
  // SubT : TINY_RES - request all results
  // SubT : TINY_NLP - request a single IS_NLP
  // SubT : TINY_MCI - request a set of IS_MCI






  // CAR POSITION PACKETS (Initialising OutSim from InSim - See "OutSim" below)
  // ====================

  // To request Car Positions from the currently viewed car, send this IS_SMALL :

  // ReqI : 0
  // SubT : SMALL_SSP		(Start Sending Positions)
  // UVal : interval		(time between updates - zero means stop sending)

  // The SSP packet makes LFS start sending UDP packets if in game, using the OutSim
  // system as documented near the end of this text file.

  // You do not need to set any OutSim values in LFS cfg.txt as OutSim is automatically
  // initialised by the SSP packet.

  // The OutSim packets will be sent to the UDP port specified in the InSimInit packet.

  // NOTE : OutSim packets are not InSim packets and don't have a 4-byte header.


  // DASHBOARD PACKETS (Initialising OutGauge from InSim - See "OutGauge" below)
  // =================

  // To request Dashboard Packets from the currently viewed car, send this IS_SMALL :

  // ReqI : 0
  // SubT : SMALL_SSG		(Start Sending Gauges)
  // UVal : interval		(time between updates - zero means stop sending)

  // The SSG packet makes LFS start sending UDP packets if in game, using the OutGauge
  // system as documented near the end of this text file.

  // You do not need to set any OutGauge values in LFS cfg.txt as OutSim is automatically
  // initialised by the SSG packet.

  // The OutGauge packets will be sent to the UDP port specified in the InSimInit packet.

  // NOTE : OutGauge packets are not InSim packets and don't have a 4-byte header.


  // CAMERA CONTROL
  // ==============


  // NOTE : Set InGameCam or ViewPLID to 255 to leave that option unchanged.



  // SMOOTH CAMERA POSITIONING
  // --------------------------

  // The "Time" value in the packet is used for camera smoothing.  A zero Time means instant
  // positioning.  Any other value (milliseconds) will cause the camera to move smoothly to
  // the requested position in that time.  This is most useful in SHIFT+U camera modes or
  // for smooth changes of internal view when using the ISS_VIEW_OVERRIDE flag.

  // NOTE : You can use frequently updated camera positions with a longer "Time" than your
  // updates.  For example, sending a camera position every 100 ms, with a Time value of
  // 1000 ms.  LFS will make a smooth motion from these "rough" inputs.

  // If the requested camera mode is different from the one LFS is already in, it cannot
  // move smoothly to the new position, so in this case the "Time" value is ignored.

  // GETTING A CAMERA PACKET
  // -----------------------

  // To GET a CamPosPack from LFS, send this IS_TINY :

  // ReqI : non-zero		(returned in the reply)
  // SubT : TINY_SCP		(Send Cam Pos)

  // LFS will reply with a CamPosPack as described above.  You can store this packet
  // and later send back exactly the same packet to LFS and it will try to replicate
  // that camera position.


  // TIME CONTROL
  // ============

  // You can Stop or Start Time in LFS and while it is stopped you can make LFS move
  // in time steps in multiples of 100th of a second.  Warning : unlike pausing, this
  // is a "trick" to LFS and the program is unaware of time passing, so you must not
  // leave it stopped because LFS is unusable in that state.  You must never use this
  // packet in multiplayer mode.

  // Request the current time at any point with this IS_TINY :

  // ReqI : non-zero		(returned in the reply)
  // SubT : TINY_GTH		(Get Time in Hundredths)

  // The time will be sent back in this IS_SMALL :

  // ReqI : non-zero		(as received in the request packet)
  // SubT : SMALL_RTP		(Race Time Packet)
  // UVal	: Time			(hundredths of a second since start of race or replay)

  // Stop and Start with this IS_SMALL :

  // ReqI : 0
  // SubT : SMALL_TMS		(TiMe Stop)
  // UVal	: stop			(1 - stop / 0 - carry on)

  // When STOPPED, make time step updates with this IS_SMALL :

  // ReqI : 0
  // SubT : SMALL_STP		(STeP)
  // UVal : number		(number of hundredths of a second to update)
}
