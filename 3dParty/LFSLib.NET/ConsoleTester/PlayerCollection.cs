using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;

namespace FullMotion.LiveForSpeed
{
  // TODO: Race Start/Restart awareness
  class PlayerCollection : Dictionary<int, Player>
  {
    public static void Test()
    {
      InSimHandler handler = new InSimHandler();
      handler.Configuration.LFSHost = "127.0.0.1";
      handler.Configuration.LFSHostPort = 50000;
      handler.Configuration.UseTCP = true;
      handler.Initialize();
      
      PlayerCollection players = new PlayerCollection(handler);
      Console.WriteLine("Press Enter to Exit");
      Console.ReadLine();
      handler.Close();
    }

    InSimHandler handler;

    public PlayerCollection(InSimHandler handler)
    {
      this.handler = handler;
      handler.RaceTrackPlayer += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerHandler(handler_RaceTrackPlayer);
      handler.RequestPlayerInfo();
    }

    void handler_RaceTrackPlayer(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayer e)
    {
      if (!ContainsKey(e.PlayerId))
      {
        Player player = new Player(handler, e.ConnectionId, e.PlayerId, e.Playername);
        this.Add(e.PlayerId, player);
      }
    }
  }
}
