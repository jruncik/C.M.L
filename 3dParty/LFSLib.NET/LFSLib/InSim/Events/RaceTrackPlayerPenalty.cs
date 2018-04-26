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
	/// Event describing a player penalty event (given or cleared)
	/// </summary>
  public class RaceTrackPlayerPenalty : RaceTrackEvent
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
    private Packets.IS_PEN packet;
		#endregion

		#region Constructors ##########################################################################
    internal RaceTrackPlayerPenalty(Packets.IS_PEN packet)
		{
      this.packet = packet;

      log.Debug("RaceTrackPlayerPenalty event created");
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
    /// Previous Penalty value
    /// </summary>
    public Enums.Penalty OldPenalty
    {
      get { return packet.OldPen; }
    }

    /// <summary>
    /// New Penalty value
    /// </summary>
    public Enums.Penalty NewPenalty
    {
      get { return packet.NewPen; }
    }

    /// <summary>
    /// Reason for the penalty change
    /// </summary>
    public Enums.PenaltyReason Reason
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
