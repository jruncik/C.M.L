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
using log4net;

namespace FullMotion.LiveForSpeed.InSim
{
  /// <summary>
  /// Camera Position Event
  /// </summary>
  public class CameraPositionInfo
  {
    #region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

    #region Member Variables ######################################################################
    private Packets.IS_CPP packet;
    #endregion

    #region Constructors ##########################################################################
    internal CameraPositionInfo()
    {
      this.packet = new Packets.IS_CPP();
      this.packet.Size = 32;
      this.packet.Type = Enums.ISP.CPP;
    }

    internal CameraPositionInfo(Packets.IS_CPP packet)
    {
      this.packet = packet;
    }
    #endregion

    #region Properties ############################################################################
    /// <summary>
    /// Camera Position in the world if in <see cref="InShiftUMode"/> and <see cref="IsFollowing"/>
    /// is false. If <see cref="IsFollowing"/> is true, the position is relative to the car.
    /// </summary>
    public Vector Position
    {
      get { return new Vector(packet.Pos); }
      set { packet.Pos = value.Vec; }
    }
    /// <summary>
    /// Heading of the camera. 0 points along Y axis
    /// </summary>
    public ushort Heading
    {
      get { return packet.H; }
      set { packet.H = value; }
    }
    /// <summary>
    /// Pitch of the camera. 0 means looking at horizon
    /// </summary>
    public ushort Pitch
    {
      get { return packet.P; }
      set { packet.P = value; }
    }
    /// <summary>
    /// Roll of the camera. 0 means no roll
    /// </summary>
    public ushort Roll
    {
      get { return packet.R; }
      set { packet.R = value; }
    }
    /// <summary>
    /// Player Id of car the camera is looking at. 0 is the pole car
    /// </summary>
    public byte ViewPlayerId
    {
      get { return packet.ViewPLID; }
      set { packet.ViewPLID = value; }
    }
    /// <summary>
    /// Type of Camera
    /// </summary>
    public Enums.View Camera
    {
      get { return packet.InGameCam; }
      set { packet.InGameCam = value; }
    }
    /// <summary>
    /// Field Of View in radians
    /// </summary>
    public float FOV
    {
      get { return packet.FOV; }
      set { packet.FOV = value; }
    }
    /// <summary>
    /// Time to get to position. 0 means instantaneously and reset
    /// </summary>
    public ushort Time
    {
      get { return packet.Time; }
      set { packet.Time = value; }
    }
    /// <summary>
    /// Camera in Free View/Autox view mode
    /// </summary>
    public bool InShiftUMode
    {
      get { return (packet.StateFlags & Flags.ISS.SHIFTU) == Flags.ISS.SHIFTU; }
      set { packet.StateFlags = (value) ? packet.StateFlags | Flags.ISS.SHIFTU : packet.StateFlags & ~Flags.ISS.SHIFTU; }
    }
    /// <summary>
    /// Camera in High View Mode (has no effect if <see cref="InShiftUMode"/> is false)
    /// </summary>
    public bool IsHighView
    {
      get { return (packet.StateFlags & Flags.ISS.SHIFTU_HIGH) == Flags.ISS.SHIFTU_HIGH; }
      set { packet.StateFlags = (value) ? packet.StateFlags | Flags.ISS.SHIFTU_HIGH : packet.StateFlags & ~Flags.ISS.SHIFTU_HIGH; }
    }
    /// <summary>
    /// Camera in Follow Mode (has no effect if <see cref="InShiftUMode"/> is false)
    /// </summary>
    public bool IsFollowing
    {
      get { return (packet.StateFlags & Flags.ISS.SHIFTU_FOLLOW) == Flags.ISS.SHIFTU_FOLLOW; }
      set { packet.StateFlags = (value) ? packet.StateFlags | Flags.ISS.SHIFTU_FOLLOW : packet.StateFlags & ~Flags.ISS.SHIFTU_FOLLOW; }
    }
    /// <summary>
    /// Camera is overriding User View
    /// </summary>
    public bool OverrideUserView
    {
      get { return (packet.StateFlags & Flags.ISS.VIEW_OVERRIDE) == Flags.ISS.VIEW_OVERRIDE; }
      set { packet.StateFlags = (value) ? packet.StateFlags | Flags.ISS.VIEW_OVERRIDE : packet.StateFlags & ~Flags.ISS.VIEW_OVERRIDE; }
    }
    #endregion

    #region Methods ###############################################################################
    internal Packets.IClientLfsInSimPacket GetPacket()
    {
      return packet;
    }
    #endregion

    #region Private Methods #######################################################################
    #endregion
  }
}
