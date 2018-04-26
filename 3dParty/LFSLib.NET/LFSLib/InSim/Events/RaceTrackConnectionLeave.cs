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
	/// Event sent by LFS when a Connection leaves the server
	/// </summary>
  public class RaceTrackConnectionLeave : RaceTrackEvent
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private Packets.IS_CNL packet;
		#endregion

		#region Constructors ##########################################################################
    internal RaceTrackConnectionLeave(Packets.IS_CNL packet)
		{
      this.packet = packet;

			log.Debug("RaceTrackConnectionLeave event created");
		}
		#endregion

		#region Properties ############################################################################
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
    /// The reason for the connnection leaving
    /// </summary>
    public Enums.LeaveReason Reason
    {
      get { return packet.Reason; }
    }
		#endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
