using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim.EventHandlers;
using FullMotion.LiveForSpeed.InSim;
using System.Threading;

namespace ConsoleTester
{
  class EventTester : InSimHandlerTestbase
  {

    byte playerId;

    public EventTester()
      :base("127.0.0.1",30000)
    {
      //handler.RequestPlayerInfo();
    }
  
    protected override void ConfigureHandler()
    {
      base.ConfigureHandler();
      handler.Configuration.AutoReconnect = true;
      handler.Configuration.ReconnectRetry = 100;
      handler.Configuration.AdminPass = "zxcyui";
      handler.StateChanged += new StateChangeHandler(handler_StateChanged);
      handler.RaceTrackConnection += new RaceTrackNewConnectionHandler(handler_RaceTrackConnection);
      handler.RaceTrackConnectionLeave += new RaceTrackConnectionLeaveHandler(handler_RaceTrackConnectionLeave);
      handler.RaceTrackPlayer += new RaceTrackPlayerHandler(handler_RaceTrackPlayer);
      handler.RaceTrackPlayerLeave += new RaceTrackPlayerLeaveHandler(handler_RaceTrackPlayerLeave);
      handler.LFSMultiplayer += new MultiplayerEventHandler(handler_LFSMultiplayer);
      handler.RaceTrackPlayerLap += new RaceTrackPlayerLapHandler(handler_RaceTrackPlayerLap);
      handler.RaceTrackPlayerSplitTime += new RaceTrackPlayerSplitTimeHandler(handler_RaceTrackPlayerSplitTime);
      handler.RaceTrackPlayerPitlane += new RaceTrackPlayerPitlaneHandler(handler_RaceTrackPlayerPitlane);
      handler.LFSMessage += new MessageEventHandler(handler_LFSMessage);
      handler.RaceTrackCarReset += new RaceTrackCarResetHandler(handler_RaceTrackCarReset);
      handler.LFSState += new StateEventHandler(handler_LFSState);
      handler.RaceTrackPlayerPenalty += new RaceTrackPlayerPenaltyHandler(handler_RaceTrackPlayerPenalty);
      handler.RaceTrackRaceStart += new RaceTrackRaceStartHandler(handler_RaceTrackRaceStart);
    }

    void handler_RaceTrackRaceStart(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackRaceStart e)
    {
      Console.WriteLine("Track {0}: {1},{2},{3},{4}", e.ShortTrackName, e.FinishNodeIndex, e.Split1NodeIndex, e.Split2NodeIndex, e.Split3NodeIndex);
    }

    void handler_RaceTrackPlayerPenalty(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerPenalty e)
    {
      Console.WriteLine("penalty: {0}/{1} ({2})", e.NewPenalty, e.OldPenalty, e.Reason);
    }

    void handler_LFSState(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.State e)
    {
      Console.WriteLine("got lfs state");
      handler.RequestPlayerInfo();
    }

    void handler_RaceTrackCarReset(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackCarReset e)
    {
    }

    void handler_LFSMessage(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.Message e)
    {
    }

    void handler_RaceTrackPlayerPitlane(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerPitlane e)
    {
      Console.WriteLine("got pitlane of type '{0}'",e.EventType);
    }


    void handler_StateChanged(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.StateChangeEventArgs e)
    {
      Console.WriteLine("previous state: {0}, current state: {1}", e.PreviousState, e.CurrentState);
    }

    void handler_RaceTrackPlayerSplitTime(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerSplitTime e)
    {
      Console.WriteLine("split: {0}", e.SplitTime);
    }

    void handler_RaceTrackPlayerLap(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerLap e)
    {
       Console.WriteLine("lap: {0}", e.LapTime);
    }

    void handler_LFSMultiplayer(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.Multiplayer e)
    {
      handler.RequestConnectionInfo();
    }

    void handler_RaceTrackPlayerLeave(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerLeave e)
    {
      if (e.PlayerId == playerId)
      {
        Console.WriteLine("spectating");
      }
    }

    void handler_RaceTrackConnectionLeave(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackConnectionLeave e)
    {
    }

    void handler_RaceTrackPlayer(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayer e)
    {
      if (!e.Remote && !e.AI)
      {
        Console.WriteLine("my player id is {0}", e.PlayerId);
        playerId = e.PlayerId;
      }
      Console.WriteLine("Added weight: {0}", e.HandicapMass);
      if (e.Playername == "sim 1")
      {
        playerId = e.PlayerId;
        Console.WriteLine("joined the game");
      }
    }

    void handler_RaceTrackConnection(FullMotion.LiveForSpeed.InSim.InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackConnection e)
    {
      Console.WriteLine("ConnectionId: {0}", e.ConnectionId);
      Console.WriteLine("Username:     {0}", e.Username);
      Console.WriteLine("Playername:   {0}", e.Playername);
      Console.WriteLine("Remote:       {0}", e.IsRemote);
    }
  }
}
