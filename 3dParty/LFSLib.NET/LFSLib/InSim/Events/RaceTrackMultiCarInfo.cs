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
	/// Event describing each players current standing in the race
	/// </summary>
  public class RaceTrackMultiCarInfo : RaceTrackEvent
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private Packets.IS_MCI packet;
		private CarInfo[] cars;
		#endregion

		#region Constructors ##########################################################################
    internal RaceTrackMultiCarInfo(Packets.IS_MCI packet)
		{
      this.packet = packet;

			cars = new CarInfo[packet.NumC];
      for (byte i = 0; i < packet.NumC; i++)
			{
				cars[i] = new CarInfo(packet.Info[i]);
			}
			log.Debug("RaceTrackMultiCarInfo event created");
		}
		#endregion

		#region Properties ############################################################################
		/// <summary>
    /// Array of up to 8 <see cref="CarInfo"/> objects. If there are more than 8 cars in the race
    /// more than one <see cref="RaceTrackMultiCarInfo"/> events will be sent
		/// </summary>
		public CarInfo[] CarInfo
		{
			get { return cars; }
		}
		#endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
