using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim.Packets;

namespace FullMotion.LiveForSpeed.InSim
{
  class InSimPacketEventArgs : EventArgs
  {
    ILfsInSimPacket packet;

    public InSimPacketEventArgs(ILfsInSimPacket packet)
    {
      this.packet = packet;
    }

    public ILfsInSimPacket Packet { get { return packet; } }
  }

  delegate void InSimPacketReceivedEventHandler(ILfsReader sender, InSimPacketEventArgs e);

}
