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

namespace FullMotion.LiveForSpeed.InSim
{
  /// <summary>
  /// A button that can be sent to LFS for display in game.
  /// You can make up to 240 buttons appear on the host or guests (buttonId = 0 to 239).
  /// You should set <see cref="Configuration.Local"/> if your program is not a host control
  /// system, to make sure your buttons do not conflict with any buttons sent by the host.
  /// </summary>
  public class LfsButton
  {
    #region Static Members ########################################################################
    private const byte LOW = 127;
    private const byte HIGH = 128;
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

    #region Member Variables ######################################################################
    private Packets.IS_BTN packet;
    private string text = string.Empty;
    private string caption;
    #endregion

    #region Constructors ##########################################################################
    /// <summary>
    /// Create a new button. Caller is responsible for choosing a non-conflicting buttonId
    /// between 0 and 239
    /// </summary>
    /// <param name="buttonId"></param>
    public LfsButton(byte buttonId)
    {
      packet = new Packets.IS_BTN(buttonId);
    }
    #endregion

    #region Properties ############################################################################
    /// <summary>
    /// The button's id
    /// </summary>
    public byte ButtonId
    {
      get { return packet.ClickID; }
    }

    /// <summary>
    /// The type of Button
    /// </summary>
    public Enums.ButtonType ButtonType
    {
      get
      {
        if (!Clickable)
        {
          return Enums.ButtonType.Label;
        }
        else if (packet.TypeIn == 0)
        {
          return Enums.ButtonType.Button;
        }
        else
        {
          return Enums.ButtonType.Dialog;
        }
      }
    }


    /// <summary>
    /// Number of Characters allowed in the Dialog caused by this button. Setting DialogTextLimit
    /// or <see cref="InitializeDialogWithButtonText"/> converts the a simple button to a Dialog.
    /// </summary>
    public byte DialogTextLimit
    {
      get { return (byte)(packet.TypeIn & LOW); }
      set
      {
        if (value > 95)
        {
          throw new ArgumentException("Cannot exceed 95");
        }
        packet.TypeIn = (byte)((packet.TypeIn & HIGH) | (value & LOW));
      }
    }

    /// <summary>
    /// Initialize the dialog text with the <see cref="Text"/> of the button (not the <see cref="DialogCaption"/>).
    /// Setting <see cref="DialogTextLimit"/> or this converts the a simple button to a Dialog.
    /// </summary>
    public bool InitializeDialogWithButtonText
    {
      get { return (HIGH == (HIGH & packet.TypeIn)); }
      set { packet.TypeIn = (value) ? (byte)(packet.TypeIn | HIGH) : (byte)(packet.TypeIn & ~HIGH); }
    }

    /// <summary>
    /// Caption for the dialog displayed for a Button of type Dialog
    /// </summary>
    public string DialogCaption
    {
      get { return caption; }
      set
      {
        if (caption != null && AlwaysVisible)
        {
          throw new ArgumentException("Cannot created a dialog button if AlwaysVisible is set");
        }
        int length = (value == null) ? 0 : value.Length;
        length += (text == null) ? 0 : (text.Length + 1);
        if (length > 238)
        {
          throw new ArgumentException("Button & Caption Text together cannot exceed 238 characters");
        }
        caption = value;
      }
    }

    /// <summary>
    /// Display Text of the Button
    /// </summary>
    public string Text
    {
      get { return text; }
      set
      {
        int length = (value == null) ? 0 : value.Length;
        length += (caption == null) ? 0 : (caption.Length + 1);
        if (length > 238)
        {
          throw new ArgumentException("Button & Caption Text together cannot exceed 238 characters");
        }
        text = value;
      }
    }

    /// <summary>
    /// Position of the Button from left edge, specified as a value from 0 to 200. (200 = 100%)
    /// </summary>
    public byte Left
    {
      get { return packet.L; }
      set
      {
        if (value > 200)
        {
          throw new ArgumentException("Cannot exceed 200");
        }
        packet.L = value;
      }
    }

    /// <summary>
    /// Position of the Button from top edge, specified as a value from 0 to 200. (200 = 100%)
    /// </summary>
    public byte Top
    {
      get { return packet.T; }
      set
      {
        if (value > 200)
        {
          throw new ArgumentException("Cannot exceed 200");
        }
        packet.T = value;
      }
    }

    /// <summary>
    /// Width of the Button relative to screen size, specified as a value from 0 to 200. (200 = 100%)
    /// </summary>
    public byte Width
    {
      get { return packet.W; }
      set
      {
        if (value > 200)
        {
          throw new ArgumentException("Cannot exceed 200");
        }
        packet.W = value;
      }
    }

    /// <summary>
    /// Height of the Button relative to screen size, specified as a value from 0 to 200. (200 = 100%)
    /// </summary>
    public byte Height
    {
      get { return packet.H; }
      set
      {
        if (value > 200)
        {
          throw new ArgumentException("Cannot exceed 200");
        }
        packet.H = value;
      }
    }

    /// <summary>
    /// Should this button always be visible. Invalid to set for Dialogs
    /// </summary>
    public bool AlwaysVisible
    {
      get { return (Flags.INST.ALWAYS_ON == (Flags.INST.ALWAYS_ON & packet.Inst)); }
      set
      {
        if (ButtonType == Enums.ButtonType.Dialog)
        {
          throw new ArgumentException("Cannot set a Dialog Button to always visible");
        }
        packet.Inst = (value) ? packet.Inst | Flags.INST.ALWAYS_ON : packet.Inst & ~Flags.INST.ALWAYS_ON;
      }
    }

    /// <summary>
    /// If true, the button can be interacted with, otherwise it is simply a text Label
    /// </summary>
    public bool Clickable
    {
      get { return (Flags.ISB.CLICK == (Flags.ISB.CLICK & packet.BStyle)); }
      set { packet.BStyle = (value) ? packet.BStyle | Flags.ISB.CLICK : packet.BStyle & ~Flags.ISB.CLICK; }
    }

    /// <summary>
    /// Button Background Color
    /// </summary>
    public Enums.ButtonColor Color
    {
      get
      {
        if (Flags.ISB.DARK == (Flags.ISB.DARK & packet.BStyle))
        {
          return Enums.ButtonColor.Dark;
        }
        else if (Flags.ISB.LIGHT == (Flags.ISB.LIGHT & packet.BStyle))
        {
          return Enums.ButtonColor.Light;
        }
        else
        {
          return Enums.ButtonColor.Transparent;
        }
      }
      set
      {
        switch (value)
        {
          case Enums.ButtonColor.Transparent:
            packet.BStyle = packet.BStyle & ~Flags.ISB.DARK & ~Flags.ISB.LIGHT;
            break;
          case Enums.ButtonColor.Dark:
            packet.BStyle = packet.BStyle | Flags.ISB.DARK;
            packet.BStyle = packet.BStyle & ~Flags.ISB.LIGHT;
            break;
          case Enums.ButtonColor.Light:
            packet.BStyle = packet.BStyle | Flags.ISB.LIGHT;
            packet.BStyle = packet.BStyle & ~Flags.ISB.DARK;
            break;
        }
      }
    }

    /// <summary>
    /// Button text alignment
    /// </summary>
    public Enums.ButtonTextAlignment TextAlignment
    {
      get
      {
        if (Flags.ISB.RIGHT == (Flags.ISB.RIGHT & packet.BStyle))
        {
          return Enums.ButtonTextAlignment.Right;
        }
        else if (Flags.ISB.LEFT == (Flags.ISB.LEFT & packet.BStyle))
        {
          return Enums.ButtonTextAlignment.Left;
        }
        else
        {
          return Enums.ButtonTextAlignment.Center;
        }
      }
      set
      {
        switch (value)
        {
          case Enums.ButtonTextAlignment.Center:
            packet.BStyle = packet.BStyle & ~Flags.ISB.RIGHT & ~Flags.ISB.LEFT;
            break;
          case Enums.ButtonTextAlignment.Right:
            packet.BStyle = packet.BStyle | Flags.ISB.RIGHT;
            packet.BStyle = packet.BStyle & ~Flags.ISB.LEFT;
            break;
          case Enums.ButtonTextAlignment.Left:
            packet.BStyle = packet.BStyle | Flags.ISB.LEFT;
            packet.BStyle = packet.BStyle & ~Flags.ISB.RIGHT;
            break;
        }
      }
    }

    /// <summary>
    /// Button text color (based on LFS color preferences
    /// </summary>
    public Enums.ButtonTextColor TextColor
    {
      get { return (Enums.ButtonTextColor)(packet.BStyle & Flags.ISB.COLOR_OPP); }
      set { packet.BStyle = packet.BStyle & ~Flags.ISB.COLOR_OPP | (Flags.ISB)value; }
    }
    #endregion

    #region Methods ###############################################################################
    internal Packets.IClientLfsInSimPacket GetPacket(byte connectionId)
    {
      packet.UCID = connectionId;
      packet.ReqI = Packets.PacketFactory.NextRequestId;
      if (string.IsNullOrEmpty(caption))
      {
        packet.SetText(text);
      }
      else
      {
        packet.SetText(text, caption);
      }
      return packet;
    }
    #endregion

    #region Private Methods #######################################################################
    #endregion
  }
}
