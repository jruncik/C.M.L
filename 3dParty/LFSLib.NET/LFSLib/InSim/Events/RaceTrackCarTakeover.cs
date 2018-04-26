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
	/// Event describing a car being taken over by another player
	/// </summary>
  public class RaceTrackCarTakeover : RaceTrackEvent
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
    private Packets.IS_TOC packet;
		#endregion

		#region Constructors ##########################################################################
    internal RaceTrackCarTakeover(Packets.IS_TOC packet)
		{
      this.packet = packet;

      log.Debug("RaceTrackCarTakeover event created");
		}
		#endregion

		#region Properties ############################################################################
		/// <summary>
		/// The player's unique Id
		/// </summary>
		public byte PlayerId
		{
			get { return packet.PLID; }
		}

    /// <summary>
    /// Connection Id of the previous driver
    /// </summary>
    public byte OldConnectionId
    {
      get { return packet.OldUCID; }
    }

    /// <summary>
    /// Connection Id of the new driver
    /// </summary>
    public byte NewConnectionId
    {
      get { return packet.NewUCID; }
    }
    #endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
