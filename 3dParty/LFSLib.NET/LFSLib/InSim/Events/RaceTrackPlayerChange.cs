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
using FullMotion.LiveForSpeed.Util;

namespace FullMotion.LiveForSpeed.InSim.Events
{
  /// <summary>
  /// Event describing a player's option changing
  /// </summary>
  public class RaceTrackPlayerChange : RaceTrackEvent
  {
    #region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

    #region Member Variables ######################################################################
    private Packets.IS_PFL packet;
    #endregion

    #region Constructors ##########################################################################
    internal RaceTrackPlayerChange(Packets.IS_PFL packet)
    {
      this.packet = packet;

      log.Debug("RaceTrackPlayerChange event created");
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
      get { return (packet.Flags & Flags.PIF.AUTOCLUTCH) == Flags.PIF.AUTOCLUTCH; }
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
    #endregion

    #region Methods ###############################################################################
    #endregion

    #region Private Methods #######################################################################
    #endregion
  }
}

