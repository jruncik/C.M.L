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
  /// <summary>
  /// Multi Car Info - if more than 8 in race then more than one of these is sent
  /// </summary>
  struct IS_MCI : ILfsInSimPacket
  {
    public byte Size;		// 4 + NumP * 28
    public Enums.ISP Type;		// ISP_MCI
    public byte ReqI;		// 0 unless this is a reply to an TINY_MCI request
    public byte NumC;		// number of valid CompCar structs in this packet

    public Support.CompCar[] Info; //[8];	// car info for each player, 1 to 8 of these (NumC)

    public IS_MCI(byte[] bytes)
    {
      int position = 0;
      Size = bytes[position++];
      Type = (Enums.ISP)bytes[position++];
      ReqI = bytes[position++];
      NumC = bytes[position++];
      Info = new Support.CompCar[NumC];
      for (int i = 0; i < NumC; i++)
      {
        byte[] compCarBytes = new byte[Support.CompCar.SIZE];
        Array.Copy(bytes,position,compCarBytes,0,Support.CompCar.SIZE);
        position += Support.CompCar.SIZE;
        Info[i] = new Support.CompCar(compCarBytes);
      }
    }

    #region ILfsInSimPacket Members

    public Enums.ISP PacketType
    {
      get { return Type; }
    }

    #endregion
  }

  // You can change the rate of NLP or MCI after initialisation by sending this IS_SMALL :

  // ReqI : 0
  // SubT : SMALL_NLI		(Node Lap Interval)
  // UVal : interval      (0 means stop, otherwise interval in ms, 100 to 8000)
}
