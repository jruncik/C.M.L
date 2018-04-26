/* ------------------------------------------------------------------------- *
 * Copyright (C) 2005,2006,2007 Arne Claassen
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
using System.Threading;
using System.Xml;
using log4net;
using FullMotion.LiveForSpeed.InSim.Exceptions;
using FullMotion.LiveForSpeed.InSim.Events;
using FullMotion.LiveForSpeed.InSim.EventHandlers;
using FullMotion.LiveForSpeed.InSim.Packets;

using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;

namespace FullMotion.LiveForSpeed.InSim
{

	/// <summary>
	/// Object Model encapsulating all InSim communication with an LFS Host
	/// </summary>
	public partial class InSimHandler : IInSimRelayHandler, IDisposable
	{
		#region Enums #################################################################################
		/// <summary>
		/// State of the InSimHandler object
		/// </summary>
		public enum HandlerState
		{
			/// <summary> Handler has been created and never connected </summary>
			New,
			/// <summary> Handler has been created and initialized </summary>
			Initiaized,
			/// <summary> Handler is currently connected to Live For Speed </summary>
			Connected,
			/// <summary> Handler has been closed </summary>
			Closed
		}
		#endregion

		#region Constants #############################################################################
		private const string CONFIG_TAG = "lfs-insim-config";
		private const string RELAY_HOST = "lfs.net";
		private const int RELAY_PORT = 47474;
		#endregion

		#region Static Members ########################################################################
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Instantiate a relay handler for the master relay
		/// </summary>
		/// <returns></returns>
		public static IInSimRelayHandler GetMasterRelayHandler() {
			return GetRelayHandler(RELAY_HOST, RELAY_PORT);
		}

		/// <summary>
		/// Instantiate a relay handler against a custom relay (should be primarily limited to debug)
		/// </summary>
		/// <param name="host">relay host</param>
		/// <param name="port">relay port</param>
		/// <returns></returns>
		public static IInSimRelayHandler GetRelayHandler(string host, int port) {
			InSimHandler handler = new InSimHandler();
			handler.relayHost = host;
			handler.relayPort = port;
			return (IInSimRelayHandler)handler;
		}
		#endregion

		#region Member Variables ######################################################################
		private IInSimReader reader;
		private IInSimWriter writer;
		private Configuration configuration = new Configuration();
		private ManualResetEvent awaitingVersion = new ManualResetEvent(false);
		private ManualResetEvent awaitingState = new ManualResetEvent(false);
		private LFSVersion version;
		private State lastState;
		private State currentState;
		private HandlerState state = HandlerState.New;
		private int trackInterval;
		private int initWait;
		private bool isDisposed;

		private Thread processPacketsQueue;
		private Queue<ILfsInSimPacket> packets = new Queue<ILfsInSimPacket>(256);

		//relay
		private string relayHost = string.Empty;
		private int relayPort = 0;
		#endregion

		#region Constructors ##########################################################################
		/// <summary>
		/// No argument constructor. Expects that Handler is manually configured
		/// </summary>
		public InSimHandler()
			: this(false) {
		}

		/// <summary>
		/// Expects that Handler is manually configured. 
		/// </summary>
		/// <param name="useApplicationConfig">If true, configuration settings are loaded from </param>
		public InSimHandler(bool useApplicationConfig) {
			if (useApplicationConfig) {
				this.configuration.InitFromApplicationConfig();
				log.Debug("InSimHandler created with application settings as configuration provider");
			} else {
				log.Debug("InSimHandler created without configuration file");
			}
		}

		/// <summary>
		/// Constructor expecting configuration file. Will throw exceptions if configuration cannot be
		/// processed
		/// </summary>
		/// <param name="configurationFile"></param>
		public InSimHandler(string configurationFile) {
			log.Debug("InSimHandler created with configuration file");
			this.configuration.Load(configurationFile);
		}

		/// <summary>
		/// Constructor expecting a populated instance of <see cref="FullMotion.LiveForSpeed.InSim.Configuration.LfsInSim"/>. This
		/// instance should never be created by hand. Use the <see cref="InSimHandler.Configuration"/>
		/// accessor for all manual configuration. This constructor is intended soley for applications
		/// that have their own configuration mechanism based on XmlSerializer and hence can incorporate
		/// the internal storage object into their own scheme.
		/// </summary>
		/// <param name="configurationData"></param>
		public InSimHandler(Configuration.LfsInSim configurationData) {
			log.Debug("InSimHandler created with configuration element");
			this.configuration.Init(configurationData);
		}
		#endregion

		#region Properties ############################################################################
		public bool IsDisposed {
			get { return isDisposed; }
		}

		/// <summary>
		/// Accessor to Configuration object to be used in manual configuration
		/// </summary>
		public Configuration Configuration {
			get {
				return this.configuration;
			}
		}

		/// <summary>
		/// The state invalidated by the most recent State event. Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		public State LastLFSState {
			get {
				CheckConnection();
				CheckState();
				return lastState;
			}
		}

		/// <summary>
		/// The last state that was received. Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		public State CurrentLFSState {
			get {
				CheckConnection();
				CheckState();
				return currentState;
			}
		}

		/// <summary>
		/// Camera is set to Follow the car
		/// </summary>
		public bool IsFollowing {
			get {
				return CurrentLFSState.IsFollowing;
			}
			set {
				CheckConnection();
				writer.Send(Packets.PacketFactory.GetStateModPacket(Packets.Support.StateModHelper.FollowCar, value));
			}
		}

		/// <summary>
		/// Buttons are hidden in UI
		/// </summary>
		public bool ShiftUButtonsHidden {
			get {
				return CurrentLFSState.ShiftUButtonsHidden;
			}
			set {
				CheckConnection();
				writer.Send(Packets.PacketFactory.GetStateModPacket(Packets.Support.StateModHelper.ShiftUButtonsHidden, value));
			}
		}

		/// <summary>
		/// Camera is in 2D view
		/// </summary>
		public bool IsIn2DView {
			get {
				return CurrentLFSState.IsIn2DView;
			}
			set {
				CheckConnection();
				writer.Send(Packets.PacketFactory.GetStateModPacket(Packets.Support.StateModHelper.Show2D, value));
			}
		}

		/// <summary>
		/// Multiplayer speedup option is turned on in the configuration
		/// </summary>
		public bool MultiplayerSpeedupOptionOn {
			get {
				return CurrentLFSState.MultiplayerSpeedupOptionOn;
			}
			set {
				CheckConnection();
				writer.Send(Packets.PacketFactory.GetStateModPacket(Packets.Support.StateModHelper.MultiplayerSpeedup, value));
			}
		}

		/// <summary>
		/// Sound is muted
		/// </summary>
		public bool IsMuted {
			get {
				return CurrentLFSState.IsMuted;
			}
			set {
				CheckConnection();
				writer.Send(Packets.PacketFactory.GetStateModPacket(Packets.Support.StateModHelper.Mute, value));
			}
		}

		/// <summary>
		/// The version of the currently connected to LFS instance. Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		public LFSVersion Version {
			get {
				CheckConnection();
				return version;
			}
		}
		/// <summary>
		/// Is Multi Car Info tracking turned on
		/// </summary>
		public bool MultiCarTrackingOn {
			get {
				CheckConnection();
				return configuration.MultiCarTracking;
			}
		}
		/// <summary>
		/// Is Node Lap tracking turned on
		/// </summary>
		public bool NodeLapTrackingOn {
			get {
				CheckConnection();
				return configuration.NodeLapTracking;
			}
		}
		/// <summary>
		/// The number of milliseconds between tracking messages sent by the host.
		/// 0 means no tracking messages, otherwise 50 is the smallest amount.
		/// </summary>
		public int TrackingInterval {
			get {
				CheckConnection();
				return trackInterval;
			}
			set {
				CheckConnection();
				if (value < 50 && value != 0) {
					throw new ArgumentException("Interval has to be 0 or greater than 50 milliseconds");
				}
				trackInterval = value;
				writer.Send(Packets.PacketFactory.GetRaceTrackingIntervalRequestPacket(trackInterval));
			}
		}

		/// <summary>
		/// The State of the object
		/// </summary>
		public HandlerState State {
			get { return state; }
			protected set {
				HandlerState previous = state;
				state = value;
				if (previous != state) {
					OnStateChanged(previous, state);
				}
			}
		}
		#endregion

		#region Methods ###############################################################################
		/// <summary>
		/// Initialize the connection to the LFS Host. May throw one of the following exceptions: <see cref="InSimHandlerException.AlreadyInitialized"/>, <see cref="InSimHandlerException.NoVersion"/>, <see cref="InSimHandlerException.NoInitialState"/> or <see cref="InSimHandlerException.ConnectionFailed"/>
		/// </summary>
		public void Initialize() {
			State = HandlerState.Initiaized;

			processPacketsQueue = new Thread(new ThreadStart(ProccessPackets));
			processPacketsQueue.Priority = ThreadPriority.AboveNormal;
			processPacketsQueue.Start();

			Initialize(10);
		}

		void IInSimRelayHandler.Initialize() {
			log.Debug("Calling relay's initialize");
			InitializeRelay(relayHost, relayPort);
		}

		/// <summary>
		/// Initialize the connection to the LFS Host. May throw one of the following exceptions: <see cref="InSimHandlerException.AlreadyInitialized"/>, <see cref="InSimHandlerException.NoVersion"/>, <see cref="InSimHandlerException.NoInitialState"/> or <see cref="InSimHandlerException.ConnectionFailed"/>
		/// </summary>
		/// <param name="wait">Maximum seconds to wait for initialization to complete. A value of 0 waits indefinitely.</param>
		public virtual void Initialize(int wait) {
			Initialize(wait, false);
		}

		/// <summary>
		/// Send a text message to the Host. Throws <see cref="InSimHandlerException.NotConnected"/>.
		/// </summary>
		/// <param name="bytes"></param>
		public void SendMessage(byte[] bytes) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetMessagePacket(bytes));
		}

		/// <summary>
		/// Send a text message to the Host. Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <param name="message"></param>
		public void SendMessage(string message) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetMessagePacket(message));
		}

		/// <summary>
		/// Send a text message to a specific client connection. (Only works when connected to the Host)
		/// </summary>
		/// <param name="message"></param>
		/// <param name="connectionId"></param>
		public void SendMessageToConnection(string message, byte connectionId) {
			writer.Send(Packets.PacketFactory.GetMessageToConnectionPacket(message, connectionId));
		}

		/// <summary>
		/// Send a text message to a specific client. (Only works when connected to the Host)
		/// </summary>
		/// <param name="message"></param>
		/// <param name="playerId"></param>
		public void SendMessageToPlayer(string message, byte playerId) {
			writer.Send(Packets.PacketFactory.GetMessageToPlayerPacket(message, playerId));
		}

		/// <summary>
		/// Send a text message only to the local client. Can be used in conjuntion with a sound effect
		/// </summary>
		/// <param name="message"></param>
		/// <param name="soundEffect"></param>
		public void SendMessageToLocalClient(string message, Enums.Sound soundEffect) {
			writer.Send(Packets.PacketFactory.GetLocalMessagePacket(message, soundEffect));
		}

		/// <summary>
		/// Send a Keypress object to LFS
		/// </summary>
		/// <param name="keyPress"></param>
		public void SendKey(KeyPress keyPress) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetSingleKeyPacket(keyPress));
		}

		byte IInSimRelayHandler.RequestRelayHostlist() {
			return SendRequestPacket(Packets.PacketFactory.GetRelayHostlistRequestPacket());
		}

		byte IInSimRelayHandler.ConnectToHost(string hostname) {
			return ((IInSimRelayHandler)this).ConnectToHost(hostname, null, null);
		}

		byte IInSimRelayHandler.ConnectToHost(string hostname, string adminPass) {
			return ((IInSimRelayHandler)this).ConnectToHost(hostname, adminPass, null);
		}

		byte IInSimRelayHandler.ConnectToHost(string hostname, string adminPass, string spectatorPass) {
			return SendRequestPacket(Packets.PacketFactory.GetRelayHostSelectPacket(hostname, adminPass, spectatorPass));
		}

		/// <summary>
		/// Requests current State from LFS. Need to be listening to <see cref="LFSState"/> event for response.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestState() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetStateRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Request the <see cref="RaceTrackRaceStart"/> events. Need to be listening to <see cref="RaceTrackRaceStart"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns></returns>
		public byte RequestRaceTrackingStart() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetRaceTrackingStartRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Request all <see cref="RaceTrackResult"/> events. Need to be listening to <see cref="RaceTrackResult"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestRaceTrackingResult() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetRaceTrackingResultRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Request all <see cref="RaceTrackConnection"/> events. Need to be listening to <see cref="RaceTrackConnection"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestConnectionInfo() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetRaceTrackingConnectionRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Request all <see cref="RaceTrackPlayer"/> events. Need to be listening to <see cref="RaceTrackPlayer"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestPlayerInfo() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetRaceTrackingPlayerRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Request a <see cref="RaceTrackNodeLap"/> event. Need to be listening to <see cref="RaceTrackNodeLap"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestRaceTrackingNodeLap() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetRaceTrackingNodeLapRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Request a <see cref="RaceTrackMultiCarInfo"/> event. Need to be listening to <see cref="RaceTrackMultiCarInfo"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestRaceTrackingMultiCarInfo() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetRaceTrackingMultiCarInfoRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Request a <see cref="Multiplayer"/> event. Need to be listening to <see cref="LFSMultiplayer"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestMultiplayer() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetMultiplayerRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Requests a <see cref="FullMotion.LiveForSpeed.InSim.Events.CameraPosition"/> event. Need to be listening to
		/// <see cref="LFSCameraPosition"/>. Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestCameraPosition() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetCameraPositionRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Requests a <see cref="FullMotion.LiveForSpeed.InSim.Events.RaceTrackReorder"/> event. Need to be listening to
		/// <see cref="RaceTrackReorder"/>. Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestGridOrder() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetReorderRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Cancel a currently vote in progress. Should see a <see cref="VoteCancel"/> event come into
		/// the <see cref="Vote"/> event handler.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		public void CancelVote() {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetVoteCancelPacket());
		}

		/// <summary>
		/// Request that all buttons created by this handler be cleared either locally or on all clients.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <param name="local"></param>
		public void ClearButtons(bool local) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetButtonClearPacket((byte)((local) ? 0 : 255)));
		}

		/// <summary>
		/// Request that all buttons created by this handler be cleared on a specific connection.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <param name="connectionId"></param>
		public void ClearButtons(byte connectionId) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetButtonClearPacket(connectionId));
		}

		/// <summary>
		/// Send a button to be created locally or globally
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <param name="button"></param>
		/// <param name="local"></param>
		/// <returns>requestId</returns>
		public byte ShowButton(LfsButton button, bool local) {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = button.GetPacket((byte)((local) ? 0 : 255));
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Send a button to be created on a specific connection.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <param name="button"></param>
		/// <param name="connectionId"></param>
		/// <returns>requestId</returns>
		public byte ShowButton(LfsButton button, byte connectionId) {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = button.GetPacket(connectionId);
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Delete a specific button by Id locally or globally
		/// </summary>
		/// <param name="buttonId"></param>
		/// <param name="local"></param>
		public void DeleteButton(byte buttonId, bool local) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetButtonDeletePacket(buttonId, (byte)((local) ? 0 : 255)));
		}

		/// <summary>
		/// Delete a specific button by Id on a specific connection
		/// </summary>
		/// <param name="buttonId"></param>
		/// <param name="connectionId"></param>
		public void DeleteButton(byte buttonId, byte connectionId) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetButtonDeletePacket(buttonId, connectionId));
		}

		/// <summary>
		/// Delete a specific button locally or globally
		/// </summary>
		/// <param name="button"></param>
		/// <param name="local"></param>
		public void DeleteButton(LfsButton button, bool local) {
			DeleteButton(button.ButtonId, local);
		}

		/// <summary>
		/// Delete a specific button on a specific connection
		/// </summary>
		/// <param name="button"></param>
		/// <param name="connectionId"></param>
		public void DeleteButton(LfsButton button, byte connectionId) {
			DeleteButton(button.ButtonId, connectionId);
		}

		/// <summary>
		/// Set the game's screen mode.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <param name="width">Screen width in pixels</param>
		/// <param name="height">Screen height in pixels</param>
		/// <param name="refreshRate">Refresh Rate in Hertz (LFS picks closest match), 0 retains present refresh rate</param>
		/// <param name="screenDepth16">True for 16-bit mode, otherwise 32-bit mode</param>
		public void SetScreenMode(int width, int height, int refreshRate, bool screenDepth16) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetScreenModePacket(width, height, refreshRate, screenDepth16));
		}

		/// <summary>
		/// Switch the game's mode to Windowed at the present resolution
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		public void SetWindowedMode() {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetScreenModePacket(0, 0, 0, false));
		}

		/// <summary>
		/// You can send the grid order LFS before a race start, to specify the starting order.
		/// It may be a good idea to avoid conflict by using /start=fixed (LFS setting).
		/// Alternatively, you can leave the LFS setting, but make sure you send your order
		/// AFTER you receive the VoteAction.  LFS does its default grid reordering at the same time
		/// as it sends the VoteAction and you can override this by sending your grid order.
		/// </summary>
		/// <param name="orderedPlayerIds"></param>
		public void SetGridOrder(byte[] orderedPlayerIds) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetGridOrderPacket(orderedPlayerIds));
		}

		/// <summary>
		/// Set the Camera to focus on the identified car
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <param name="playerId">Unique Id of the player to watch</param>
		/// <param name="camera"></param>
		public void SetCarCamera(byte playerId, Enums.View camera) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetCarCameraRequest(playerId, camera));
		}

		/// <summary>
		/// Set the camera using a <see cref="CameraPositionInfo"/> object, a blank instance of which
		/// can be retrieved from the <see cref="GetCameraPositionInfo"/>. Alternately, an instance
		/// initialized to the current settings can be retrieved from the <see cref="CameraPosition"/>
		/// event.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <param name="info"></param>
		public void SetCamera(CameraPositionInfo info) {
			CheckConnection();
			writer.Send(info.GetPacket());
		}

		/// <summary>
		/// A blank instance for use with <see cref="SetCamera"/>. Alternately, an instance
		/// initialized to the current settings can be retrieved from the <see cref="CameraPosition"/>
		/// event.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns></returns>
		public CameraPositionInfo GetCameraPositionInfo() {
			CheckConnection();
			return new CameraPositionInfo();
		}

		/// <summary>
		/// Stop the race time. This is not the same as pausing the game, as it will completely stop
		/// the processing of time in LFS. Primarily meant for controlling Replays.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		public void RaceTimeStop() {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetRaceTimeControlPacket(false));
		}

		/// <summary>
		/// Start race time back up, after <see cref="RaceTimeStop"/> has been called.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		public void RaceTimeContinue() {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetRaceTimeControlPacket(true));
		}

		/// <summary>
		/// Step the race forward by some amount of time, while race time is stopped with
		/// <see cref="RaceTimeStop"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <param name="stepTime"></param>
		public void RaceTimeStep(TimeSpan stepTime) {
			CheckConnection();
			writer.Send(Packets.PacketFactory.GetRaceTimeControlPacket((int)(stepTime.TotalMilliseconds / 10)));
		}

		/// <summary>
		/// Request the current Time in the race via <see cref="RaceTime"/> event. Need to be
		/// listening to <see cref="LFSRaceTime"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestRaceTime() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetRaceTimeRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Request information about the current Autocross setup. Need to be listening to <see cref="AutocrossInfo"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns></returns>
		public byte RequestAutocrossInfo() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetAutocrossInfoRequestPacket();
			writer.Send(packet);
			return packet.RequestId;
		}

		/// <summary>
		/// Request a ping response from LFS. Need to be listening to <see cref="Ping"/>.
		/// Throws <see cref="InSimHandlerException.NotConnected"/>
		/// </summary>
		/// <returns>requestId</returns>
		public byte RequestPing() {
			CheckConnection();
			Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetPingPacket();
			writer.Send(packet);
			return packet.RequestId;
		}
		/// <summary>
		/// Disconnect from LFS Host
		/// </summary>
		public void Close() {
			if (State == HandlerState.Closed) {
				return;
			}

			try {
				if (reader != null && configuration.UseTCP) {
					((InSimTcpReaderWriter)reader).ConnectionClosed -= new EventHandler(readerWriter_ConnectionClosed);
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.Assert(false, ex.Message);
			}

			try {
				if (!IsDisposed) {
					CheckConnection();
				}
				if (writer != null) {
					writer.Close();
				} else {
					System.Diagnostics.Debug.Assert(State == HandlerState.New);
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.Assert(false, ex.Message);
			}

			if (reader != null) {
				reader.Stop();
			} else {
				System.Diagnostics.Debug.Assert(State == HandlerState.New);
			}
			State = HandlerState.Closed;
		}
		#endregion

		#region Private Methods #######################################################################
		private byte SendRequestPacket(Packets.IClientLfsInSimPacket packet) {
			CheckConnection();
			writer.Send(packet);
			return packet.RequestId;
		}

		private void InitializeRelay(string host, int port) {
			try {
				log.DebugFormat("initializing relay connection to {0}:{1}", host, port);
				InSimTcpReaderWriter readerWriter = new InSimTcpReaderWriter(host, port, 0);
				readerWriter.ConnectionClosed += new EventHandler(readerWriter_ConnectionClosed);
				reader = readerWriter;
				writer = readerWriter;

				reader.InSimPacketReceived += new InSimPacketReceivedEventHandler(reader_InSimPacketReceived);
			} catch (InSimHandlerException e) {
				reader.Stop();
				throw e;
			}

			State = HandlerState.Connected;
		}

		private void Initialize(int wait, bool isReconnect) {
			if (!isReconnect && State == HandlerState.Connected) {
				throw new InSimHandlerException.AlreadyInitialized();
			}
			log.Debug("Initializing");

			if (configuration.UseTCP) {
				if (reader != null) {
					Debug.Assert(State != HandlerState.New);
					((InSimTcpReaderWriter)reader).ConnectionClosed -= new EventHandler(readerWriter_ConnectionClosed);
				}

				InSimTcpReaderWriter readerWriter = new InSimTcpReaderWriter(configuration.LFSHost, configuration.LFSHostPort, configuration.ReplyPort);
				readerWriter.ConnectionClosed += new EventHandler(readerWriter_ConnectionClosed);
				reader = readerWriter;
				writer = readerWriter;
			} else {
				writer = new InSimWriter(configuration.LFSHost, configuration.LFSHostPort);
				reader = new InSimReader(configuration.ReplyPort);
			}

			if (wait == 0) {
				initWait = int.MaxValue;
			} else {
				initWait = wait * 1000;
			}
			try {
				InitLFS();
				if (IsDisposed) {
					return;
				}
			} catch (InSimHandlerException e) {
				reader.Stop();
				throw e;
			}
			trackInterval = configuration.TrackingInterval;
			CheckState();
		}

		private void InitLFS() {
			reader.InSimPacketReceived += new InSimPacketReceivedEventHandler(reader_InSimPacketReceived);
			writer.Send(configuration.GetInitPacket());
			awaitingVersion.Reset();
			if (!awaitingVersion.WaitOne(initWait, true)) {
				throw new InSimHandlerException.NoVersion();
			}
			if (IsDisposed) {
				log.Debug("got disposed since our connection attempt, so clean up");
				try {
					Close();
				} catch { }
			}
		}

		private void CheckState() {
			if (currentState == null) {
				awaitingState.Reset();
				Packets.IClientLfsInSimPacket packet = Packets.PacketFactory.GetStateRequestPacket();
				writer.Send(packet);
				// we're willing to wait 3 seconds before we give up waiting on a Version on initialization.
				// if it comes in later, we still catch it, but we don't hold up initialization for it
				if (!awaitingState.WaitOne(3000, true)) {
					throw new InSimHandlerException.NoInitialState();
				}
			}
			if (IsDisposed) {
				log.Debug("got disposed since our connection attempt, so clean up");
				try {
					Close();
				} catch { }
			}
		}

		private void CheckConnection() {
			if (IsDisposed) {
				log.Debug("got disposed since our last communication, so clean up");
				try {
					Close();
				} catch { }
			}
			if (State != HandlerState.Connected) {

				throw new InSimHandlerException.NotConnected();
			}
		}
		#endregion

		#region Event Handlers ########################################################################
		protected void readerWriter_ConnectionClosed(object sender, EventArgs e) {
			if (configuration.AutoReconnect && !IsDisposed) {
				for (int i = 0; i < configuration.ReconnectRetry; i++) {
					log.DebugFormat("trying to reconnect: {0}", i);
					try {
						Initialize(10, true);
						log.Debug("reconnected");
						return;
					} catch { }
				}
			}
			log.Debug("connection closed on us");
			State = HandlerState.Closed;
		}

		void pingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			// Do we need this?
			this.RequestPing();
		}

		private void reader_InSimPacketReceived(ILfsReader sender, InSimPacketEventArgs e) {
			lock (packets) {
				packets.Enqueue(e.Packet);
			}
		}

		private void ProccessPackets() {
			ILfsInSimPacket packet = null;

			while (State!=HandlerState.Closed) {
				lock (packets) {
					if (packets.Count != 0) {
						packet = packets.Dequeue();
					} else {
						packet = null;
					}
				}

				if (packet != null) {
					FireInSimEvent(packet);
				} else {
					Thread.Sleep(5);
				}
				
			}
		}

		private void FireInSimEvent(ILfsInSimPacket packet) {
			log.Debug("Type: " + packet.PacketType);
			switch (packet.PacketType) {
				case Enums.ISP.AXI:
				OnAutocrossInfo((Packets.IS_AXI)packet);
				break;
				case Enums.ISP.AXO:
				OnAutocrossObject((Packets.IS_AXO)packet);
				break;
				case Enums.ISP.BFN: {
					log.Debug("SubT: " + ((Packets.IS_BFN)packet).SubT);
					switch (((Packets.IS_BFN)packet).SubT) {
						case Enums.BFN.CLEAR:
						// should never receive this
						break;
						case Enums.BFN.DEL_BTN:
						// should never receive this
						break;
						case Enums.BFN.REQUEST:
						OnButtonsRequest((Packets.IS_BFN)packet);
						break;
						case Enums.BFN.USER_CLEAR:
						OnButtonsCleared((Packets.IS_BFN)packet);
						break;
					}
					break;
				}
				case Enums.ISP.BTC:
				OnButtonClick((Packets.IS_BTC)packet);
				break;
				case Enums.ISP.BTN:
				// should never receive this
				break;
				case Enums.ISP.BTT:
				OnButtonType((Packets.IS_BTT)packet);
				break;
				case Enums.ISP.CCH:
				OnRaceTrackPlayerCameraChange((Packets.IS_CCH)packet);
				break;
				case Enums.ISP.CNL:
				OnRaceTrackConnectionLeave((Packets.IS_CNL)packet);
				break;
				case Enums.ISP.CPP:
				OnLFSCameraPosition((Packets.IS_CPP)packet);
				break;
				case Enums.ISP.CPR:
				OnRaceTrackPlayerRename((Packets.IS_CPR)packet);
				break;
				case Enums.ISP.CRS:
				OnRaceTrackCarReset((Packets.IS_CRS)packet);
				break;
				case Enums.ISP.FIN:
				OnRaceTrackPlayerFinish((Packets.IS_FIN)packet);
				break;
				case Enums.ISP.FLG:
				OnRaceTrackPlayerFlag((Packets.IS_FLG)packet);
				break;
				case Enums.ISP.III:
				OnInSimInfo((Packets.IS_III)packet);
				break;
				case Enums.ISP.ISI:
				break;
				case Enums.ISP.ISM:
				OnLFSMultiplayer((Packets.IS_ISM)packet);
				break;
				case Enums.ISP.LAP:
				OnRaceTrackPlayerLap((Packets.IS_LAP)packet);
				break;
				case Enums.ISP.MCI:
				OnRaceTrackMultiCarInfo((Packets.IS_MCI)packet);
				break;
				case Enums.ISP.MOD:
				break;
				case Enums.ISP.MSL:
				break;
				case Enums.ISP.MSO:
				OnLFSMessage((Packets.IS_MSO)packet);
				break;
				case Enums.ISP.MST:
				break;
				case Enums.ISP.MSX:
				break;
				case Enums.ISP.MTC:
				break;
				case Enums.ISP.NCN:
				OnRaceTrackConnection((Packets.IS_NCN)packet);
				break;
				case Enums.ISP.NLP:
				OnRaceTrackNodeLap((Packets.IS_NLP)packet);
				break;
				case Enums.ISP.NPL:
				OnRaceTrackPlayer((Packets.IS_NPL)packet);
				break;
				case Enums.ISP.PEN:
				OnRaceTrackPlayerPenalty((Packets.IS_PEN)packet);
				break;
				case Enums.ISP.PFL:
				OnRaceTrackPlayerChange((Packets.IS_PFL)packet);
				break;
				case Enums.ISP.PIT:
				OnRaceTrackPlayerPitStopBegin((Packets.IS_PIT)packet);
				break;
				case Enums.ISP.PLA:
				OnRaceTrackPlayerPitlane((Packets.IS_PLA)packet);
				break;
				case Enums.ISP.PLL:
				OnRaceTrackPlayerLeave((Packets.IS_PLL)packet);
				break;
				case Enums.ISP.PLP:
				OnRaceTrackPlayerPits((Packets.IS_PLP)packet);
				break;
				case Enums.ISP.PSF:
				OnRaceTrackPlayerPitStopFinish((Packets.IS_PSF)packet);
				break;
				case Enums.ISP.REO:
				OnRaceTrackReorder((Packets.IS_REO)packet);
				break;
				case Enums.ISP.RES:
				OnRaceTrackPlayerResult((Packets.IS_RES)packet);
				break;
				case Enums.ISP.RST:
				OnRaceTrackRaceStart((Packets.IS_RST)packet);
				break;
				case Enums.ISP.SCC:
				break;
				case Enums.ISP.SCH:
				break;
				case Enums.ISP.SFP:
				break;
				case Enums.ISP.SMALL: {
					log.Debug("SubT: " + ((Packets.IS_SMALL)packet).SubT);
					switch (((Packets.IS_SMALL)packet).SubT) {
						case Enums.SMALL.NLI:
						break;
						case Enums.SMALL.NONE:
						break;
						case Enums.SMALL.RTP:
						OnLFSRaceTime((Packets.IS_SMALL)packet);
						break;
						case Enums.SMALL.SSG:
						break;
						case Enums.SMALL.SSP:
						break;
						case Enums.SMALL.STP:
						break;
						case Enums.SMALL.TMS:
						break;
						case Enums.SMALL.VTA:
						OnVoteFinal((Packets.IS_SMALL)packet);
						break;
					}
					break;
				}
				case Enums.ISP.SPX:
				OnRaceTrackPlayerSplitTime((Packets.IS_SPX)packet);
				break;
				case Enums.ISP.STA:
				OnLFSState((Packets.IS_STA)packet);
				break;
				case Enums.ISP.TINY: {
					log.Debug("SubT: " + ((Packets.IS_TINY)packet).SubT);
					switch (((Packets.IS_TINY)packet).SubT) {
						case Enums.TINY.AXC:
						OnAutocrossCleared((Packets.IS_TINY)packet);
						break;
						case Enums.TINY.AXI:
						break;
						case Enums.TINY.CLOSE:
						break;
						case Enums.TINY.CLR:
						OnRaceTrackClearRace((Packets.IS_TINY)packet);
						break;
						case Enums.TINY.GTH:
						break;
						case Enums.TINY.ISM:
						break;
						case Enums.TINY.MCI:
						break;
						case Enums.TINY.MPE:
						OnLFSMultiplayer((Packets.IS_TINY)packet);
						break;
						case Enums.TINY.NCN:
						break;
						case Enums.TINY.NLP:
						break;
						case Enums.TINY.NONE:
						// keep alive
						writer.Send(Packets.PacketFactory.GetKeepAlivePacket());
						break;
						case Enums.TINY.NPL:
						break;
						case Enums.TINY.PING:
						break;
						case Enums.TINY.REN:
						OnRaceTrackRaceEnd((Packets.IS_TINY)packet);
						break;
						case Enums.TINY.REO:
						break;
						case Enums.TINY.REPLY:
						OnPing((Packets.IS_TINY)packet);
						break;
						case Enums.TINY.RES:
						break;
						case Enums.TINY.RST:
						break;
						case Enums.TINY.SCP:
						break;
						case Enums.TINY.SST:
						break;
						case Enums.TINY.VER:
						break;
						case Enums.TINY.VTC:
						OnVoteCancel((Packets.IS_TINY)packet);
						break;
					}
					break;
				}
				case Enums.ISP.TOC:
				OnRaceTrackCarTakeover((Packets.IS_TOC)packet);
				break;
				case Enums.ISP.VER:
				OnLFSVersion((Packets.IS_VER)packet);
				break;
				case Enums.ISP.VTN:
				OnVote((Packets.IS_VTN)packet);
				break;
				case Enums.ISP.IRP_ERR:
				OnRelayError((Packets.IR_ERR)packet);
				break;
				case Enums.ISP.IRP_HOS:
				OnRelayHostlist((Packets.IR_HOS)packet);
				break;
			}
		}

		#endregion

		#region Events ################################################################################
		#region StateChanged
		/// <summary>
		/// Notification that the state of the handler has changed
		/// </summary>
		public event StateChangeHandler StateChanged;

		private void OnStateChanged(HandlerState previous, HandlerState current) {
			log.Debug("OnStateChanged called");
			if (StateChanged != null) {
				StateChangeEventArgs eventArgs = new StateChangeEventArgs(previous, current);
				StateChanged(this, eventArgs);
			}
		}
		#endregion
		#region AutocrossCleared
		/// <summary>
		/// Notification that all objects have been cleared from the current layout
		/// </summary>
		public event AutocrossClearedHandler AutocrossCleared;

		private void OnAutocrossCleared(Packets.IS_TINY packet) {
			log.Debug("OnAutocrossCleared called");
			if (AutocrossCleared != null) {
				AutocrossCleared eventArgs = new AutocrossCleared(packet);
				AutocrossCleared(this, eventArgs);
			}
		}
		#endregion
		#region AutocrossInfo
		/// <summary>
		/// Response to a Autocross information request
		/// </summary>
		public event AutocrossInfoHandler AutocrossInfo;

		private void OnAutocrossInfo(Packets.IS_AXI packet) {
			log.Debug("OnAutocrossInfo called");
			if (AutocrossInfo != null) {
				AutocrossInfo eventArgs = new AutocrossInfo(packet);
				AutocrossInfo(this, eventArgs);
			}
		}
		#endregion
		#region AutocrossObject
		/// <summary>
		/// Notification that a player has hit an autocross object
		/// </summary>
		public event AutocrossObjectHandler AutocrossObject;

		private void OnAutocrossObject(Packets.IS_AXO packet) {
			log.Debug("OnAutocrossObject called");
			if (AutocrossObject != null) {
				AutocrossObject eventArgs = new AutocrossObject(packet);
				AutocrossObject(this, eventArgs);
			}
		}
		#endregion
		#region ButtonType
		/// <summary>
		/// Notification of a custom Dialog Button having text typed in
		/// </summary>
		public event ButtonTypeHandler ButtonType;

		private void OnButtonType(Packets.IS_BTT packet) {
			log.Debug("OnButtonType called");
			if (ButtonType != null) {
				ButtonType eventArgs = new ButtonType(packet);
				ButtonType(this, eventArgs);
			}
		}
		#endregion
		#region ButtonClick
		/// <summary>
		/// A button click notification for a custom LFS button
		/// </summary>
		public event ButtonClickHandler ButtonClick;

		private void OnButtonClick(Packets.IS_BTC packet) {
			log.Debug("OnButtonClick called");
			if (ButtonClick != null) {
				ButtonClick eventArgs = new ButtonClick(packet);
				ButtonClick(this, eventArgs);
			}
		}
		#endregion
		#region ButtonRequest
		/// <summary>
		/// Notification for a client requesting custom buttons
		/// </summary>
		public event ButtonsRequestHandler ButtonsRequest;
		[Obsolete("Replaced with ButtonsRequest in 0.18+")]
		public event ButtonRequestHandler ButtonRequest;

		private void OnButtonsRequest(Packets.IS_BFN packet) {
			log.Debug("OnButtonRequest called");
			if (ButtonsRequest != null) {
				ButtonsRequest eventArgs = new ButtonsRequest(packet);
				ButtonsRequest(this, eventArgs);
			}
			if (ButtonRequest != null) {
				ButtonRequest eventArgs = new ButtonRequest(packet);
				ButtonRequest(this, eventArgs);
			}
		}

		#endregion
		#region ButtonsCleared
		/// <summary>
		/// Notification that a client has cleared its buttons
		/// </summary>
		public event ButtonsClearedHandler ButtonsCleared;

		private void OnButtonsCleared(Packets.IS_BFN packet) {
			log.Debug("OnButtonsCleared called");
			if (ButtonsCleared != null) {
				ButtonsCleared eventArgs = new ButtonsCleared(packet);
				ButtonsCleared(this, eventArgs);
			}
		}
		#endregion
		#region InSimInfo
		/// <summary>
		/// Notification that a connection has requested InSim information
		/// </summary>
		public event InfoEventHandler Info;

		private void OnInSimInfo(Packets.IS_III packet) {
			log.Debug("OnInSimInfo called");
			if (Info != null) {
				Info eventArgs = new Info(packet);
				Info(this, eventArgs);
			}
		}
		#endregion
		#region Ping
		/// <summary>
		/// Response to a <see cref="RequestPing"/>
		/// </summary>
		public event PingEventHandler Ping;

		private void OnPing(Packets.IS_TINY packet) {
			log.Debug("OnPing called");
			if (Ping != null) {
				Ping eventArgs = new Ping(packet);
				Ping(this, eventArgs);
			}
		}
		#endregion
		#region Message
		/// <summary>
		/// A message received from LFS
		/// </summary>
		public event MessageEventHandler LFSMessage;

		private void OnLFSMessage(Packets.IS_MSO packet) {
			log.Debug("OnLFSMessage called");
			if (LFSMessage != null) {
				Message eventArgs = new Message(packet, configuration.UseSplitMessages);
				LFSMessage(this, eventArgs);
			}
		}
		#endregion
		#region State
		/// <summary>
		/// A state event received from LFS
		/// </summary>
		public event StateEventHandler LFSState;

		private void OnLFSState(Packets.IS_STA packet) {
			log.Debug("OnLFSState called");
			State eventArgs = new State(packet);
			if (currentState == null) {
				lastState = eventArgs;
			} else {
				lastState = currentState;
			}
			currentState = eventArgs;
			awaitingState.Set();

			if (LFSState != null) {
				LFSState(this, eventArgs);
			}
		}
		#endregion
		#region Version/RelayHostConnected
		private event RelayHostConnectedEventHandler Internal_RelayHostConnected;
		event RelayHostConnectedEventHandler IInSimRelayHandler.RelayHostConnected {
			add { Internal_RelayHostConnected += value; }
			remove { Internal_RelayHostConnected += value; }
		}

		private void OnLFSVersion(Packets.IS_VER packet) {
			log.Debug("OnLFSVersion called");
			version = new LFSVersion(packet);
			if (Internal_RelayHostConnected != null) {
				Internal_RelayHostConnected(this, version);
			}
			log.Debug(String.Format("Expected version: {0}, received version: {1}", LibVersion.INSIM_SERIAL, version.Serial));

			State = HandlerState.Connected;

			awaitingVersion.Set();
		}
		#endregion
		#region RaceTrack
		/// <summary>
		/// A race tracking event received from LFS
		/// </summary>
		public event RaceTrackEventHandler RaceTrack;

		private void OnRaceTrack(RaceTrackEvent eventArgs) {
			log.Debug("OnRaceTrack called");
			RaceTrack(this, eventArgs);
		}
		#endregion
		#region RaceTrackClearRace
		/// <summary>
		/// A race tracking event received from LFS regarding the clearing of the current grid
		/// </summary>
		public event RaceTrackClearRaceHandler RaceTrackClearRace;

		private void OnRaceTrackClearRace(Packets.IS_TINY packet) {
			log.Debug("reader_RaceTrackAckClearRacePacket called");
			if (RaceTrack != null) {
				RaceTrackClearRace eventArgs = new RaceTrackClearRace(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackClearRace != null) {
				RaceTrackClearRace eventArgs = new RaceTrackClearRace(packet);
				RaceTrackClearRace(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackConnectionLeave
		/// <summary>
		/// A race tracking event received from LFS regarding a connection leaving the server
		/// </summary>
		public event RaceTrackConnectionLeaveHandler RaceTrackConnectionLeave;

		private void OnRaceTrackConnectionLeave(Packets.IS_CNL packet) {
			log.Debug("OnRaceTrackConnectionLeave called");
			if (RaceTrack != null) {
				RaceTrackConnectionLeave eventArgs = new RaceTrackConnectionLeave(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackConnectionLeave != null) {
				RaceTrackConnectionLeave eventArgs = new RaceTrackConnectionLeave(packet);
				RaceTrackConnectionLeave(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerLap
		/// <summary>
		/// A race tracking event received from LFS regarding the lap time for a player
		/// </summary>
		public event RaceTrackPlayerLapHandler RaceTrackPlayerLap;

		private void OnRaceTrackPlayerLap(Packets.IS_LAP packet) {
			log.Debug("OnRaceTrackPlayerLap called");
			if (RaceTrack != null) {
				RaceTrackPlayerLap eventArgs = new RaceTrackPlayerLap(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerLap != null) {
				RaceTrackPlayerLap eventArgs = new RaceTrackPlayerLap(packet);
				RaceTrackPlayerLap(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackConnection
		/// <summary>
		/// A race tracking event received from LFS regarding a (possibly new) connection
		/// </summary>
		public event RaceTrackNewConnectionHandler RaceTrackConnection;

		private void OnRaceTrackConnection(Packets.IS_NCN packet) {
			log.Debug("OnRaceTrackConnection called");
			if (RaceTrack != null) {
				RaceTrackConnection eventArgs = new RaceTrackConnection(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackConnection != null) {
				RaceTrackConnection eventArgs = new RaceTrackConnection(packet);
				RaceTrackConnection(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayer
		/// <summary>
		/// A race tracking event received from LFS regarding a (possibly new) player
		/// </summary>
		public event RaceTrackPlayerHandler RaceTrackPlayer;

		private void OnRaceTrackPlayer(Packets.IS_NPL packet) {
			log.Debug("OnRaceTrackPlayer called");
			if (RaceTrack != null) {
				RaceTrackPlayer eventArgs = new RaceTrackPlayer(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayer != null) {
				RaceTrackPlayer eventArgs = new RaceTrackPlayer(packet);
				RaceTrackPlayer(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerChange
		/// <summary>
		/// A race tracking event received from LFS regarding a change in a player's options
		/// </summary>
		public event RaceTrackPlayerChangeHandler RaceTrackPlayerChange;

		private void OnRaceTrackPlayerChange(Packets.IS_PFL packet) {
			log.Debug("OnRaceTrackPlayerChange called");
			if (RaceTrack != null) {
				RaceTrackPlayerChange eventArgs = new RaceTrackPlayerChange(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerChange != null) {
				RaceTrackPlayerChange eventArgs = new RaceTrackPlayerChange(packet);
				RaceTrackPlayerChange(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerFlag
		/// <summary>
		/// A race tracking event received from LFS regarding a player flag event
		/// </summary>
		public event RaceTrackPlayerFlagHandler RaceTrackPlayerFlag;

		private void OnRaceTrackPlayerFlag(Packets.IS_FLG packet) {
			log.Debug("OnRaceTrackPlayerFlag called");
			if (RaceTrack != null) {
				RaceTrackPlayerFlag eventArgs = new RaceTrackPlayerFlag(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerFlag != null) {
				RaceTrackPlayerFlag eventArgs = new RaceTrackPlayerFlag(packet);
				RaceTrackPlayerFlag(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerLeave
		/// <summary>
		/// A race tracking event received from LFS regarding a player leaving the game
		/// </summary>
		public event RaceTrackPlayerLeaveHandler RaceTrackPlayerLeave;

		private void OnRaceTrackPlayerLeave(Packets.IS_PLL packet) {
			log.Debug("OnRaceTrackPlayerLeave called");
			if (RaceTrack != null) {
				RaceTrackPlayerLeave eventArgs = new RaceTrackPlayerLeave(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerLeave != null) {
				RaceTrackPlayerLeave eventArgs = new RaceTrackPlayerLeave(packet);
				RaceTrackPlayerLeave(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerPitlane
		/// <summary>
		/// A race tracking event received from LFS regarding a player pitting
		/// </summary>
		public event RaceTrackPlayerPitlaneHandler RaceTrackPlayerPitlane;

		private void OnRaceTrackPlayerPitlane(Packets.IS_PLA packet) {
			log.Debug("OnRaceTrackPlayerPitlane called");
			if (RaceTrack != null) {
				RaceTrackPlayerPitlane eventArgs = new RaceTrackPlayerPitlane(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerPitlane != null) {
				RaceTrackPlayerPitlane eventArgs = new RaceTrackPlayerPitlane(packet);
				RaceTrackPlayerPitlane(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerPits
		/// <summary>
		/// A race tracking event received from LFS regarding a player pitting
		/// </summary>
		public event RaceTrackPlayerPitsHandler RaceTrackPlayerPits;

		private void OnRaceTrackPlayerPits(Packets.IS_PLP packet) {
			log.Debug("OnRaceTrackPlayerPits called");
			if (RaceTrack != null) {
				RaceTrackPlayerPits eventArgs = new RaceTrackPlayerPits(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerPits != null) {
				RaceTrackPlayerPits eventArgs = new RaceTrackPlayerPits(packet);
				RaceTrackPlayerPits(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerPitStopBegin
		/// <summary>
		/// A race tracking event received from LFS regarding a player beginning a pit stop
		/// </summary>
		public event RaceTrackPlayerPitStopBeginHandler RaceTrackPlayerPitStopBegin;

		private void OnRaceTrackPlayerPitStopBegin(Packets.IS_PIT packet) {
			log.Debug("OnRaceTrackPlayerPitStopBegin called");
			if (RaceTrack != null) {
				RaceTrackPlayerPitStopBegin eventArgs = new RaceTrackPlayerPitStopBegin(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerPitStopBegin != null) {
				RaceTrackPlayerPitStopBegin eventArgs = new RaceTrackPlayerPitStopBegin(packet);
				RaceTrackPlayerPitStopBegin(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerPitStopFinish
		/// <summary>
		/// A race tracking event received from LFS regarding a player completing a pit stop
		/// </summary>
		public event RaceTrackPlayerPitStopFinishHandler RaceTrackPlayerPitStopFinish;

		private void OnRaceTrackPlayerPitStopFinish(Packets.IS_PSF packet) {
			log.Debug("OnRaceTrackPlayerPitStopFinish called");
			if (RaceTrack != null) {
				RaceTrackPlayerPitStopFinish eventArgs = new RaceTrackPlayerPitStopFinish(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerPitStopFinish != null) {
				RaceTrackPlayerPitStopFinish eventArgs = new RaceTrackPlayerPitStopFinish(packet);
				RaceTrackPlayerPitStopFinish(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerCameraChange
		/// <summary>
		/// A race tracking event received from LFS regarding a player pitting
		/// </summary>
		public event RaceTrackPlayerCameraChangeHandler RaceTrackPlayerCameraChange;

		private void OnRaceTrackPlayerCameraChange(Packets.IS_CCH packet) {
			log.Debug("OnRaceTrackPlayerCameraChange called");
			if (RaceTrack != null) {
				RaceTrackPlayerCameraChange eventArgs = new RaceTrackPlayerCameraChange(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerCameraChange != null) {
				RaceTrackPlayerCameraChange eventArgs = new RaceTrackPlayerCameraChange(packet);
				RaceTrackPlayerCameraChange(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackCarReset
		/// <summary>
		/// A race tracking event received from LFS regarding a car resetting on track
		/// </summary>
		public event RaceTrackCarResetHandler RaceTrackCarReset;

		private void OnRaceTrackCarReset(Packets.IS_CRS packet) {
			log.Debug("OnRaceTrackCarReset called");
			if (RaceTrack != null) {
				RaceTrackCarReset eventArgs = new RaceTrackCarReset(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackCarReset != null) {
				RaceTrackCarReset eventArgs = new RaceTrackCarReset(packet);
				RaceTrackCarReset(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackCarTakeover
		/// <summary>
		/// A race tracking event received from LFS regarding a driver change
		/// </summary>
		public event RaceTrackCarTakeoverHandler RaceTrackCarTakeover;

		private void OnRaceTrackCarTakeover(Packets.IS_TOC packet) {
			log.Debug("OnRaceTrackCarTakeover called");
			if (RaceTrack != null) {
				RaceTrackCarTakeover eventArgs = new RaceTrackCarTakeover(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackCarTakeover != null) {
				RaceTrackCarTakeover eventArgs = new RaceTrackCarTakeover(packet);
				RaceTrackCarTakeover(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerPenalty
		/// <summary>
		/// A race tracking event received from LFS regarding a player penalty event (given or cleared
		/// </summary>
		public event RaceTrackPlayerPenaltyHandler RaceTrackPlayerPenalty;

		private void OnRaceTrackPlayerPenalty(Packets.IS_PEN packet) {
			log.Debug("OnRaceTrackPlayerPenalty called");
			if (RaceTrack != null) {
				RaceTrackPlayerPenalty eventArgs = new RaceTrackPlayerPenalty(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerPenalty != null) {
				RaceTrackPlayerPenalty eventArgs = new RaceTrackPlayerPenalty(packet);
				RaceTrackPlayerPenalty(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerRename
		/// <summary>
		/// A race tracking event received from LFS regarding a player changing their name
		/// </summary>
		public event RaceTrackPlayerRenameHandler RaceTrackPlayerRename;

		private void OnRaceTrackPlayerRename(Packets.IS_CPR packet) {
			log.Debug("OnRaceTrackPlayerRename called");
			if (RaceTrack != null) {
				RaceTrackPlayerRename eventArgs = new RaceTrackPlayerRename(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerRename != null) {
				RaceTrackPlayerRename eventArgs = new RaceTrackPlayerRename(packet);
				RaceTrackPlayerRename(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackPlayerFinish
		/// <summary>
		/// A race tracking event received from LFS regarding a player finishing the race (not final race result)
		/// </summary>
		public event RaceTrackPlayerFinishHandler RaceTrackPlayerFinish;

		private void OnRaceTrackPlayerFinish(Packets.IS_FIN packet) {
			log.Debug("OnRaceTrackPlayerFinish called");
			if (RaceTrack != null) {
				RaceTrackPlayerFinish eventArgs = new RaceTrackPlayerFinish(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerFinish != null) {
				RaceTrackPlayerFinish eventArgs = new RaceTrackPlayerFinish(packet);
				RaceTrackPlayerFinish(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackRaceEnd
		/// <summary>
		/// A race tracking event received from LFS regarding the end of the race
		/// </summary>
		public event RaceTrackRaceEndHandler RaceTrackRaceEnd;

		private void OnRaceTrackRaceEnd(Packets.IS_TINY packet) {
			log.Debug("OnRaceTrackRaceEnd called");
			if (RaceTrack != null) {
				RaceTrackRaceEnd eventArgs = new RaceTrackRaceEnd(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackRaceEnd != null) {
				RaceTrackRaceEnd eventArgs = new RaceTrackRaceEnd(packet);
				RaceTrackRaceEnd(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackRaceStart
		/// <summary>
		/// A race tracking event received from LFS regarding the start of the race
		/// </summary>
		public event RaceTrackRaceStartHandler RaceTrackRaceStart;

		private void OnRaceTrackRaceStart(Packets.IS_RST packet) {
			log.Debug("OnRaceTrackRaceStart called");
			if (RaceTrack != null) {
				RaceTrackRaceStart eventArgs = new RaceTrackRaceStart(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackRaceStart != null) {
				RaceTrackRaceStart eventArgs = new RaceTrackRaceStart(packet);
				RaceTrackRaceStart(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackReorder
		/// <summary>
		/// A race tracking event received from LFS regarding a grid re-ordering
		/// </summary>
		public event RaceTrackReorderHandler RaceTrackReorder;

		private void OnRaceTrackReorder(Packets.IS_REO packet) {
			log.Debug("OnRaceTrackReorder called");
			if (RaceTrack != null) {
				RaceTrackReorder eventArgs = new RaceTrackReorder(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackReorder != null) {
				RaceTrackReorder eventArgs = new RaceTrackReorder(packet);
				RaceTrackReorder(this, eventArgs);
			}
		}
		#endregion

		#region RaceTrackPlayerResult
		/// <summary>
		/// A race tracking event received from LFS regarding a player's race result
		/// </summary>
		public event RaceTrackPlayerResultHandler RaceTrackPlayerResult;

		private void OnRaceTrackPlayerResult(Packets.IS_RES packet) {
			log.Debug("OnRaceTrackPlayerResult called");
			if (RaceTrack != null) {
				RaceTrackPlayerResult eventArgs = new RaceTrackPlayerResult(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerResult != null) {
				RaceTrackPlayerResult eventArgs = new RaceTrackPlayerResult(packet);
				RaceTrackPlayerResult(this, eventArgs);
			}
		}
		#endregion

		#region RaceTrackPlayerSplitTime
		/// <summary>
		/// A race tracking event received from LFS regarding a split time for a player
		/// </summary>
		public event RaceTrackPlayerSplitTimeHandler RaceTrackPlayerSplitTime;

		private void OnRaceTrackPlayerSplitTime(Packets.IS_SPX packet) {
			log.Debug("OnRaceTrackPlayerSplitTime called");
			if (RaceTrack != null) {
				RaceTrackPlayerSplitTime eventArgs = new RaceTrackPlayerSplitTime(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackPlayerSplitTime != null) {
				RaceTrackPlayerSplitTime eventArgs = new RaceTrackPlayerSplitTime(packet);
				RaceTrackPlayerSplitTime(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackNodeLap
		/// <summary>
		/// A race tracking event received from LFS regarding node lap information
		/// </summary>
		public event RaceTrackNodeLapPacketHandler RaceTrackNodeLap;

		private void OnRaceTrackNodeLap(Packets.IS_NLP packet) {
			log.Debug("OnRaceTrackNodeLap called");
			if (RaceTrack != null) {
				RaceTrackNodeLap eventArgs = new RaceTrackNodeLap(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackNodeLap != null) {
				RaceTrackNodeLap eventArgs = new RaceTrackNodeLap(packet);
				RaceTrackNodeLap(this, eventArgs);
			}
		}
		#endregion
		#region RaceTrackMultiCarInfo
		/// <summary>
		/// A race tracking event received from LFS regarding multi car information
		/// </summary>
		public event RaceTrackMultiCarInfoHandler RaceTrackMultiCarInfo;

		private void OnRaceTrackMultiCarInfo(Packets.IS_MCI packet) {
			log.Debug("OnRaceTrackMultiCarInfo called");
			if (RaceTrack != null) {
				RaceTrackMultiCarInfo eventArgs = new RaceTrackMultiCarInfo(packet);
				OnRaceTrack(eventArgs);
			}
			if (RaceTrackMultiCarInfo != null) {
				RaceTrackMultiCarInfo eventArgs = new RaceTrackMultiCarInfo(packet);
				RaceTrackMultiCarInfo(this, eventArgs);
			}
		}
		#endregion
		#region Multiplayer
		/// <summary>
		/// A multiplayer event received from LFS
		/// </summary>
		public event MultiplayerEventHandler LFSMultiplayer;

		private void OnLFSMultiplayer(Packets.IS_ISM packet) {
			log.Debug("OnLFSMultiplayer called");
			if (LFSMultiplayer != null) {
				Multiplayer eventArgs = new Multiplayer(packet);
				LFSMultiplayer(this, eventArgs);
			}
		}

		private void OnLFSMultiplayer(Packets.IS_TINY packet) {
			log.Debug("OnLFSMultiplayer (end) called");
			if (LFSMultiplayer != null) {
				Multiplayer eventArgs = new Multiplayer(packet);
				LFSMultiplayer(this, eventArgs);
			}
		}
		#endregion
		#region CameraPosition
		/// <summary>
		/// A Camera Position event received from LFS
		/// </summary>
		public event CameraPositionEventHandler LFSCameraPosition;

		private void OnLFSCameraPosition(Packets.IS_CPP packet) {
			log.Debug("OnLFSCameraPosition called");
			if (LFSCameraPosition != null) {
				CameraPosition eventArgs = new CameraPosition(packet);
				LFSCameraPosition(this, eventArgs);
			}
		}
		#endregion
		#region RaceTime
		/// <summary>
		/// A Race Time event received from LFS
		/// </summary>
		public event RaceTimeEventHandler LFSRaceTime;

		private void OnLFSRaceTime(Packets.IS_SMALL packet) {
			log.Debug("OnLFSRaceTime called");
			if (LFSRaceTime != null) {
				RaceTime eventArgs = new RaceTime(packet);
				LFSRaceTime(this, eventArgs);
			}
		}
		#endregion
		#region Voting
		/// <summary>
		/// A multiplayer event received from LFS
		/// </summary>
		public event VoteEventHandler Vote;

		private void OnVote(Packets.IS_VTN packet) {
			log.Debug("OnVote (action) called");
			if (Vote != null) {
				VoteAction eventArgs = new VoteAction(packet);
				Vote(this, eventArgs);
			}
		}

		private void OnVoteCancel(Packets.IS_TINY packet) {
			log.Debug("OnVoteCancel called");
			if (Vote != null) {
				VoteCancel eventArgs = new VoteCancel(packet);
				Vote(this, eventArgs);
			}
		}

		private void OnVoteFinal(Packets.IS_SMALL packet) {
			log.Debug("OnVoteFinal called");
			if (Vote != null) {
				VoteAction eventArgs = new VoteAction(packet);
				Vote(this, eventArgs);
			}
		}
		#endregion

		#region RelayError
		private event RelayErrorEventHandler Internal_RelayError;
		event RelayErrorEventHandler IInSimRelayHandler.RelayError {
			add { Internal_RelayError += value; }
			remove { Internal_RelayError += value; }
		}

		private void OnRelayError(Packets.IR_ERR packet) {
			log.Debug("OnRelayError called");
			if (Internal_RelayError != null) {
				RelayError eventArgs = new RelayError(packet);
				Internal_RelayError(this, eventArgs);
			}
		}
		#endregion

		#region RelayHostlist
		private event RelayHostlistEventHandler Internal_RelayHostlist;
		event RelayHostlistEventHandler IInSimRelayHandler.RelayHostlist {
			add { Internal_RelayHostlist += value; }
			remove { Internal_RelayHostlist += value; }
		}

		private void OnRelayHostlist(Packets.IR_HOS packet) {
			log.Debug("OnRelayHostlist called");
			if (Internal_RelayHostlist != null) {
				RelayHostlist eventArgs = new RelayHostlist(packet);
				Internal_RelayHostlist(this, eventArgs);
			}
		}
		#endregion

		#endregion

		#region IDisposable Members

		public void Dispose() {
			if (!isDisposed) {
				isDisposed = true;
				Close();
				awaitingVersion.Set();
				awaitingState.Set();
			}
		}

		#endregion
	}
}

namespace FullMotion.LiveForSpeed.InSim.EventHandlers
{
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.StateChanged"/> events
	/// </summary>
	public delegate void StateChangeHandler(InSimHandler sender, StateChangeEventArgs e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.ButtonType"/> events
	/// </summary>
	public delegate void ButtonTypeHandler(InSimHandler sender, ButtonType e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.ButtonClick"/> events
	/// </summary>
	public delegate void ButtonClickHandler(InSimHandler sender, ButtonClick e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.ButtonRequest"/> events
	/// </summary>
	public delegate void ButtonsRequestHandler(InSimHandler sender, ButtonsRequest e);
	/// <summary>
	/// DEPRECATED: EventHandler for <see cref="InSimHandler.ButtonRequest"/> events
	/// </summary>
	//[Obsolete("Replaced with ButtonsRequestHandler in 0.18+")]
	public delegate void ButtonRequestHandler(InSimHandler sender, ButtonRequest e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.ButtonsCleared"/> events
	/// </summary>
	public delegate void ButtonsClearedHandler(InSimHandler sender, ButtonsCleared e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.Info"/> events
	/// </summary>
	public delegate void InfoEventHandler(InSimHandler sender, Info e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.AutocrossCleared"/> events
	/// </summary>
	public delegate void AutocrossClearedHandler(InSimHandler sender, AutocrossCleared e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.AutocrossInfo"/> events
	/// </summary>
	public delegate void AutocrossInfoHandler(InSimHandler sender, AutocrossInfo e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.AutocrossObject"/> events
	/// </summary>
	public delegate void AutocrossObjectHandler(InSimHandler sender, AutocrossObject e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.Ping"/> events
	/// </summary>
	public delegate void PingEventHandler(InSimHandler sender, Ping e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.LFSMessage"/> events
	/// </summary>
	public delegate void MessageEventHandler(InSimHandler sender, Message e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.State"/> events
	/// </summary>
	public delegate void StateEventHandler(InSimHandler sender, State e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrack"/> events, i.e. a catch-all for all race tracking
	/// events. Race tracking events can also be subscribed one at a time.
	/// </summary>
	public delegate void RaceTrackEventHandler(InSimHandler sender, RaceTrackEvent e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackRaceStart"/> events
	/// </summary>
	public delegate void RaceTrackRaceStartHandler(InSimHandler sender, RaceTrackRaceStart e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackRaceEnd"/> events
	/// </summary>
	public delegate void RaceTrackRaceEndHandler(InSimHandler sender, RaceTrackRaceEnd e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackConnection"/> events
	/// </summary>
	public delegate void RaceTrackNewConnectionHandler(InSimHandler sender, RaceTrackConnection e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackConnectionLeave"/> events
	/// </summary>
	public delegate void RaceTrackConnectionLeaveHandler(InSimHandler sender, RaceTrackConnectionLeave e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayer"/> events
	/// </summary>
	public delegate void RaceTrackPlayerHandler(InSimHandler sender, RaceTrackPlayer e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayerChange"/> events
	/// </summary>
	public delegate void RaceTrackPlayerChangeHandler(InSimHandler sender, RaceTrackPlayerChange e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayerFlag"/> events
	/// </summary>
	public delegate void RaceTrackPlayerFlagHandler(InSimHandler sender, RaceTrackPlayerFlag e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayerPits"/> events
	/// </summary>
	public delegate void RaceTrackPlayerPitsHandler(InSimHandler sender, RaceTrackPlayerPits e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayerPitlane"/> events
	/// </summary>
	public delegate void RaceTrackPlayerPitlaneHandler(InSimHandler sender, RaceTrackPlayerPitlane e);
	/// <summary>
	/// EventHandler for <see cref="RaceTrackPlayerPitStopBegin"/> events
	/// </summary>
	public delegate void RaceTrackPlayerPitStopBeginHandler(InSimHandler sender, RaceTrackPlayerPitStopBegin e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayerPitStopFinish"/> events
	/// </summary>
	public delegate void RaceTrackPlayerPitStopFinishHandler(InSimHandler sender, RaceTrackPlayerPitStopFinish e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayerCameraChange"/> events
	/// </summary>
	public delegate void RaceTrackPlayerCameraChangeHandler(InSimHandler sender, RaceTrackPlayerCameraChange e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackCarReset"/> events
	/// </summary>
	public delegate void RaceTrackCarResetHandler(InSimHandler sender, RaceTrackCarReset e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackCarTakeover"/> events
	/// </summary>
	public delegate void RaceTrackCarTakeoverHandler(InSimHandler sender, RaceTrackCarTakeover e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayerLeave"/> events
	/// </summary>
	public delegate void RaceTrackPlayerLeaveHandler(InSimHandler sender, RaceTrackPlayerLeave e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayerPenalty"/> events
	/// </summary>
	public delegate void RaceTrackPlayerPenaltyHandler(InSimHandler sender, RaceTrackPlayerPenalty e);
	/// <summary>
	/// EventHandler for <see cref="RaceTrackPlayerRename"/> events
	/// </summary>
	public delegate void RaceTrackPlayerRenameHandler(InSimHandler sender, RaceTrackPlayerRename e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackPlayerFinish"/> events
	/// </summary>
	public delegate void RaceTrackPlayerFinishHandler(InSimHandler sender, RaceTrackPlayerFinish e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackClearRace"/> events
	/// </summary>
	public delegate void RaceTrackClearRaceHandler(InSimHandler sender, RaceTrackClearRace e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackLapTime"/> events
	/// </summary>
	public delegate void RaceTrackPlayerLapHandler(InSimHandler sender, RaceTrackPlayerLap e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackSplitTime"/> events
	/// </summary>
	public delegate void RaceTrackPlayerSplitTimeHandler(InSimHandler sender, RaceTrackPlayerSplitTime e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackResult"/> events
	/// </summary>
	public delegate void RaceTrackPlayerResultHandler(InSimHandler sender, RaceTrackPlayerResult e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackReorder"/> events
	/// </summary>
	public delegate void RaceTrackReorderHandler(InSimHandler sender, RaceTrackReorder e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackNodeLap"/> events
	/// </summary>
	public delegate void RaceTrackNodeLapPacketHandler(InSimHandler sender, RaceTrackNodeLap e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.RaceTrackMultiCarInfo"/> events
	/// </summary>
	public delegate void RaceTrackMultiCarInfoHandler(InSimHandler sender, RaceTrackMultiCarInfo e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.LFSMultiplayer"/> events
	/// </summary>
	public delegate void MultiplayerEventHandler(InSimHandler sender, Multiplayer e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.LFSCameraPosition"/> events
	/// </summary>
	public delegate void CameraPositionEventHandler(InSimHandler sender, CameraPosition e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.LFSRaceTime"/> events
	/// </summary>
	public delegate void RaceTimeEventHandler(InSimHandler sender, RaceTime e);
	/// <summary>
	/// EventHandler for <see cref="InSimHandler.Vote"/> events, i.e. see <see cref="VoteAction"/>
	/// or <see cref="VoteCancel"/> for possible event args
	/// </summary>
	public delegate void VoteEventHandler(InSimHandler sender, VoteEventArgs e);


	/// <summary>
	/// EventHandler for <see cref="IInSimRelayHandler.RelayError"/> events
	/// </summary>
	public delegate void RelayErrorEventHandler(IInSimRelayHandler sender, RelayError e);

	/// <summary>
	/// EventHandler for <see cref="IInSimRelayHandler.RelayHostlist"/> events
	/// </summary>
	public delegate void RelayHostlistEventHandler(IInSimRelayHandler sender, RelayHostlist e);

	/// <summary>
	/// EventHandler for <see cref="IInSimRelayHandler.RelayHostConnected"/> events
	/// </summary>
	public delegate void RelayHostConnectedEventHandler(IInSimRelayHandler sender, LFSVersion e);
}

namespace FullMotion.LiveForSpeed.InSim.Exceptions
{
	/// <summary>
	/// Abstract base for all InSimHandler related Exceptions
	/// </summary>
	public abstract class InSimHandlerException : Exception
	{
		internal InSimHandlerException()
			: base() {
		}

		internal InSimHandlerException(Exception innerException)
			: base("InsimHandler experienced an innner failure", innerException) {
		}

		/// <summary>
		/// The connection to the LFS host has not been initialized
		/// </summary>
		public class NotConnected : InSimHandlerException
		{
			internal NotConnected(Exception innerException)
				: base(innerException) {
			}
			internal NotConnected()
				: base() {
			}
		}

		/// <summary>
		/// The LFS specified could not be connected to
		/// </summary>
		public class ConnectionFailed : InSimHandlerException
		{
			internal ConnectionFailed(Exception innerException)
				: base(innerException) {
			}
			internal ConnectionFailed()
				: base() {
			}
		}

		/// <summary>
		/// The LFS instance we connected to did not return a Version in a timely manner
		/// </summary>
		public class NoVersion : ConnectionFailed
		{
		}

		/// <summary>
		/// The LFS instance we connected to did not return a State in a timely manner
		/// </summary>
		public class NoInitialState : ConnectionFailed
		{
		}

		/// <summary>
		/// The connection to the LFS host has already been initialized
		/// </summary>
		public class AlreadyInitialized : InSimHandlerException
		{
		}
	}
}

