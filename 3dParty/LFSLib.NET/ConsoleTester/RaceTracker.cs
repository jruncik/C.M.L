using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.EventHandlers;
using FullMotion.LiveForSpeed.InSim.Events;

namespace FullMotion.LiveForSpeed
{
  /// <summary>
  /// Simple Race Tracking example. Keeps track of player names
  /// and reports the split and lap times for each player
  /// </summary>
  public class RaceTracker
  {
    /// <summary>
    /// Just a static test call for running RaceTracker against a local LFS instance
    /// </summary>
    public static void Test()
    {
      RaceTracker tracker = new RaceTracker("127.0.0.1", 30000);
      tracker.Start();
      Console.WriteLine("Press Enter to Exit");
      Console.ReadLine();
      tracker.Stop();
    }

    InSimHandler handler;

    // state variables to make sure we don't start and stop
    // the handler multiple times
    bool started = false;
    bool running = false;

    // keep track of player names, since race tracking only gives us IDs
    Dictionary<byte, string> players = new Dictionary<byte, string>();


    /// <summary>
    /// Set up an insim handler for tracking basic race info
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    public RaceTracker(string host, int port)
    {
      handler = new InSimHandler();
      handler.Configuration.LFSHost = host;
      handler.Configuration.LFSHostPort = port;
      handler.Configuration.UseTCP = true;
      handler.RaceTrackPlayer 
        += new RaceTrackPlayerHandler(handler_RaceTrackPlayer);
      handler.RaceTrackPlayerLeave 
        += new RaceTrackPlayerLeaveHandler(handler_RaceTrackPlayerLeave);
      handler.RaceTrackPlayerSplitTime 
        += new RaceTrackPlayerSplitTimeHandler(handler_RaceTrackPlayerSplitTime);
      handler.RaceTrackPlayerLap 
        += new RaceTrackPlayerLapHandler(handler_RaceTrackPlayerLap);
    }

    /// <summary>
    /// Connect to LFS and start tracking
    /// </summary>
    public void Start()
    {
      if (started)
      {
        throw new InvalidOperationException("RaceTracker cannot be started multiple times");
      }
      handler.Initialize();
      started = true;
      running = true;

      // make sure we get all players in the race
      handler.RequestPlayerInfo();
    }

    /// <summary>
    /// Close down the connection
    /// </summary>
    public void Stop()
    {
      if (running)
      {
        handler.Close();
        running = false;
      }
    }

    private void handler_RaceTrackPlayer(InSimHandler sender, RaceTrackPlayer e)
    {
      if (!players.ContainsKey(e.PlayerId))
      {
        players.Add(e.PlayerId, e.Playername);
        Console.WriteLine("Player joined: {0} ({1})", e.Playername, e.PlayerId);
      }
    }

    private void handler_RaceTrackPlayerLeave(InSimHandler sender, RaceTrackPlayerLeave e)
    {
      Console.WriteLine("Player left: {0} ({1})", players[e.PlayerId], e.PlayerId);
      players.Remove(e.PlayerId);
    }

    void handler_RaceTrackPlayerLap(InSimHandler sender, RaceTrackPlayerLap e)
    {
      Console.WriteLine(
        "Player '{0}': Lap {1} @ {2}:{3}.{4}",
        players[e.PlayerId],
        e.LapsDone,
        e.LapTime.Minutes,
        e.LapTime.Seconds,
        e.LapTime.Milliseconds);
    }

    void handler_RaceTrackPlayerSplitTime(InSimHandler sender, RaceTrackPlayerSplitTime e)
    {
      Console.WriteLine(
        "Player '{0}': Split {1} @ {2:00}:{3:00}.{4:000}",
        players[e.PlayerId],
        e.Split,
        e.SplitTime.Minutes,
        e.SplitTime.Seconds,
        e.SplitTime.Milliseconds);
    }
  }
}
