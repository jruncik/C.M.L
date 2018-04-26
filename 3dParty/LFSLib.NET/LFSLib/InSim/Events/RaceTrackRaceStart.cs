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
	/// Event sent when a race starts
	/// </summary>
  public class RaceTrackRaceStart : RaceTrackEvent
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private Packets.IS_RST packet;
		#endregion

		#region Constructors ##########################################################################
    internal RaceTrackRaceStart(Packets.IS_RST packet)
		{
      this.packet = packet;

			log.Debug("RaceTrackRaceStart event created");
		}
		#endregion

		#region Properties ############################################################################
    /// <summary>
    /// Id of the request that caused this event to be triggered. 0 if the event trigger was not
    /// manually requested
    /// </summary>
    public byte RequestId
    {
      get { return packet.ReqI; }
    }
		/// <summary>
		/// Race Lap Info
		/// </summary>
		public LapInfo LapInfo
		{
			get { return new LapInfo(packet.RaceLaps); }
		}
		/// <summary>
		/// Total minutes for Qualifying
		/// </summary>
		public byte QualifyingMinutes
		{
			get { return packet.QualMins; }
		}
		/// <summary>
		/// How many players in this race
		/// </summary>
		public byte NumberInRace
		{
			get { return packet.NumP; }
		}
    /// <summary>
    /// Current weather
    /// </summary>
    public Enums.Weather Weather
    {
      get { return packet.Weather; }
    }
    /// <summary>
    /// Current weather
    /// </summary>
    public Enums.Wind Wind
    {
      get { return packet.Wind; }
    }
    /// <summary>
		/// The short name for the current track
		/// </summary>
		public string ShortTrackName
		{
			get { return packet.Track.Value; }
		}

    /// <summary>
    /// Number of nodes in this race configuration
    /// </summary>
    public int NumberOfNodes
    {
      get { return packet.NumNodes; }
    }

    /// <summary>
    /// Index of the Start/Finish line node
    /// </summary>
    public int FinishNodeIndex
    {
      get { return packet.Finish; }
    }

    /// <summary>
    /// Index of the first split line node
    /// </summary>
    public int Split1NodeIndex
    {
      get { return packet.Split1; }
    }

    /// <summary>
    /// Index of the second split line node
    /// </summary>
    public int Split2NodeIndex
    {
      get { return packet.Split2; }
    }

    /// <summary>
    /// Index of the third split line node
    /// </summary>
    public int Split3NodeIndex
    {
      get { return packet.Split3; }
    }

    /// <summary>
    /// Users can vote
    /// </summary>
    public bool CanVote
    {
      get { return Flags.RaceFlags.CAN_VOTE == (Flags.RaceFlags.CAN_VOTE & packet.Flags); }
    }

    /// <summary>
    /// Users can select tracks
    /// </summary>
    public bool CanSelect
    {
      get { return Flags.RaceFlags.CAN_SELECT == (Flags.RaceFlags.CAN_SELECT & packet.Flags); }
    }

    /// <summary>
    /// Users can join mid race
    /// </summary>
    public bool MidRaceJoin
    {
      get { return Flags.RaceFlags.MID_RACE == (Flags.RaceFlags.MID_RACE & packet.Flags); }
    }

    /// <summary>
    /// Racers must make a pitstop for a valid race
    /// </summary>
    public bool MustPit
    {
      get { return Flags.RaceFlags.MUST_PIT == (Flags.RaceFlags.MUST_PIT & packet.Flags); }
    }

    /// <summary>
    /// Racers can reset their cars
    /// </summary>
    public bool CanReset
    {
      get { return Flags.RaceFlags.CAN_RESET == (Flags.RaceFlags.CAN_RESET & packet.Flags); }
    }

		#endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
