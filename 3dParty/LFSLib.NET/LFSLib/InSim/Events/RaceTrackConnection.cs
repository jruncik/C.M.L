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
using System.Text;
using log4net;

namespace FullMotion.LiveForSpeed.InSim.Events
{
	/// <summary>
	/// Event describing a connection, sent either on new connect or when directly requested
	/// </summary>
  public class RaceTrackConnection : RaceTrackEvent
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
    private Packets.IS_NCN packet;
		#endregion

		#region Constructors ##########################################################################
    internal RaceTrackConnection(Packets.IS_NCN packet)
    {
      this.packet = packet;
      log.Debug("RaceTrackConnection event created");
    }
		#endregion

		#region Properties ############################################################################
    /// <summary>
    /// Id of the request that caused this event to be triggered. 0 if the event trigger was not
    /// manually requested
    /// </summary>
    public byte RequestId
    {
      get { return packet.ReqI; }
    }
		/// <summary>
		/// Is the Connection an Admin connection?
		/// </summary>
		public bool IsAdmin
		{
			get { return ( packet.Admin > 0 ) ? true : false; }
		}
    /// <summary>
    /// DEPRECATED: Username
    /// </summary>
    [Obsolete("Replaced with UserName in 0.18+")]
    public string Username
    {
      get { return UserName; }
    }
    /// <summary>
    /// Username
    /// </summary>
    public string UserName
    {
      get { return packet.UName.Value; }
    }
    /// <summary>
    /// DEPRECATED: Name of the Player
    /// </summary>
    [Obsolete("Replaced with PlayerName in 0.18+")]
    public string Playername
    {
      get { return PlayerName; }
    }
    /// <summary>
    /// Name of the Player
    /// </summary>
    public string PlayerName
    {
      get { return packet.PName.Value; }
    }
    /// <summary>
		/// The id of the connection
		/// </summary>
		public byte ConnectionId
		{
			get { return packet.UCID; }
		}
		/// <summary>
		/// Total number of connections to the server
		/// </summary>
		public int TotalConnections
		{
			get { return (int)packet.Total; }
		}

    /// <summary>
    /// Connection is a remote connection
    /// </summary>
    public bool IsRemote
    {
      get { return (packet.Flags & Flags.NCN.Remote) == Flags.NCN.Remote; }
    }
		#endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
