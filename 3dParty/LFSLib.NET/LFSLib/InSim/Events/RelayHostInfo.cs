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
using FullMotion.LiveForSpeed.InSim.Packets.Support;
using FullMotion.LiveForSpeed.Util;

namespace FullMotion.LiveForSpeed.InSim.Events
{
  /// <summary>
  /// Info about a single host accessible via the relay
  /// </summary>
  public class RelayHostInfo : EventArgs
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
    private Packets.Support.HInfo packet;
    private string hostName;
    private string trackName;
		#endregion

		#region Constructors ##########################################################################
    internal RelayHostInfo(Packets.Support.HInfo packet)
		{
      this.packet = packet;
		}
		#endregion

		#region Properties ############################################################################
    /// <summary>
    /// Name of the host
    /// </summary>
    public string Name
    {
      get
      {
        if (string.IsNullOrEmpty(hostName))
        {
          hostName = CharHelper.GetString(packet.HName);
        }
        return hostName;
      }
    }

    /// <summary>
    /// The track currently being run on the host
    /// </summary>
    public string ShortTrackname
    {
      get
      {
        if (string.IsNullOrEmpty(trackName))
        {
          trackName = CharHelper.GetString(packet.Track);
        }
        return trackName;
      }
    }

    /// <summary>
    /// Number of connections on the host
    /// </summary>
    public byte NumberOfConnections
    {
      get { return packet.NumConns; }
    }

    /// <summary>
    /// Does this host require a spectator password?
    /// </summary>
    public bool RequiresSpectatorPassword
    {
      get { return (packet.Flags & Flags.HOS.SpecPass) == Flags.HOS.SpecPass; }
    }
		#endregion
	}
}
