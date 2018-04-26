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
	/// Event sent each time a split is reached by a player
	/// </summary>
  public class RaceTrackPlayerSplitTime : RaceTrackEvent
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private Packets.IS_SPX packet;
		#endregion

		#region Constructors ##########################################################################
    internal RaceTrackPlayerSplitTime(Packets.IS_SPX packet)
		{
      this.packet = packet;
      log.Debug("RaceTrackPlayerSplitTime event created");
		}
		#endregion

		#region Properties ############################################################################
    /// <summary>
    /// The Split time as a timespan object
    /// </summary>
    public TimeSpan SplitTime
    {
      get { return new TimeSpan(0, 0, 0, 0, (int)packet.STime); }
    }
    /// <summary>
    /// The Total time as a timespan object
    /// </summary>
    public TimeSpan TotalTime
    {
      get { return new TimeSpan(0, 0, 0, 0, (int)packet.ETime); }
    }
    /// <summary>
		/// The player's unique Id
		/// </summary>
		public byte PlayerId
		{
			get { return packet.PLID; }
		}
    /// <summary>
		/// Which split does this event concern?
		/// </summary>
		public int Split
		{
			get { return packet.Split; }
		}
    /// <summary>
    /// Current Penalty
    /// </summary>
    public Enums.Penalty Penalty
    {
      get { return packet.Penalty; }
    }
    /// <summary>
    /// Number of Pit Stops
    /// </summary>
    public int NumberOfPitStops
    {
      get { return packet.NumStops; }
    }
    #endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
