using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FullMotion.LiveForSpeed.InSim;

namespace ConsoleTester
{
  class RaceStart : IDisposable
  {
    public static void Test()
    {
      using (new RaceStart())
      {
        Console.WriteLine("Press RETURN to exit");
        Console.ReadLine();
      }
    }


    InSimHandler handler;

    private RaceStart()
    {
      handler = new InSimHandler();
      handler.Configuration.LFSHost = "127.0.0.1";
      handler.Configuration.LFSHostPort = 30000;
      handler.Configuration.UseTCP = true;
      handler.RaceTrackRaceStart += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackRaceStartHandler(handler_RaceTrackRaceStart);
      handler.Initialize();
    }

    void handler_RaceTrackRaceStart(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackRaceStart e)
    {
      Thread x = new Thread(new ThreadStart(HandleRaceStart));
      x.Start();
    }

    private void HandleRaceStart()
    {
      LfsButton b = new LfsButton(1);
      b.Left = 30;
      b.Top = 10;
      b.Width = 140;
      b.Height = 20;
      b.Text = "Wait for green light!";
      handler.ShowButton(b, true);
      Thread.Sleep(4000);
      handler.DeleteButton(b.ButtonId, true);
      Thread.Sleep(2000);
      b.Text = "GO!";
      handler.ShowButton(b, true);
      Thread.Sleep(2000);
      handler.DeleteButton(b.ButtonId, true);
    }

    #region IDisposable Members

    public void Dispose()
    {
      handler.Close();
    }

    #endregion
  }
}
