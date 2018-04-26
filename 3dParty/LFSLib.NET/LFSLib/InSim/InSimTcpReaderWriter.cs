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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;
using FullMotion.LiveForSpeed;
using System.Runtime.Remoting.Messaging;

namespace FullMotion.LiveForSpeed.InSim
{
	// TODO: need event to notify clients of our udp reader dying
	internal class InSimTcpReaderWriter : IInSimReader, IInSimWriter
	{
		#region Static Members ########################################################################
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private const int BUFFER_SIZE = 512;
		#endregion

		#region Member Variables ######################################################################
		private InSimReader udpReader;
		private string lfsHost;
		private int lfsTcpPort;
		private int lfsUdpReplyPort;
		private TcpClient tcpClient;
		private bool stopped = false;
		private byte[] received = new byte[BUFFER_SIZE];
		private int offset = 0;
		private bool closed = true;
		#endregion

		#region Constructors ##########################################################################
		public InSimTcpReaderWriter(string lfsHost, int lfsTcpPort, int lfsUdpReplyPort) {
			this.lfsHost = lfsHost;
			this.lfsTcpPort = lfsTcpPort;
			this.lfsUdpReplyPort = lfsUdpReplyPort;

			try {

				tcpClient = new TcpClient();
				tcpClient.Connect(this.lfsHost, this.lfsTcpPort);
				log.DebugFormat("connected to {0}:{1}", this.lfsHost, this.lfsTcpPort);
				closed = false;
			} catch (Exception e) {
				throw new InSim.Exceptions.InSimHandlerException.ConnectionFailed(e);
			}
			if (this.lfsUdpReplyPort > 0) {
				udpReader = new InSimReader(this.lfsUdpReplyPort);
				udpReader.InSimPacketReceived += new InSimPacketReceivedEventHandler(udpReader_InSimPacketReceived);
			}
			Listen();
		}
		#endregion

		#region Properties ############################################################################
		public bool Closed {
			get { return closed; }
		}
		#endregion

		#region Methods ###############################################################################
		#endregion

		#region Private Methods #######################################################################
		void udpReader_InSimPacketReceived(ILfsReader sender, InSimPacketEventArgs e) {
			if (InSimPacketReceived != null) {
				log.Debug("passing on insim from UDP");
				InSimPacketReceived(this, e);
			}
		}

		private void Listen() {
			try {
				tcpClient.Client.BeginReceive(received, offset, received.Length - offset, SocketFlags.None, new AsyncCallback(ReceiveCallBack), null);
			} catch (SocketException se) {
				if (se.ErrorCode == 10004) {
					log.Error("been asked to shut down", se);
				} else {
					log.Error("A Socket Exception has occurred!" + se.ToString(), se);
					OnConnectionClosed();
				}
			} catch (Exception e) {
				log.Error("An unknown exception happened in our TCP packet reader", e);
				OnConnectionClosed();
			}

		}

		private void ReceiveCallBack(IAsyncResult ar) {
			try {

				if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected) {
					SocketError err = new SocketError();
					int bytesReceived = tcpClient.Client.EndReceive(ar, out err);
					log.Debug(String.Format("received {0} bytes (w/ offset {1})", bytesReceived, offset));

					// need to count left over bytes in bytesReceived
					bytesReceived += offset;
					// and then we start from the beginning, so no more offset
					offset = 0;

					if (bytesReceived > 0) {
						while (true) {
							int packetByteCount = Packets.PacketFactory.CheckHeader(received, offset);

							if (packetByteCount == 0) {
								log.Debug("Unknown packet");
								offset = 0;
								break;
							} else {
								if (offset + packetByteCount > bytesReceived) {
									log.Debug("not enough bytes for the current packet in buffer, listen for more bytes");
									log.DebugFormat("got {0} bytes left, need {1}", bytesReceived - offset, packetByteCount);
									//Array.Copy(received, offset, received, 0, bytesReceived - (offset + packetByteCount));
									Array.Copy(received, offset, received, 0, bytesReceived - offset);
									offset = bytesReceived - offset;
									log.Debug("setting buffer offset to: " + offset);
									break;
								}
								Packets.ILfsInSimPacket packet = Packets.PacketFactory.GetPacket(received, offset);

								if (packet == null) {
									log.Debug("Unknown packet (one with data, so we drop it)");
									offset = 0;
									break;
								}

								log.Debug("InSim packet: " + packet.PacketType);
								if (InSimPacketReceived != null) {
									InSimPacketReceived(this, new InSimPacketEventArgs(packet));
								}
								if (offset + packetByteCount == bytesReceived) {
									// emptied current buffer
									offset = 0;
									break;
								} else {
									// got more bytes in buffer
									log.Debug("More packets in this buffer");
									offset += packetByteCount;
								}
							}
						}
						Listen();
					} else {
						log.Debug(String.Format("received no bytes", bytesReceived));
						OnConnectionClosed();
					}
				}
			} catch (SocketException se) {
				if (se.ErrorCode == 10004) {
					log.Error("been asked to shut down", se);
				} else {
					log.Error("A Socket Exception has occurred!" + se.ToString(), se);
					OnConnectionClosed();
				}
			} catch (Exception e) {
				log.Error("An unknown exception happened in our TCP packet reader", e);
				OnConnectionClosed();
			}
		}
		#endregion

		#region Events ################################################################################
		public event EventHandler ConnectionClosed;
		private void OnConnectionClosed() {
			Stop();
			closed = true;
			if (ConnectionClosed != null) {
				ConnectionClosed(this, EventArgs.Empty);
			}

		}
		#endregion

		#region IInSimReader Members

		public event InSimPacketReceivedEventHandler InSimPacketReceived;

		public void Stop() {
			if (!stopped) {
				log.Debug("shutting down");
				stopped = true;
				tcpClient.Close();
				if (udpReader != null) {
					udpReader.Stop();
					udpReader.InSimPacketReceived -= new InSimPacketReceivedEventHandler(udpReader_InSimPacketReceived);
				}
				log.Debug("shut down");
			}
		}

		#endregion

		#region IInSimWriter Members

		public void Close() {
			Stop();
		}

		public void Send(Packets.IClientLfsInSimPacket packet) {
			if (!tcpClient.Connected) {
				throw new InSim.Exceptions.InSimHandlerException.NotConnected();
			}
			if (packet.PacketType == Enums.ISP.TINY) {
				log.DebugFormat("Sent packet {0}/{1}", packet.PacketType, ((Packets.IS_TINY)packet).SubT);
			} else if (packet.PacketType == Enums.ISP.SMALL) {
				log.DebugFormat("Sent packet {0}/{1}", packet.PacketType, ((Packets.IS_SMALL)packet).SubT);
			} else {
				log.DebugFormat("Sent packet {0}", packet.PacketType);
			}
			byte[] packetBytes = packet.GetBytes();
			tcpClient.Client.Send(packetBytes);
		}

		#endregion
	}
}
