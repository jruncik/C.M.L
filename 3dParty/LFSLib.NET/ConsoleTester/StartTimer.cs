using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FullMotion.LiveForSpeed.InSim;

namespace ConsoleTester
{
  class StartTimer : InSimHandlerTestbase
  {
    int playerCount;

    public StartTimer()
    {
      handler.RaceTrackRaceStart += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceStartHandler(handler_RaceTrackRaceStart);
      Console.WriteLine("Waiting for start");
    }

    void handler_RaceTrackRaceStart(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackRaceStart e)
    {
      playerCount = e.NumberInRace;
      new Thread(new ThreadStart(StartHandler)).Start();
    }

    private void StartHandler()
    {
      int delay = 6 + (playerCount >> 1);
      Console.WriteLine("sleeping for {0} seconds", delay);
      Thread.Sleep(delay * 1000);
      LfsButton b = new LfsButton(1);
      b.Top = 30;
      b.Left = 50;
      b.Width = 100;
      b.Height = 20;
      b.Text = "Go!";
      handler.ShowButton(b, true);
      Thread.Sleep(2000);
      handler.DeleteButton(b.ButtonId, true);
    }
  }
}
