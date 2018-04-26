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
using FullMotion.LiveForSpeed.InSim.EventHandlers;
using FullMotion.LiveForSpeed.InSim.Events;
using FullMotion.LiveForSpeed.InSim.Exceptions;
namespace FullMotion.LiveForSpeed.InSim
{
  /// <summary>
  /// An InSim Handler specifically designed for talking to the Live For Speed InSim relay.
  /// </summary>
  public interface IInSimRelayHandler
  {
    /// <summary>
    /// The last state that was received. Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    State CurrentLFSState { get; }

    /// <summary>
    /// The state invalidated by the most recent State event. Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    State LastLFSState { get; }

    /// <summary>
    /// The State of the object
    /// </summary>
    InSimHandler.HandlerState State { get; }

    /// <summary>
    /// The version of the currently connected to LFS instance. Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    LFSVersion Version { get; }


    /// <summary>
    /// Cancel a currently vote in progress. Should see a <see cref="VoteCancel"/> event come into
    /// the <see cref="Vote"/> event handler.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    void CancelVote();

    /// <summary>
    /// Request that all buttons created by this handler be cleared either locally or on all clients.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <param name="local"></param>
    void ClearButtons(bool local);

    /// <summary>
    /// Request that all buttons created by this handler be cleared on a specific connection.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <param name="connectionId"></param>
    void ClearButtons(byte connectionId);

    /// <summary>
    /// Disconnect from LFS Relay
    /// </summary>
    void Close();

    /// <summary>
    /// Connect to a specific host from the relay host list
    /// </summary>
    /// <param name="hostname">hostname to connect to</param>
    /// <returns></returns>
    byte ConnectToHost(string hostname);

    /// <summary>
    /// Connect to a specific host from the relay host list
    /// </summary>
    /// <param name="hostname">hostname to connect to</param>
    /// <param name="adminPass">admin password for the host</param>
    /// <returns></returns>
    byte ConnectToHost(string hostname, string adminPass);

    /// <summary>
    /// Connect to a specific host from the relay host list
    /// </summary>
    /// <param name="hostname">hostname to connect to</param>
    /// <param name="adminPass">admin password for the host (can be null)</param>
    /// <param name="spectatorPass">spectator password for the host, should it require one</param>
    /// <returns></returns>
    byte ConnectToHost(string hostname, string adminPass, string spectatorPass);

    /// <summary>
    /// Delete a specific button locally or globally
    /// </summary>
    /// <param name="button"></param>
    /// <param name="local"></param>
    void DeleteButton(LfsButton button, bool local);

    /// <summary>
    /// Delete a specific button on a specific connection
    /// </summary>
    /// <param name="button"></param>
    /// <param name="connectionId"></param>
    void DeleteButton(LfsButton button, byte connectionId);

    /// <summary>
    /// Delete a specific button by Id locally or globally
    /// </summary>
    /// <param name="buttonId"></param>
    /// <param name="local"></param>
    void DeleteButton(byte buttonId, bool local);

    /// <summary>
    /// Delete a specific button by Id on a specific connection
    /// </summary>
    /// <param name="buttonId"></param>
    /// <param name="connectionId"></param>
    void DeleteButton(byte buttonId, byte connectionId);

    /// <summary>
    /// Initialize the connection to the LFS Relay. May throw one of the following exceptions: <see cref="InSimHandlerException.AlreadyInitialized"/>, <see cref="InSimHandlerException.NoVersion"/>, <see cref="InSimHandlerException.NoInitialState"/> or <see cref="InSimHandlerException.ConnectionFailed"/>
    /// </summary>
    void Initialize();

    /// <summary>
    /// Request information about the current Autocross setup. Need to be listening to <see cref="AutocrossInfo"/>.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns></returns>
    byte RequestAutocrossInfo();

    /// <summary>
    /// Request all <see cref="RaceTrackConnection"/> events. Need to be listening to <see cref="RaceTrackConnection"/>.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns>requestId</returns>
    byte RequestConnectionInfo();

    /// <summary>
    /// Requests a <see cref="FullMotion.LiveForSpeed.InSim.Events.RaceTrackReorder"/> event. Need to be listening to
    /// <see cref="RaceTrackReorder"/>. Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns>requestId</returns>
    byte RequestGridOrder();

    /// <summary>
    /// Request a ping response from LFS. Need to be listening to <see cref="Ping"/>.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns>requestId</returns>
    byte RequestPing();

    /// <summary>
    /// Request all <see cref="RaceTrackPlayer"/> events. Need to be listening to <see cref="RaceTrackPlayer"/>.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns>requestId</returns>
    byte RequestPlayerInfo();

    /// <summary>
    /// Request a <see cref="RaceTrackMultiCarInfo"/> event. Need to be listening to <see cref="RaceTrackMultiCarInfo"/>.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns>requestId</returns>
    byte RequestRaceTrackingMultiCarInfo();

    /// <summary>
    /// Request a <see cref="RaceTrackNodeLap"/> event. Need to be listening to <see cref="RaceTrackNodeLap"/>.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns>requestId</returns>
    byte RequestRaceTrackingNodeLap();

    /// <summary>
    /// Request all <see cref="RaceTrackResult"/> events. Need to be listening to <see cref="RaceTrackResult"/>.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns>requestId</returns>
    byte RequestRaceTrackingResult();

    /// <summary>
    /// Request the <see cref="RaceTrackRaceStart"/> events. Need to be listening to <see cref="RaceTrackRaceStart"/>.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns></returns>
    byte RequestRaceTrackingStart();

    /// <summary>
    /// Request <see cref="RelayHostlist"/> events to get a complete list of relay accessible hosts
    /// </summary>
    /// <returns></returns>
    byte RequestRelayHostlist();
    
    /// <summary>
    /// Requests current State from LFS. Need to be listening to <see cref="LFSState"/> event for response.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <returns>requestId</returns>
    byte RequestState();

    /// <summary>
    /// Send a text message to the Host. Throws <see cref="InSimHandlerException.NotConnected"/>.
    /// </summary>
    /// <param name="bytes"></param>
    void SendMessage(byte[] bytes);

    /// <summary>
    /// Send a text message to the Host. Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <param name="message"></param>
    void SendMessage(string message);

    /// <summary>
    /// Send a text message to a specific client connection. (Only works when connected to the Host)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="connectionId"></param>
    void SendMessageToConnection(string message, byte connectionId);

    /// <summary>
    /// Send a text message only to the local client. Can be used in conjuntion with a sound effect
    /// </summary>
    /// <param name="message"></param>
    /// <param name="soundEffect"></param>
    void SendMessageToLocalClient(string message, Enums.Sound soundEffect);

    /// <summary>
    /// Send a text message to a specific client. (Only works when connected to the Host)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="playerId"></param>
    void SendMessageToPlayer(string message, byte playerId);

    /// <summary>
    /// You can send the grid order LFS before a race start, to specify the starting order.
    /// It may be a good idea to avoid conflict by using /start=fixed (LFS setting).
    /// Alternatively, you can leave the LFS setting, but make sure you send your order
    /// AFTER you receive the VoteAction.  LFS does its default grid reordering at the same time
    /// as it sends the VoteAction and you can override this by sending your grid order.
    /// </summary>
    /// <param name="orderedPlayerIds"></param>
    void SetGridOrder(byte[] orderedPlayerIds);

    /// <summary>
    /// Send a button to be created locally or globally
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <param name="button"></param>
    /// <param name="local"></param>
    /// <returns>requestId</returns>
    byte ShowButton(LfsButton button, bool local);

    /// <summary>
    /// Send a button to be created on a specific connection.
    /// Throws <see cref="InSimHandlerException.NotConnected"/>
    /// </summary>
    /// <param name="button"></param>
    /// <param name="connectionId"></param>
    /// <returns>requestId</returns>
    byte ShowButton(LfsButton button, byte connectionId);

    /// <summary>
    /// Notification that all objects have been cleared from the current layout
    /// </summary>
    event AutocrossClearedHandler AutocrossCleared;
    
    /// <summary>
    /// Response to a Autocross information request
    /// </summary>
    event AutocrossInfoHandler AutocrossInfo;

    /// <summary>
    /// Notification that a player has hit an autocross object
    /// </summary>
    event AutocrossObjectHandler AutocrossObject;

    /// <summary>
    /// A button click notification for a custom LFS button
    /// </summary>
    event ButtonClickHandler ButtonClick;

    /// <summary>
    /// Notification for a client requesting custom buttons
    /// </summary>
    event ButtonRequestHandler ButtonRequest;

    /// <summary>
    /// Notification that a client has cleared its buttons
    /// </summary>
    event ButtonsClearedHandler ButtonsCleared;

    /// <summary>
    /// Notification of a custom Dialog Button having text typed in
    /// </summary>
    event ButtonTypeHandler ButtonType;

    /// <summary>
    /// Notification that a connection has requested InSim information
    /// </summary>
    event InfoEventHandler Info;

    /// <summary>
    /// A message received from LFS
    /// </summary>
    event MessageEventHandler LFSMessage;

    /// <summary>
    /// A Race Time event received from LFS
    /// </summary>
    event RaceTimeEventHandler LFSRaceTime;

    /// <summary>
    /// A state event received from LFS
    /// </summary>
    event StateEventHandler LFSState;

    /// <summary>
    /// Response to a <see cref="RequestPing"/>
    /// </summary>
    event PingEventHandler Ping;

    /// <summary>
    /// A race tracking event received from LFS
    /// </summary>
    event RaceTrackEventHandler RaceTrack;

    /// <summary>
    /// A race tracking event received from LFS regarding a car resetting on track
    /// </summary>
    event RaceTrackCarResetHandler RaceTrackCarReset;

    /// <summary>
    /// A race tracking event received from LFS regarding a driver change
    /// </summary>
    event RaceTrackCarTakeoverHandler RaceTrackCarTakeover;

    /// <summary>
    /// A race tracking event received from LFS regarding the clearing of the current grid
    /// </summary>
    event RaceTrackClearRaceHandler RaceTrackClearRace;

    /// <summary>
    /// A race tracking event received from LFS regarding a (possibly new) connection
    /// </summary>
    event RaceTrackNewConnectionHandler RaceTrackConnection;

    /// <summary>
    /// A race tracking event received from LFS regarding a connection leaving the server
    /// </summary>
    event RaceTrackConnectionLeaveHandler RaceTrackConnectionLeave;

    /// <summary>
    /// A race tracking event received from LFS regarding multi car information
    /// </summary>
    event RaceTrackMultiCarInfoHandler RaceTrackMultiCarInfo;

    /// <summary>
    /// A race tracking event received from LFS regarding node lap information
    /// </summary>
    event RaceTrackNodeLapPacketHandler RaceTrackNodeLap;

    /// <summary>
    /// A race tracking event received from LFS regarding a (possibly new) player
    /// </summary>
    event RaceTrackPlayerHandler RaceTrackPlayer;

    /// <summary>
    /// A race tracking event received from LFS regarding a player pitting
    /// </summary>
    event RaceTrackPlayerCameraChangeHandler RaceTrackPlayerCameraChange;

    /// <summary>
    /// A race tracking event received from LFS regarding a change in a player's options
    /// </summary>
    event RaceTrackPlayerChangeHandler RaceTrackPlayerChange;

    /// <summary>
    /// A race tracking event received from LFS regarding a player finishing the race (not final race result)
    /// </summary>
    event RaceTrackPlayerFinishHandler RaceTrackPlayerFinish;

    /// <summary>
    /// A race tracking event received from LFS regarding a player flag event
    /// </summary>
    event RaceTrackPlayerFlagHandler RaceTrackPlayerFlag;

    /// <summary>
    /// A race tracking event received from LFS regarding the lap time for a player
    /// </summary>
    event RaceTrackPlayerLapHandler RaceTrackPlayerLap;

    /// <summary>
    /// A race tracking event received from LFS regarding a player leaving the game
    /// </summary>
    event RaceTrackPlayerLeaveHandler RaceTrackPlayerLeave;

    /// <summary>
    /// A race tracking event received from LFS regarding a player penalty event (given or cleared
    /// </summary>
    event RaceTrackPlayerPenaltyHandler RaceTrackPlayerPenalty;

    /// <summary>
    /// A race tracking event received from LFS regarding a player pitting
    /// </summary>
    event RaceTrackPlayerPitlaneHandler RaceTrackPlayerPitlane;

    /// <summary>
    /// A race tracking event received from LFS regarding a player pitting
    /// </summary>
    event RaceTrackPlayerPitsHandler RaceTrackPlayerPits;

    /// <summary>
    /// A race tracking event received from LFS regarding a player beginning a pit stop
    /// </summary>
    event RaceTrackPlayerPitStopBeginHandler RaceTrackPlayerPitStopBegin;

    /// <summary>
    /// A race tracking event received from LFS regarding a player completing a pit stop
    /// </summary>
    event RaceTrackPlayerPitStopFinishHandler RaceTrackPlayerPitStopFinish;

    /// <summary>
    /// A race tracking event received from LFS regarding a player changing their name
    /// </summary>
    event RaceTrackPlayerRenameHandler RaceTrackPlayerRename;

    /// <summary>
    /// A race tracking event received from LFS regarding a player's race result
    /// </summary>
    event RaceTrackPlayerResultHandler RaceTrackPlayerResult;

    /// <summary>
    /// A race tracking event received from LFS regarding a split time for a player
    /// </summary>
    event RaceTrackPlayerSplitTimeHandler RaceTrackPlayerSplitTime;

    /// <summary>
    /// A race tracking event received from LFS regarding the end of the race
    /// </summary>
    event RaceTrackRaceEndHandler RaceTrackRaceEnd;

    /// <summary>
    /// A race tracking event received from LFS regarding the start of the race
    /// </summary>
    event RaceTrackRaceStartHandler RaceTrackRaceStart;

    /// <summary>
    /// A race tracking event received from LFS regarding a grid re-ordering
    /// </summary>
    event RaceTrackReorderHandler RaceTrackReorder;
    
    /// <summary>
    /// Notification of a relay error
    /// </summary>
    event RelayErrorEventHandler RelayError;
    
    /// <summary>
    /// Notification that the requested connection to a relayed host succeeded
    /// </summary>
    event RelayHostConnectedEventHandler RelayHostConnected;
    
    /// <summary>
    /// Set of relay hosts
    /// </summary>
    event RelayHostlistEventHandler RelayHostlist;

    /// <summary>
    /// Notification that the state of the handler has changed
    /// </summary>
    event StateChangeHandler StateChanged;

    /// <summary>
    /// A multiplayer event received from LFS
    /// </summary>
    event VoteEventHandler Vote;
  }
}
