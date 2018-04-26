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
  /// Client request for AutocrossInfo information
  /// </summary>
  public class AutocrossInfo : EventArgs
  {
    #region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

    #region Member Variables ######################################################################
    private Packets.IS_AXI packet;
    private string layout;
    #endregion

    #region Constructors ##########################################################################
    internal AutocrossInfo(Packets.IS_AXI packet)
    {
      this.packet = packet;
      this.layout = CharHelper.GetString(packet.LName);
    }
    #endregion

    #region Properties ############################################################################
    /// <summary>
    /// The id of the originating request, if any
    /// </summary>
    public byte RequestId
    {
      get { return packet.ReqI; }
    }

    /// <summary>
    /// Number of Checkpoints in the layout
    /// </summary>
    public int NumberOfCheckPoints
    {
      get { return packet.NumCP; }
    }

    /// <summary>
    /// Number of objects in the layout
    /// </summary>
    public int NumberOfObjects
    {
      get { return packet.NumO; }
    }

    /// <summary>
    /// The name of the layout last loaded (if loaded locally)
    /// </summary>
    public string Layoutname
    {
      get { return layout; }
    }
    #endregion

    #region Methods ###############################################################################
    #endregion

    #region Private Methods #######################################################################
    #endregion
  }
}
