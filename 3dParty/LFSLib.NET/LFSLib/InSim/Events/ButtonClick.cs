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
using System.Collections.Generic;
using System.Text;

using log4net;

namespace FullMotion.LiveForSpeed.InSim.Events
{
  /// <summary>
  /// A button was clicked
  /// </summary>
  public class ButtonClick : EventArgs
  {
    #region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

    #region Member Variables ######################################################################
    private Packets.IS_BTC packet;
    #endregion

    #region Constructors ##########################################################################
    internal ButtonClick(Packets.IS_BTC packet)
    {
      this.packet = packet;
    }
    #endregion

    #region Properties ############################################################################
    /// <summary>
    /// Did the click event come from the local connection or from another client
    /// </summary>
    public bool Local
    {
      get { return (packet.UCID == 0); }
    }

    /// <summary>
    /// The connection Id of the requestor (0 if the click was locally originated)
    /// </summary>
    public byte ConnectionId
    {
      get { return packet.UCID; }
    }

    /// <summary>
    /// The Id of the request that created the button
    /// </summary>
    public byte RequestId
    {
      get { return packet.ReqI; }
    }

    /// <summary>
    /// The Id of the button clicked
    /// </summary>
    public byte ButtonId
    {
      get { return packet.ClickID; }
    }

    /// <summary>
    /// Was the Left Mouse Button clicked?
    /// </summary>
    public bool LeftMouseButtonClick
    {
      get { return (Flags.ClickFlags.LMB == (Flags.ClickFlags.LMB & packet.CFlags)); }
    }

    /// <summary>
    /// Was the Right Mouse Button clicked?
    /// </summary>
    public bool RightMouseButtonClick
    {
      get { return (Flags.ClickFlags.RMB == (Flags.ClickFlags.RMB & packet.CFlags)); }
    }

    /// <summary>
    /// Was the Ctrl key down at the time of the click?
    /// </summary>
    public bool CtrlClick
    {
      get { return (Flags.ClickFlags.CTRL == (Flags.ClickFlags.CTRL & packet.CFlags)); }
    }

    /// <summary>
    /// Was the Shift key down at the time of the click?
    /// </summary>
    public bool ShiftClick
    {
      get { return (Flags.ClickFlags.SHIFT == (Flags.ClickFlags.SHIFT & packet.CFlags)); }
    }
    #endregion

    #region Methods ###############################################################################
    #endregion

    #region Private Methods #######################################################################
    #endregion
  }
}
