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
using System.Text;
using System.Threading;
using log4net;
using FullMotion.LiveForSpeed.InSim.Packets;
using FullMotion.LiveForSpeed.InSim;

namespace FullMotion.LiveForSpeed
{
	/// <summary>
	/// AbstractLfsPacketReader uses a separate thread to wait for incoming messages from an LFS host.
	/// Other objects can subscribe to the received events for processing
	/// </summary>
	internal class AbstractLfsPacketReader : ILfsReader
	{
		#region Static Members ########################################################################
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Member Variables ######################################################################
		Thread listenerThread;
		Socket listener;
		int port;
		IPAddress[] localIPs;
		EndPoint remoteEP;
		#endregion

		#region Constructors ##########################################################################
		/// <summary>
		/// Public constructor requiring a port to listen on
		/// </summary>
		/// <param name="port"></param>
		public AbstractLfsPacketReader(int port) {
			this.port = port;
			localIPs = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			remoteEP = new IPEndPoint(IPAddress.Any, this.port);
			try {
				//Starting the UDP Server thread.
				listenerThread = new Thread(new ThreadStart(Listen));
				listenerThread.Start();
				log.Debug("Started Listener Thread on port " + this.port);
			} catch (Exception e) {
				log.Debug("Listener Thread unable to start", e);
				listenerThread.Abort();
			}
		}
		#endregion

		#region Properties ############################################################################
		public IPAddress RemoteIP {
			get { return ((IPEndPoint)remoteEP).Address; }
		}
		public IPAddress[] LocalIPs {
			get { return localIPs; }
		}
		#endregion

		#region Methods ###############################################################################

		public void Stop() {
			log.Debug("shutting down");
			try {
				listener.Shutdown(SocketShutdown.Both);
			} catch { }
			try {
				listener.Close();
			} catch { }
			if (listenerThread != null) {
				listenerThread.Abort();
				listenerThread.Join();
			}
			log.Debug("shut down");
		}
		#endregion

		#region Private Methods #######################################################################
		private void Listen() {
			try {
				IPEndPoint ipep = new IPEndPoint(IPAddress.Any, this.port);

				listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

				listener.Bind(ipep);

				log.Debug("Listening...");

				Byte[] received = new Byte[512];

				while (true) {
					Thread.Sleep(10);
					int bytesReceived = listener.ReceiveFrom(received, ref remoteEP);

					log.Debug(String.Format("received {0} bytes", bytesReceived));
					int packetByteCount = PacketFactory.CheckHeader(received, 0);
					if (packetByteCount == 0) {
						log.Debug("Unknown packet");
					} else {
						ILfsInSimPacket packet = PacketFactory.GetPacket(received, 0);
						log.Debug("InSim packet: " + packet.PacketType);
						if (InSimPacketReceived != null) {
							InSimPacketReceived(this, new InSimPacketEventArgs(packet));
						}
					}
					log.Debug("waiting");
				}
			} catch (SocketException se) {
				if (se.ErrorCode == 10004) {
					log.Error("been asked to shut down", se);
				} else {
					log.Error("A Socket Exception has occurred!" + se.ToString(), se);
				}
			} catch (Exception e) {
				log.Error("An unknown exception happened in our packet reader", e);
			}
		}
		#endregion

		#region Events ################################################################################
		public event InSimPacketReceivedEventHandler InSimPacketReceived;

		#endregion
	}
}