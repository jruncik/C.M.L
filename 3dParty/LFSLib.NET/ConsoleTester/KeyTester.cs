using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;

namespace ConsoleTester
{
  class KeyTester : InSimHandlerTestbase
  {
    public KeyTester()
      :base("127.0.0.1",30000)
    {
      byte g = (byte)'g';
      Console.WriteLine("{0}/{0:x00}", g, g);
      KeyPress p = new KeyPress('g');
      handler.SendKey(p);
      handler.RaceTrackRaceEnd += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceEndHandler(handler_RaceTrackRaceEnd);
    }

    void handler_RaceTrackRaceEnd(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackRaceEnd e)
    {
    }
  }
}
