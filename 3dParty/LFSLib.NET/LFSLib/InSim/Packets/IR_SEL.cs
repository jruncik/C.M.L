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

namespace FullMotion.LiveForSpeed.InSim.Packets
{
  struct IR_SEL : IClientLfsInSimPacket
  {
    public byte Size;		// 68
    public Enums.ISP Type;		// IRP_SEL
    public byte ReqI;		// If non-zero Relay will send an IS_VER packet
    public byte Zero;		// 0

    public byte[] HName; //[32];	// Hostname to receive data from
    public byte[] Admin; //[16];	// Admin password (to gain admin access to host)
    public byte[] Spec; //[16];	// Spectator password (if host requires it)

    public IR_SEL(string hostname, string adminPass, string specPass)
    {
      Size = 68;
      Type = Enums.ISP.IRP_SEL;
      ReqI = PacketFactory.NextRequestId;
      Zero = 0;
      HName = new byte[32];
      byte[] hostBytes = Encoding.ASCII.GetBytes(hostname);
      Array.Copy(hostBytes, HName, Math.Min(32,hostBytes.Length));
      Admin = new byte[16];
      byte[] adminBytes = Encoding.ASCII.GetBytes((adminPass == null) ? string.Empty : adminPass);
      Array.Copy(adminBytes, Admin, Math.Min(16, adminBytes.Length));
      Spec = new byte[16];
      byte[] specBytes = Encoding.ASCII.GetBytes((specPass == null) ? string.Empty : specPass);
      Array.Copy(specBytes, Spec, Math.Min(16, specBytes.Length));
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
      bytes[position++] = Zero;

      Array.Copy(this.HName, 0, bytes, position, this.HName.Length);
      position += this.HName.Length;
      Array.Copy(this.Admin, 0, bytes, position, this.Admin.Length);
      position += this.Admin.Length;
      Array.Copy(this.Spec, 0, bytes, position, this.Spec.Length);
      position += this.Spec.Length;

      return bytes;
    }

    #endregion
  }
}
