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

namespace FullMotion.LiveForSpeed.InSim.Events
{
	/// <summary>
	/// Camera Position Event
	/// </summary>
	public class CameraPosition : EventArgs
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private Packets.IS_CPP packet;
		#endregion

		#region Constructors ##########################################################################
    internal CameraPosition(Packets.IS_CPP packet)
		{
			this.packet = packet;
		}
		#endregion

		#region Properties ############################################################################
		/// <summary>
		/// Camera Position in the world
		/// </summary>
		public Vector Position
		{
			get	{ return new Vector(packet.Pos); }
		}
		/// <summary>
		/// Heading of the camera. 0 points along Y axis
		/// </summary>
		public ushort Heading
		{
			get {	return packet.H; }
		}
		/// <summary>
		/// Pitch of the camera. 0 means looking at horizon
		/// </summary>
		public ushort Pitch
		{
			get {	return packet.P; }
		}
		/// <summary>
		/// Roll of the camera. 0 means no roll
		/// </summary>
		public ushort Roll
		{
			get {	return packet.R; }
		}
		/// <summary>
		/// Player Index of car the camera is looking at. 0 is the pole car
		/// </summary>
		public byte ViewPlayer
		{
			get {	return packet.ViewPLID; }
		}
		/// <summary>
		/// Type of Camera
		/// </summary>
		public Enums.View Camera
		{
			get {	return packet.InGameCam; }
		}
		/// <summary>
		/// Field Of View in radians
		/// </summary>
		public float FOV
		{
			get {	return packet.FOV; }
		}
		/// <summary>
		/// Time to get to position. 0 means instantaneously and reset
		/// </summary>
		public ushort Time
		{
			get {	return packet.Time; }
		}
		/// <summary>
		/// Camera in Free View/Autox view mode
		/// </summary>
		public bool InShiftUMode
		{
      get { return (packet.StateFlags & Flags.ISS.SHIFTU) == Flags.ISS.SHIFTU; }
		}
		/// <summary>
		/// Camera in High View Mode
		/// </summary>
		public bool IsHighView
		{
      get { return (packet.StateFlags & Flags.ISS.SHIFTU_HIGH) == Flags.ISS.SHIFTU_HIGH; }
		}
		/// <summary>
		/// Camera in Follow Mode
		/// </summary>
		public bool IsFollowing
		{
      get { return (packet.StateFlags & Flags.ISS.SHIFTU_FOLLOW) == Flags.ISS.SHIFTU_FOLLOW; }
		}
		/// <summary>
		/// Camera is overriding User View
		/// </summary>
		public bool OverrideUserView
		{
      get { return (packet.StateFlags & Flags.ISS.VIEW_OVERRIDE) == Flags.ISS.VIEW_OVERRIDE; }
		}
		#endregion

		#region Methods ###############################################################################
		/// <summary>
		/// Get a CameraPositionInfo object based on the CameraPositionInfo returned from LFS
		/// </summary>
		/// <returns></returns>
		public CameraPositionInfo GetCameraPositionInfo()
		{
			return new CameraPositionInfo(this.packet);
		}
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
