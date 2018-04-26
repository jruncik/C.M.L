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
	/// Event describing a player changing its name
	/// </summary>
  public class RaceTrackPlayerRename : RaceTrackEvent
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private Packets.IS_CPR packet;
		#endregion

		#region Constructors ##########################################################################
    internal RaceTrackPlayerRename(Packets.IS_CPR packet)
		{
      this.packet = packet;

			log.Debug("RaceTrackPlayerRename event created");
		}
		#endregion

		#region Properties ############################################################################
		/// <summary>
		/// Username
		/// </summary>
		public string PlayerName
		{
			get { return packet.PName.Value; }
		}
		/// <summary>
		/// License Plate used by the player's car
		/// </summary>
		public string Plate
		{
			get { return packet.Plate.Value; }
		}
    /// <summary>
    /// DEPRECATED: The connection's Id
    /// </summary>
    [Obsolete("Replaced with ConnectionId in 0.18+")]
    public byte ConnectionID
    {
      get { return packet.UCID; }
    }
    /// <summary>
    /// The connection's Id
    /// </summary>
    public byte ConnectionId
    {
      get { return packet.UCID; }
    }
    #endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
