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
  /// A text message sent from LFS
  /// </summary>
  public class Message : EventArgs
  {
    #region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

    #region Member Variables ######################################################################
    private Packets.IS_MSO packet;
    private bool isSplitMessage = false;
    private string message;
    private string player;
    #endregion

    #region Constructors ##########################################################################
    internal Message(Packets.IS_MSO packet, bool isSplitMessage)
    {
      this.packet = packet;
      this.isSplitMessage = isSplitMessage;
    }
    #endregion

    #region Properties ############################################################################
    /// <summary>
    /// Is the Message a Split Message, i.e. the User Name is accessible via <see cref="Playername"/>
    /// and not part of <see cref="MessageText"/>
    /// </summary>
    public bool IsSplitMessage
    {
      get { return isSplitMessage; }
    }
    /// <summary>
    /// DEPRECATED: The name of the player sending the message. Empty string if this is not a Split Message
    /// </summary>
    [Obsolete("Replaced with PlayerName in 0.18+")]
    public string Playername
    {
      get { return PlayerName; }
    }
    /// <summary>
    /// The name of the player sending the message. Empty string if this is not a Split Message
    /// </summary>
    public string PlayerName
    {
      get
      {
        if (string.IsNullOrEmpty(player))
        {
          if (isSplitMessage)
          {
            byte[] usernameBytes = new byte[packet.TextStart];
            Array.Copy(packet.Msg, 0, usernameBytes, 0, packet.TextStart);
            player = CharHelper.GetString(usernameBytes);
          }
          else
          {
            player = string.Empty;
          }
        }
        return player;
      }
    }
    /// <summary>
    /// DEPRECATED: The name of the player sending the message as received in bytes. Empty array if this is not a Split Message
    /// </summary>
    [Obsolete("Replaced with RawPlayerName in 0.18+")]
    public byte[] RawPlayername
    {
      get { return RawPlayerName; }
    }
    /// <summary>
    /// The name of the player sending the message as received in bytes. Empty array if this is not a Split Message
    /// </summary>
    public byte[] RawPlayerName
    {
      get
      {
        if (isSplitMessage)
        {
          byte[] playerBytes = new byte[packet.TextStart];
          Array.Copy(packet.Msg, 0, playerBytes, 0, packet.TextStart);
          return playerBytes;
        }
        else
        {
          return new byte[0];
        }
      }
    }
    /// <summary>
    /// The text of the message
    /// </summary>
    public string MessageText
    {
      get
      {
        if (string.IsNullOrEmpty(message))
        {
          if (isSplitMessage)
          {
            int msgLength = packet.Msg.Length - packet.TextStart;
            byte[] msgBytes = new byte[msgLength];
            Array.Copy(packet.Msg, packet.TextStart, msgBytes, 0, msgLength);
            message = CharHelper.GetString(msgBytes);
          }
          else
          {
            message = CharHelper.GetString(packet.Msg);
          }
        }
        return message;
      }
    }
    /// <summary>
    /// The text of the message in bytes
    /// </summary>
    public byte[] RawMessageText
    {
      get
      {
        if (isSplitMessage)
        {
          int msgLength = packet.Msg.Length - packet.TextStart;
          byte[] msgBytes = new byte[msgLength];
          Array.Copy(packet.Msg, packet.TextStart, msgBytes, 0, msgLength);
          return msgBytes;
        }
        else
        {
          return packet.Msg;
        }
      }
    }
    /// <summary>
    /// The sending player's Id. If 0 check <see cref="ConnectionId"/> instead
    /// </summary>
    public byte PlayerId
    {
      get { return packet.PLID; }
    }
    /// <summary>
    /// The sending connection's Id. If 0 then the message came from the host
    /// </summary>
    public byte ConnectionId
    {
      get { return packet.UCID; }
    }
    /// <summary>
    /// The user type of the sender 
    /// </summary>
    public Enums.UserType UserType
    {
      get { return packet.UserType; }
    }
    #endregion

    #region Methods ###############################################################################
    #endregion

    #region Private Methods #######################################################################
    #endregion
  }
}
