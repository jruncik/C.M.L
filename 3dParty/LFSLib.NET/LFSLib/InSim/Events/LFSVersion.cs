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
	/// LFSVersion event. Event interception is not directly exposed. Should be accessed via <seealso cref="InSimHandler"/>
	/// </summary>
  public class LFSVersion : EventArgs
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		private Packets.IS_VER packet;
		#endregion

		#region Constructors ##########################################################################
    internal LFSVersion(Packets.IS_VER packet)
		{
			this.packet = packet;
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
		/// The serial number of the packet protocol. If it differs from the one in <see cref="LibVersion"/>,
		/// this Library may fail communicating with LFS
		/// </summary>
		public ushort Serial
		{
			get { return packet.InSimVer; }
		}

		/// <summary>
		/// The string version of LFS
		/// </summary>
		public string Version
		{
			get { return packet.Version.Value; }
		}

		/// <summary>
		/// The Product, such as DEMO, S1, S2
		/// </summary>
		public string Product
		{
			get { return packet.Product.Value; }
		}
		#endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		#endregion
	}
}
