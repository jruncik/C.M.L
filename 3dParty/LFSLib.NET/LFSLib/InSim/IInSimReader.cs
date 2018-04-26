using System;
namespace FullMotion.LiveForSpeed.InSim
{
  interface IInSimReader : ILfsReader
  {
    event InSimPacketReceivedEventHandler InSimPacketReceived;
  }
}
