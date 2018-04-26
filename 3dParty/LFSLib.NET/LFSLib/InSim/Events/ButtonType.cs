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
using FullMotion.LiveForSpeed.Util;

namespace FullMotion.LiveForSpeed.InSim.Events
{
  /// <summary>
  /// Text typed into a custom Dialog
  /// </summary>
  public class ButtonType : EventArgs
  {
    #region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

    #region Member Variables ######################################################################
    private Packets.IS_BTT packet;
    private string text;
    #endregion

    #region Constructors ##########################################################################
    internal ButtonType(Packets.IS_BTT packet)
    {
      this.packet = packet;
    }
    #endregion

    #region Properties ############################################################################
    /// <summary>
    /// Did the type event come from the local connection or from another client
    /// </summary>
    public bool Local
    {
      get { return (packet.UCID == 0); }
    }

    /// <summary>
    /// The connection Id of the requestor (0 if the type was locally originated)
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
    /// The Id of the button dialog activated
    /// </summary>
    public byte ButtonId
    {
      get { return packet.ClickID; }
    }

    /// <summary>
    /// The typed in text
    /// </summary>
    public string Text
    {
      get
      {
        if (string.IsNullOrEmpty(text))
        {
          text = CharHelper.GetString(packet.Text);
        }
        return text;
      }
    }

    #endregion

    #region Methods ###############################################################################
    #endregion

    #region Private Methods #######################################################################
    #endregion
  }
}
