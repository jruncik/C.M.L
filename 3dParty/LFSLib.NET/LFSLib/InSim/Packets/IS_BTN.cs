/* ------------------------------------------------------------------------- *
 * Copyright (C) 2007 Arne Claassen
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
using FullMotion.LiveForSpeed.Util;

namespace FullMotion.LiveForSpeed.InSim.Packets
{
  // SHIFT+I clears all host buttons if any - or sends a BFN_REQUEST to host instances
  // SHIFT+B is the same but for local buttons and local instances

  // To send a button to LFS, send this variable sized packet

  /// <summary>
  /// BuTtoN - button header - followed by 0 to 240 characters
  /// </summary>
  struct IS_BTN : IClientLfsInSimPacket
  {
    public byte Size;		// 12 + TEXT_SIZE (a multiple of 4)
    public Enums.ISP Type;		// ISP_BTN
    public byte ReqI;		// non-zero (returned in IS_BTC and IS_BTT packets)
    public byte UCID;		// connection to display the button (0 = local / 255 = all)

    public byte ClickID;	// button ID (0 to 239)
    public Flags.INST Inst;		// some extra flags - see below
    public Flags.ISB BStyle;		// button style flags - see below
    public byte TypeIn;		// max chars to type in - see below

    public byte L;			// left   : 0 - 200
    public byte T;			// top    : 0 - 200
    public byte W;			// width  : 0 - 200
    public byte H;			// height : 0 - 200

    public byte[] Text; //[TEXT_SIZE]; // 0 to 240 characters of text

    public IS_BTN(byte buttonId)
    {
      Size = 12;
      Type = Enums.ISP.BTN;
      ReqI = 1;
      UCID = 0;
      ClickID = buttonId;
      Inst = 0;
      BStyle = 0;
      TypeIn = 0;
      L = 0;
      T = 0;
      W = 0;
      H = 0;
      Text = new byte[0];
    }

    public IS_BTN(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      UCID = bytes[position++];
      ClickID = bytes[position++];
      Inst = (Flags.INST)bytes[position++];
      BStyle = (Flags.ISB)bytes[position++];
      TypeIn = bytes[position++];
      L = bytes[position++];
      T = bytes[position++];
      W = bytes[position++];
      H = bytes[position++];
      int textSize = Size - 12;
      Text = new byte[textSize];
      Array.Copy(bytes, position, Text, 0, textSize);
    }

    public void SetText(string text)
    {
      byte length = (byte)Math.Min(240, Math.Ceiling(text.Length + 1 / 4d) * 4);
      Text = new byte[length];
      Text = CharHelper.GetBytes(text, length, true);
      Size = (byte)(12 + length);
    }

    public void SetText(string text, string caption)
    {
      byte pad = (byte)((caption == string.Empty) ? 2 : 3);
      byte length = (byte)Math.Min(240, Math.Ceiling(text.Length + caption.Length + pad / 4d) * 4);
      Text = new byte[length];
      byte[] captionBytes = CharHelper.GetBytes(caption, caption.Length + 1, true);
      Array.Copy(captionBytes, 0, Text, 1, captionBytes.Length);

      int textLength = length - captionBytes.Length - 1;
      byte[] textBytes = CharHelper.GetBytes(text, textLength, true);
      Array.Copy(textBytes, 0, Text, captionBytes.Length + 1, textLength);

      Size = (byte)(12 + length);
    }

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion

    #region IClientLfsInSimPacket Members

    public byte RequestId
    {
      get { return ReqI; }
    }

    public byte[] GetBytes()
    {
      byte[] bytes = new byte[Size];

      int position = 0;

      bytes[position++] = Size;
      bytes[position++] = (byte)Type;
      bytes[position++] = ReqI;
      bytes[position++] = UCID;

      bytes[position++] = ClickID;
      bytes[position++] = (byte)Inst;
      bytes[position++] = (byte)BStyle;
      bytes[position++] = TypeIn;

      bytes[position++] = L;
      bytes[position++] = T;
      bytes[position++] = W;
      bytes[position++] = H;

      Array.Copy(this.Text, 0, bytes, position, this.Text.Length);

      return bytes;
    }

    #endregion

  }

  // ClickID byte : this value is returned in IS_BTC and IS_BTT packets.

  // Host buttons and local buttons are stored separately, so there is no chance of a conflict between
  // a host control system and a local system (although the buttons could overlap on screen).

  // Programmers of local InSim programs may wish to consider using a configurable button range and
  // possibly screen position, in case their users will use more than one local InSim program at once.

  // TypeIn byte : if set, the user can click this button to type in text.

  // Lowest 7 bits are the maximum number of characters to type in (0 to 95)
  // Highest bit (128) can be set to initialise dialog with the button's text

  // On clicking the button, a text entry dialog will be opened, allowing the specified number of
  // characters to be typed in.  The caption on the text entry dialog is optionally customisable using
  // Text in the IS_BTN packet.  If the first character of IS_BTN's Text field is zero, LFS will read
  // the caption up to the second zero.  The visible button text then follows that second zero.

  // Text : 0-65-66-0 would display button text "AB" and no caption

  // Text : 0-65-66-67-0-68-69-70-71-0-0-0 would display button text "DEFG" and caption "ABC"

  // Inst byte : mainly used internally by InSim but also provides some extra user flags


  // NOTE : You should not use INST_ALWAYS_ON for most buttons.  This is a special flag for buttons
  // that really must be on in all screens (including the garage and options screens).  You will
  // probably need to confine these buttons to the top or bottom edge of the screen, to avoid
  // overwriting LFS buttons.  Most buttons should be defined without this flag, and positioned
  // in the recommended area so LFS can keep a space clear in the main screens.


  // NOTE : If width or height are zero, this would normally be an invalid button.  But in that case if
  // there is an existing button with the same ClickID, all the packet contents are ignored except the
  // Text field.  This can be useful for updating the text in a button without knowing its position.
  // For example, you might reply to an IS_BTT using an IS_BTN with zero W and H to update the text.

}
