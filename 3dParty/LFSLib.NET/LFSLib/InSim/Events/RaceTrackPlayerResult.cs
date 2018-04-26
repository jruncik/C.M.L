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
  /// Event sent for every player that finishes a race or qualifying event
  /// </summary>
  public class RaceTrackPlayerResult : RaceTrackEvent
  {
    #region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

    #region Member Variables ######################################################################
    private Packets.IS_RES packet;
    #endregion

    #region Constructors ##########################################################################
    internal RaceTrackPlayerResult(Packets.IS_RES packet)
    {
      this.packet = packet;

      log.Debug("RaceTrackPlayerResult event created");
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
    /// UserName
    /// </summary>
    public string UserName
    {
      get { return packet.UName.Value; }
    }
    /// <summary>
    /// Name of the Player
    /// </summary>
    public string PlayerName
    {
      get { return packet.PName.Value; }
    }
    /// <summary>
    /// Id of the Player
    /// </summary>
    public byte PlayerId
    {
      get { return packet.PLID; }
    }
    /// <summary>
    /// Short Name of the car, skin prefix (e.g. XRT)
    /// </summary>
    public string ShortCarname
    {
      get { return packet.CName.Value; }
    }
    /// <summary>
    /// Player was mentioned in race results
    /// </summary>
    public bool Mentioned
    {
      get { return (packet.Confirm & Flags.CONF.MENTIONED) == Flags.CONF.MENTIONED; }
    }

    /// <summary>
    /// Player's race results are confirmed
    /// </summary>
    public bool Confirmed
    {
      get { return (packet.Confirm & Flags.CONF.CONFIRMED) == Flags.CONF.CONFIRMED; }
    }

    /// <summary>
    /// Player was disqualified
    /// </summary>
    public bool Disqualified
    {
      get { return Flags.CONF.DISQ == (Flags.CONF.DISQ & packet.Confirm); }
    }

    /// <summary>
    /// Penalty seconds added to final time
    /// </summary>
    public int PenaltySeconds
    {
      get { return packet.PSeconds; }
    }

    /// <summary>
    /// Player received a Drive-Through penalty
    /// </summary>
    public bool PenaltyDriveThrough
    {
      get { return (packet.Confirm & Flags.CONF.PENALTY_DT) == Flags.CONF.PENALTY_DT; }
    }

    /// <summary>
    /// Player received a Stop and Go penalty
    /// </summary>
    public bool PenaltyStopAndGo
    {
      get { return (packet.Confirm & Flags.CONF.PENALTY_SG) == Flags.CONF.PENALTY_SG; }
    }

    /// <summary>
    /// Player received a 30 second penalty
    /// </summary>
    public bool Penalty30seconds
    {
      get { return (packet.Confirm & Flags.CONF.PENALTY_30) == Flags.CONF.PENALTY_30; }
    }

    /// <summary>
    /// Player received a 45 second penalty
    /// </summary>
    public bool Penalty45seconds
    {
      get { return (packet.Confirm & Flags.CONF.PENALTY_45) == Flags.CONF.PENALTY_45; }
    }

    /// <summary>
    /// Player did not pit on a mandatory pit race
    /// </summary>
    public bool DidNotPit
    {
      get { return (packet.Confirm & Flags.CONF.DID_NOT_PIT) == Flags.CONF.DID_NOT_PIT; }
    }

    /// <summary>
    /// Player incurred a Time Penalty
    /// </summary>
    public bool TimePenalty
    {
      get { return Flags.CONF.TIME == (Flags.CONF.TIME & packet.Confirm); }
    }

    /// <summary>
    /// The number of laps completed by the player
    /// </summary>
    public int LapsDone
    {
      get { return (int)packet.LapsDone; }
    }

    /// <summary>
    /// The number of stops taken by the player during the race
    /// </summary>
    public int NumStops
    {
      get { return (int)packet.NumStops; }
    }
    /// <summary>
    /// License Plate used by the player's car
    /// </summary>
    public string Plate
    {
      get { return packet.Plate.Value; }
    }
    /// <summary>
    /// Is the player a left hand driver
    /// </summary>
    public bool SwapSide
    {
      get { return (packet.Flags & Flags.PIF.SWAPSIDE) == Flags.PIF.SWAPSIDE; }
    }
    /// <summary>
    /// Is gear change throttle cut enabled
    /// </summary>
    public bool GearChangeCut
    {
      get { return (packet.Flags & Flags.PIF.GC_CUT) == Flags.PIF.GC_CUT; }
    }
    /// <summary>
    /// Is gear change throttle blip enabled
    /// </summary>
    public bool GearChangeBlip
    {
      get { return (packet.Flags & Flags.PIF.GC_BLIP) == Flags.PIF.GC_BLIP; }
    }
    /// <summary>
    /// Is the player using automatic gear changes
    /// </summary>
    public bool AutoGears
    {
      get { return (packet.Flags & Flags.PIF.AUTOGEARS) == Flags.PIF.AUTOGEARS; }
    }
    /// <summary>
    /// Is brake help turned on
    /// </summary>
    public bool HelpBrake
    {
      get { return (packet.Flags & Flags.PIF.HELP_B) == Flags.PIF.HELP_B; }
    }
    /// <summary>
    /// Is the clutch set to automatic
    /// </summary>
    public bool Autoclutch
    {
      get { return (packet.Flags & Flags.PIF.AUTOCLUTCH) == Flags.PIF.CUSTOM_VIEW; }
    }
    /// <summary>
    /// Is the player using a mouse
    /// </summary>
    public bool Mouse
    {
      get { return (packet.Flags & Flags.PIF.MOUSE) == Flags.PIF.MOUSE; }
    }
    /// <summary>
    /// Is the player using a keyboard without aids
    /// </summary>
    public bool KeyboardNoHelp
    {
      get { return (packet.Flags & Flags.PIF.KB_NO_HELP) == Flags.PIF.KB_NO_HELP; }
    }
    /// <summary>
    /// Is the player using a stabilized keyboard
    /// </summary>
    public bool KeyboardStabilized
    {
      get { return (packet.Flags & Flags.PIF.KB_STABILISED) == Flags.PIF.KB_STABILISED; }
    }
    /// <summary>
    /// Is the player using a custom view
    /// </summary>
    public bool CustomView
    {
      get { return (packet.Flags & Flags.PIF.CUSTOM_VIEW) == Flags.PIF.CUSTOM_VIEW; }
    }
    /// <summary>
    /// Is the player in the Pits
    /// </summary>
    public bool InPits
    {
      get { return (packet.Flags & Flags.PIF.INPITS) == Flags.PIF.INPITS; }
    }
    /// <summary>
    /// Is the player using a Shifter
    /// </summary>
    public bool Shifter
    {
      get { return (packet.Flags & Flags.PIF.SHIFTER) == Flags.PIF.SHIFTER; }
    }
    /// <summary>
    /// Position in the results table
    /// </summary>
    public int ResultNumber
    {
      get { return (int)packet.ResultNum; }
    }
    /// <summary>
    /// Total number of results
    /// </summary>
    public int NumResults
    {
      get { return (int)packet.NumRes; }
    }

    /// <summary>
    /// Total time in race
    /// </summary>
    public TimeSpan TotalTime
    {
      get { return new TimeSpan(0, 0, 0, 0, (int)packet.TTime); }
    }

    /// <summary>
    /// Best Lap time
    /// </summary>
    public TimeSpan BestLapTime
    {
      get { return new TimeSpan(0, 0, 0, 0, (int)packet.BTime); }
    }
    #endregion

    #region Methods ###############################################################################
    #endregion

    #region Private Methods #######################################################################
    #endregion
  }
}
