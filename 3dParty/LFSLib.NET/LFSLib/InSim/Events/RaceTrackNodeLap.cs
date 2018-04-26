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
	/// Node and Lap Event describing each players current position in the race as current lap
	/// and node in that lap
	/// </summary>
  public class RaceTrackNodeLap : RaceTrackEvent
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private Packets.IS_NLP packet;
		private NodeLapInfo[] nodeLaps;
		#endregion

		#region Constructors ##########################################################################
    internal RaceTrackNodeLap(Packets.IS_NLP packet)
		{
      this.packet = packet;

      nodeLaps = new NodeLapInfo[packet.NumP];
			for(int i = 0; i < packet.NumP; i++)
			{
				nodeLaps[i] = new NodeLapInfo(packet.Info[i]);
			}
			log.Debug("RaceTrackNodeLap event created");
		}
		#endregion

		#region Properties ############################################################################
		/// <summary>
		/// Array of LapInfo objects, ordered by player number
		/// </summary>
		public NodeLapInfo[] LapInfo
		{
			get { return nodeLaps; }
		}
		#endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
