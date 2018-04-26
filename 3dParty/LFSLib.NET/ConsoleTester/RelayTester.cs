using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Events;
using System.Threading;
using FullMotion.LiveForSpeed.InSim.EventHandlers;

namespace ConsoleTester
{
  class RelayTester : IDisposable
  {
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    IInSimRelayHandler relay;
    ManualResetEvent block = new ManualResetEvent(false);

    public RelayTester()
    {
      log.Debug("starting relay tester");
      relay = InSimHandler.GetRelayHandler("vic.lfs.net", 47474);
      relay.RelayHostlist += new RelayHostlistEventHandler(relay_RelayHostlist);
      relay.RelayError += new RelayErrorEventHandler(relay_RelayError);
      relay.RaceTrackRaceStart += new RaceTrackRaceStartHandler(relay_RaceTrackRaceStart);
      relay.RaceTrackPlayer += new RaceTrackPlayerHandler(relay_RaceTrackPlayer);
      relay.RaceTrackConnection += new RaceTrackNewConnectionHandler(relay_RaceTrackConnection);
      relay.RelayHostConnected += new RelayHostConnectedEventHandler(relay_RelayHostConnected);
      relay.LFSState += new StateEventHandler(relay_LFSState);
      relay.Vote += new VoteEventHandler(relay_Vote);
      relay.Ping += new PingEventHandler(relay_Ping);
      relay.Initialize();
      relay.RequestRelayHostlist();
      block.Reset();
      block.WaitOne();
      Console.WriteLine("pinging");
      relay.RequestPing();

    }

    void relay_Ping(InSimHandler sender, Ping e)
    {
      Console.WriteLine("Ping {0}", e.RequestId);
    }

    void relay_LFSCameraPosition(InSimHandler sender, CameraPosition e)
    {
      Console.WriteLine("cam position {0}", e.Camera);
    }

    void relay_Vote(InSimHandler sender, VoteEventArgs e)
    {
      if (e is VoteAction)
      {
        VoteAction action = (VoteAction)e;
        Console.WriteLine("vote action '{0}' from {1} (final: {2})", action.Vote, action.ConnectionId, action.IsFinalResult);
        if (action.Vote == FullMotion.LiveForSpeed.InSim.Enums.Vote.End)
        {
          relay.CancelVote();
        }
      }
      else
      {
        Console.WriteLine("vote was cancelled");
      }

    }

    void relay_RaceTrackMultiCarInfo(InSimHandler sender, RaceTrackMultiCarInfo e)
    {
      Console.WriteLine("Node: {0}", e.CarInfo[0].Node);
    }

    void relay_RelayHostConnected(IInSimRelayHandler sender, LFSVersion e)
    {
      Console.WriteLine("LFS Host: {0} {1}/{2}", e.Product, e.Version, e.Serial);
      relay.RequestState();
      relay.RequestRaceTrackingStart();
      relay.RequestConnectionInfo();
      relay.RequestPlayerInfo();
      block.Set();
    }

    void relay_RaceTrackConnection(InSimHandler sender, RaceTrackConnection e)
    {
      Console.WriteLine("connection/player: {0}/{1}", e.Username, e.Playername);
    }

    void relay_LFSState(InSimHandler sender, State e)
    {
      Console.WriteLine("conn/players: {0}/{1}", e.NumberOfConnections, e.NumberOfPlayers);
    }

    void relay_RelayError(IInSimRelayHandler sender, RelayError e)
    {
      Console.WriteLine("received error: {0} for ReqI {1}", e.Error, e.RequestId);
    }

    void relay_RaceTrackPlayer(InSimHandler sender, RaceTrackPlayer e)
    {
      Console.WriteLine("player: {0}({1}/{2})", e.Playername, e.PlayerId, e.ConnectionId);
      Console.WriteLine("Added weight: {0}", e.HandicapMass);
    }

    void relay_RaceTrackRaceStart(InSimHandler sender, RaceTrackRaceStart e)
    {
      Console.WriteLine("race: {0} - {1}", e.ShortTrackName, e.LapInfo.Type);
      Console.WriteLine("nodes: max {0}, finish {1}", e.NumberOfNodes, e.FinishNodeIndex);
    }

    void relay_RelayHostlist(IInSimRelayHandler sender, FullMotion.LiveForSpeed.InSim.Events.RelayHostlist e)
    {
      foreach (RelayHostInfo info in e.HostInfo)
      {
        Console.WriteLine("{0} - {1}: {2}", info.Name, info.ShortTrackname, info.NumberOfConnections);
        if (info.Name.Contains("Full Motion"))
        {
          relay.ConnectToHost(info.Name, "zxcyui");
        }
      }
    }
    #region IDisposable Members

    public

    void Dispose()
    {
      relay.Close();
    }

    #endregion
  }
}
