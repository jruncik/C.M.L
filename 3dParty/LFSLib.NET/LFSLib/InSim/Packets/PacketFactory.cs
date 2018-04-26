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
using System.Runtime.InteropServices;
using System.Text;
using log4net;
using FullMotion.LiveForSpeed.Util;

namespace FullMotion.LiveForSpeed.InSim.Packets
{
  class PacketFactory
  {
    #region Static Members ########################################################################
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private static object padlock = new object();
    private static byte requestId;
    private static Type typeType = typeof(Enums.ISP);

    internal static byte NextRequestId
    {
      get
      {
        lock (padlock)
        {
          if (requestId == 255)
          {
            requestId = 1;
          }
          else
          {
            requestId++;
          }
        }
        return requestId;
      }
    }

    public static int CheckHeader(byte[] bytes, int offset)
    {
      byte size = bytes[offset];
      Enums.ISP type = (Enums.ISP)bytes[offset + 1];

      if (!Enum.IsDefined(typeType, type))
      {
        // not a packet we know
        return 0;
      }
      return size;
    }

    public static ILfsInSimPacket GetPacket(byte[] bytes, int offset)
    {
      byte size = bytes[offset];
      Enums.ISP type = (Enums.ISP)bytes[offset + 1];

      byte[] packetBytes = new byte[size];
      Array.Copy(bytes, offset, packetBytes, 0, size);

      ILfsInSimPacket packet = null;
      switch (type)
      {
        case Enums.ISP.AXI:
          packet = new IS_AXI(packetBytes);
          break;
        case Enums.ISP.AXO:
          packet = GetPacketSequentially(typeof(IS_AXO), packetBytes);
          break;
        case Enums.ISP.BFN:
          packet = GetPacketSequentially(typeof(IS_BFN), packetBytes);
          break;
        case Enums.ISP.BTC:
          packet = GetPacketSequentially(typeof(IS_BTC), packetBytes);
          break;
        case Enums.ISP.BTN:
          packet = new IS_BTN(packetBytes);
          break;
        case Enums.ISP.BTT:
          packet = new IS_BTT(packetBytes);
          break;
        case Enums.ISP.CCH:
          packet = GetPacketSequentially(typeof(IS_CCH), packetBytes);
          break;
        case Enums.ISP.CNL:
          packet = GetPacketSequentially(typeof(IS_CNL), packetBytes);
          break;
        case Enums.ISP.CPP:
          packet = GetPacketSequentially(typeof(IS_CPP), packetBytes);
          break;
        case Enums.ISP.CPR:
          packet = GetPacketSequentially(typeof(IS_CPR), packetBytes);
          break;
        case Enums.ISP.CRS:
          packet = GetPacketSequentially(typeof(IS_CRS), packetBytes);
          break;
        case Enums.ISP.FIN:
          packet = GetPacketSequentially(typeof(IS_FIN), packetBytes);
          break;
        case Enums.ISP.FLG:
          packet = GetPacketSequentially(typeof(IS_FLG), packetBytes);
          break;
        case Enums.ISP.III:
          packet = new IS_III(packetBytes);
          break;
        case Enums.ISP.ISI:
          packet = GetPacketSequentially(typeof(IS_ISI), packetBytes);
          break;
        case Enums.ISP.ISM:
          packet = new IS_ISM(packetBytes);
          break;
        case Enums.ISP.LAP:
          packet = GetPacketSequentially(typeof(IS_LAP), packetBytes);
          break;
        case Enums.ISP.MCI:
          packet = new IS_MCI(packetBytes);
          break;
        case Enums.ISP.MOD:
          packet = GetPacketSequentially(typeof(IS_MOD), packetBytes);
          break;
        case Enums.ISP.MSL:
          packet = new IS_MSL(packetBytes);
          break;
        case Enums.ISP.MSO:
          packet = new IS_MSO(packetBytes);
          break;
        case Enums.ISP.MST:
          packet = new IS_MST(packetBytes);
          break;
        case Enums.ISP.MSX:
          packet = new IS_MSX(packetBytes);
          break;
        case Enums.ISP.MTC:
          packet = new IS_MTC(packetBytes);
          break;
        case Enums.ISP.NCN:
          packet = GetPacketSequentially(typeof(IS_NCN), packetBytes);
          break;
        case Enums.ISP.NLP:
          packet = new IS_NLP(packetBytes);
          break;
        case Enums.ISP.NPL:
          packet = GetPacketSequentially(typeof(IS_NPL), packetBytes);
          break;
        case Enums.ISP.PEN:
          packet = GetPacketSequentially(typeof(IS_PEN), packetBytes);
          break;
        case Enums.ISP.PFL:
          packet = GetPacketSequentially(typeof(IS_PFL), packetBytes);
          break;
        case Enums.ISP.PIT:
          packet = GetPacketSequentially(typeof(IS_PIT), packetBytes);
          break;
        case Enums.ISP.PLA:
          packet = GetPacketSequentially(typeof(IS_PLA), packetBytes);
          break;
        case Enums.ISP.PLL:
          packet = GetPacketSequentially(typeof(IS_PLL), packetBytes);
          break;
        case Enums.ISP.PLP:
          packet = GetPacketSequentially(typeof(IS_PLP), packetBytes);
          break;
        case Enums.ISP.PSF:
          packet = GetPacketSequentially(typeof(IS_PSF), packetBytes);
          break;
        case Enums.ISP.REO:
          packet = new IS_REO(packetBytes);
          break;
        case Enums.ISP.RES:
          packet = GetPacketSequentially(typeof(IS_RES), packetBytes);
          break;
        case Enums.ISP.RST:
          packet = GetPacketSequentially(typeof(IS_RST), packetBytes);
          break;
        case Enums.ISP.SCC:
          packet = GetPacketSequentially(typeof(IS_SCC), packetBytes);
          break;
        case Enums.ISP.SCH:
          packet = GetPacketSequentially(typeof(IS_SCH), packetBytes);
          break;
        case Enums.ISP.SFP:
          packet = GetPacketSequentially(typeof(IS_SFP), packetBytes);
          break;
        case Enums.ISP.SMALL:
          packet = GetPacketSequentially(typeof(IS_SMALL), packetBytes);
          break;
        case Enums.ISP.SPX:
          packet = GetPacketSequentially(typeof(IS_SPX), packetBytes);
          break;
        case Enums.ISP.STA:
          packet = GetPacketSequentially(typeof(IS_STA), packetBytes);
          break;
        case Enums.ISP.TINY:
          packet = GetPacketSequentially(typeof(IS_TINY), packetBytes);
          break;
        case Enums.ISP.TOC:
          packet = GetPacketSequentially(typeof(IS_TOC), packetBytes);
          break;
        case Enums.ISP.VER:
          packet = GetPacketSequentially(typeof(IS_VER), packetBytes);
          break;
        case Enums.ISP.VTN:
          packet = GetPacketSequentially(typeof(IS_VTN), packetBytes);
          break;
        case Enums.ISP.IRP_ERR:
          packet = GetPacketSequentially(typeof(IR_ERR), packetBytes);
          break;
        case Enums.ISP.IRP_HOS:
          packet = new IR_HOS(packetBytes);
          break;
        default:
          log.WarnFormat("We're not catching packet type {0}", type);
          break;
      }
      return packet;
    }

    public static IClientLfsInSimPacket GetSingleKeyPacket(KeyPress keyPress)
    {
      return new IS_SCH(keyPress);
    }

    public static IClientLfsInSimPacket GetMessagePacket(string message)
    {
      byte[] stringBytes = CharHelper.GetBytes(message, 96, true);
      return GetMessagePacket(stringBytes);
    }

    public static IClientLfsInSimPacket GetMessagePacket(byte[] bytes)
    {
      if (bytes[0] == '/' || bytes.Length < 64 || bytes[64] == 0)
      {
        // IS_MST
        byte[] msgBytes = new byte[64];
        int count = Math.Min(bytes.Length, 64);
        Array.Copy(bytes, msgBytes, count);
        return new IS_MST(msgBytes);
      }
      else
      {
        // IS_MSX
        byte[] msgBytes = new byte[96];
        int count = Math.Min(bytes.Length, 95);
        Array.Copy(bytes, msgBytes, count);
        return new IS_MSX(msgBytes);
      }
    }

    public static IClientLfsInSimPacket GetMessageToConnectionPacket(string message, byte connectionId)
    {
      return new IS_MTC(connectionId, message, false);
    }

    public static IClientLfsInSimPacket GetMessageToPlayerPacket(string message, byte playerId)
    {
      return new IS_MTC(playerId, message, true);
    }

    public static IClientLfsInSimPacket GetStateModPacket(Support.StateModHelper flagHelper, bool on)
    {
      return new IS_SFP(flagHelper.Flag, on);
    }

    public static IClientLfsInSimPacket GetRaceTrackingIntervalRequestPacket(int interval)
    {
      return new IS_SMALL(Enums.SMALL.NLI, NextRequestId, (uint)interval);
    }

    public static IClientLfsInSimPacket GetOutGaugeCommandPacket(int interval)
    {
      return new IS_SMALL(Enums.SMALL.SSG, NextRequestId, (uint)interval);
    }

    public static IClientLfsInSimPacket GetOutSimCommandPacket(int interval)
    {
      return new IS_SMALL(Enums.SMALL.SSP, NextRequestId, (uint)interval);
    }

    public static IClientLfsInSimPacket GetAutocrossInfoRequestPacket()
    {
      return new IS_TINY(Enums.TINY.AXI, 0);
    }

    public static IClientLfsInSimPacket GetKeepAlivePacket()
    {
      return new IS_TINY(Enums.TINY.NONE, 0);
    }

    public static IClientLfsInSimPacket GetPingPacket()
    {
      return new IS_TINY(Enums.TINY.PING, NextRequestId);
    }

    public static IClientLfsInSimPacket GetClosePacket()
    {
      return new IS_TINY(Enums.TINY.CLOSE, 0);
    }

    public static IClientLfsInSimPacket GetVersionRequestPacket()
    {
      return new IS_TINY(Enums.TINY.VER, NextRequestId);
    }

    public static IClientLfsInSimPacket GetStateRequestPacket()
    {
      return new IS_TINY(Enums.TINY.SST, NextRequestId);
    }

    public static IClientLfsInSimPacket GetRaceTrackingResultRequestPacket()
    {
      return new IS_TINY(Enums.TINY.RES, NextRequestId);
    }

    public static IClientLfsInSimPacket GetRaceTrackingStartRequestPacket()
    {
      return new IS_TINY(Enums.TINY.RST, NextRequestId);
    }

    public static IClientLfsInSimPacket GetRaceTrackingConnectionRequestPacket()
    {
      return new IS_TINY(Enums.TINY.NCN, NextRequestId);
    }

    public static IClientLfsInSimPacket GetRaceTrackingPlayerRequestPacket()
    {
      return new IS_TINY(Enums.TINY.NPL, NextRequestId);
    }

    public static IClientLfsInSimPacket GetRaceTrackingNodeLapRequestPacket()
    {
      return new IS_TINY(Enums.TINY.NLP, NextRequestId);
    }

    public static IClientLfsInSimPacket GetRaceTrackingMultiCarInfoRequestPacket()
    {
      return new IS_TINY(Enums.TINY.MCI, NextRequestId);
    }

    public static IClientLfsInSimPacket GetMultiplayerRequestPacket()
    {
      return new IS_TINY(Enums.TINY.ISM, NextRequestId);
    }

    public static IClientLfsInSimPacket GetReorderRequestPacket()
    {
      return new IS_TINY(Enums.TINY.REO, NextRequestId);
    }

    public static IClientLfsInSimPacket GetCameraPositionRequestPacket()
    {
      return new IS_TINY(Enums.TINY.SCP, NextRequestId);
    }

    public static IClientLfsInSimPacket GetVoteCancelPacket()
    {
      return new IS_TINY(Enums.TINY.VTC, NextRequestId);
    }

    public static IClientLfsInSimPacket GetScreenModePacket(int width, int height, int refreshRate, bool screenDepth16)
    {
      return new IS_MOD(width, height, refreshRate, screenDepth16);
    }

    public static IClientLfsInSimPacket GetCarCameraRequest(byte playerId, Enums.View camera)
    {
      return new IS_SCC(playerId, camera);
    }

    public static IClientLfsInSimPacket GetRaceTimeControlPacket(bool run)
    {
      return new IS_SMALL(Enums.SMALL.TMS, NextRequestId, (uint)((run) ? 0 : 1));
    }

    public static IClientLfsInSimPacket GetRaceTimeControlPacket(int stepTime)
    {
      return new IS_SMALL(Enums.SMALL.STP, NextRequestId, (uint)stepTime);
    }

    public static IClientLfsInSimPacket GetRaceTimeRequestPacket()
    {
      return new IS_TINY(Enums.TINY.GTH, NextRequestId);
    }

    public static IClientLfsInSimPacket GetLocalMessagePacket(string message, FullMotion.LiveForSpeed.InSim.Enums.Sound soundEffect)
    {
      return new IS_MSL(message, soundEffect);
    }

    public static IClientLfsInSimPacket GetGridOrderPacket(byte[] orderedPlayerIds)
    {
      return new IS_REO(orderedPlayerIds, NextRequestId);
    }

    public static IClientLfsInSimPacket GetButtonClearPacket(byte connectionId)
    {
      return new IS_BFN(Enums.BFN.CLEAR, connectionId, (byte)0);
    }

    public static IClientLfsInSimPacket GetButtonDeletePacket(byte buttonId, byte connectionId)
    {
      return new IS_BFN(Enums.BFN.DEL_BTN, connectionId, buttonId);
    }

    public static IClientLfsInSimPacket GetRelayHostlistRequestPacket()
    {
      return new IR_HLR(NextRequestId);
    }

    public static IClientLfsInSimPacket GetRelayHostSelectPacket(string hostname, string adminPass, string spectatorPass)
    {
      return new IR_SEL(hostname, adminPass, spectatorPass);
    }

    public static byte[] GetBytesSequentially(IClientLfsInSimPacket packet, Int32 size)
    {
      byte[] bytes = new byte[size];
      IntPtr pnt = Marshal.AllocHGlobal(size);
      try
      {
        GCHandle pin = GCHandle.Alloc(pnt);
        try
        {
          Marshal.StructureToPtr(packet, pnt, false);
          Marshal.Copy(pnt, bytes, 0, bytes.Length);
        }
        finally
        {
          pin.Free();
        }
      }
      finally
      {
        Marshal.FreeHGlobal(pnt);
      }
      return bytes;
    }

    public static ILfsInSimPacket GetPacketSequentially(Type type, byte[] bytes)
    {
      Int32 size = bytes.Length;
      IntPtr pnt = Marshal.AllocHGlobal(size);
      ILfsInSimPacket packet = null;
      try
      {
        GCHandle pin = GCHandle.Alloc(pnt, GCHandleType.Pinned);
        try
        {
          Marshal.Copy(bytes, 0, pnt, size);
          packet = (ILfsInSimPacket)Marshal.PtrToStructure(pnt, type);
        }
        finally
        {
          pin.Free();
        }
      }
      finally
      {
        Marshal.FreeHGlobal(pnt);
      }
      return packet;
    }
    #endregion

  }
}
