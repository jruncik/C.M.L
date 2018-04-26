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
using System.Net;
using System.Net.Sockets;
using log4net;

namespace FullMotion.LiveForSpeed.InSim
{
	/// <summary>
	/// InSimWriter handles outbound messages to an LFS Host
	/// </summary>
	internal class InSimWriter : IInSimWriter
	{
		#region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

		#region Member Variables ######################################################################
		UdpClient connection;
		#endregion

		#region Constructors ##########################################################################
		/// <summary>
		/// Public constructor requring a host and port to connect to
		/// </summary>
		/// <param name="server"></param>
		/// <param name="port"></param>
		public InSimWriter(string server, int port)
		{
			try
			{
				connection = new UdpClient(server, port);
			}
			catch(Exception e)
			{
				throw new InSim.Exceptions.InSimHandlerException.ConnectionFailed(e);
			}
		}
		#endregion

		#region Properties ############################################################################
		#endregion

		#region Methods ###############################################################################

    /// <summary>
    /// Send an arbitraty IClientLfsInSimPacket
    /// </summary>
    /// <param name="packet"></param>
    public void Send(Packets.IClientLfsInSimPacket packet)
    {
      log.Debug("Sent packet: " + packet.PacketType);
      byte[] packetBytes = packet.GetBytes();
      connection.Send(packetBytes, packetBytes.Length);

    }
		/// <summary>
		/// Send a MsgClose and close the connection
		/// </summary>
		public void Close()
		{
      this.Send(Packets.PacketFactory.GetClosePacket());
			this.connection.Close();
		}
    #endregion
	}
}
