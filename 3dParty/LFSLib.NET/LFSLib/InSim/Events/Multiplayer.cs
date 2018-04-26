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
using FullMotion.LiveForSpeed.Util;

namespace FullMotion.LiveForSpeed.InSim.Events
{
	/// <summary>
	/// Multiplayer Notification
	/// </summary>
	public class Multiplayer : EventArgs
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private bool isEnd = false;
    private bool isHost = false;
		private string hostName;
    private byte requestId = 0;
		#endregion

		#region Constructors ##########################################################################
    internal Multiplayer(Packets.IS_ISM packet)
    {
      isHost = (packet.Host == Enums.ServerType.Host);
      hostName = CharHelper.GetString(packet.HName);
      if (hostName == string.Empty)
      {
        isEnd = true;
      }
    }
    internal Multiplayer(Packets.IS_TINY packet)
    {
      this.isEnd = true;
    }
		#endregion

		#region Properties ############################################################################
    /// <summary>
    /// Id of the request that caused this event to be triggered. 0 if the event trigger was not
    /// manually requested or the event signifies the end of Multiplayer (<see cref="IsEnd"/>
    /// will be true as well)
    /// </summary>
    public byte RequestId
    {
      get { return requestId; }
    }

		/// <summary>
		/// True if the event is a notification of a host ending or being left
		/// </summary>
		public bool IsEnd
		{
			get { return isEnd; }
		}

		/// <summary>
		/// True if the sender is the Host instead of a Guest. Always false if <see cref="IsEnd"/> is true.
		/// </summary>
		public bool IsHost
		{
			get { return isHost; }
		}

		/// <summary>
		/// The name of the Host joined or started. Null if <see cref="IsEnd"/> is true.
		/// </summary>
		public string HostName
		{
			get { return hostName; }
		}
		#endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
