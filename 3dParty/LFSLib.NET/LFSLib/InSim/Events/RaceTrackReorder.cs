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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace FullMotion.LiveForSpeed.InSim.Events
{
	/// <summary>
	/// Event sent after qualifying stops and the order of players on the grid changes
	/// </summary>
	public class RaceTrackReorder : RaceTrackEvent
	{
		#region Static Members ########################################################################
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Member Variables ######################################################################
		private Packets.IS_REO packet;
		byte[] playerOrder;
		#endregion

		#region Constructors ##########################################################################
		internal RaceTrackReorder(Packets.IS_REO packet)
		{
			this.packet = packet;
			log.Debug("RaceTrackReorder event created");
			playerOrder = new byte[packet.NumP];
			for (int i=0; i<packet.NumP; i++) {
				playerOrder[i] = packet.PLID[i];
			}
		}
		#endregion

		#region Properties ############################################################################
		/// <summary>
		/// Order of players on the grid
		/// </summary>
		public byte[] OrderedPlayerIds
		{
			get { return playerOrder; }
		}
		#endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
