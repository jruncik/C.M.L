using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Events;

namespace FullMotion.LiveForSpeed
{
  // TODO: there is an implicit assumption that there will never be more than one player per connection
  public class Player
  {
    #region log4net -------------------------------------------------------------------------------
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion
    
    RaceStatsDisplay raceStatsDisplay;
    InSimHandler handler;
    byte connectionId;
    byte playerId;
    string playerName;
    List<LapData> laps = new List<LapData>();
    int bestLapIdx;
    int currentLapIdx = 0;
    int currentLap = 0;

    public Player(InSimHandler handler, byte connectionId, byte playerId, string playerName)
    {
      log.DebugFormat("Added player {0} with id {1} on connection {2}", playerName, playerId, connectionId);
      this.handler = handler;
      this.connectionId = connectionId;
      this.playerId = playerId;
      this.playerName = playerName;
      this.raceStatsDisplay = new RaceStatsDisplay(handler, connectionId);
      this.bestLapIdx = 0;
      laps.Add(new LapData());
      this.handler.RaceTrackNodeLap += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackNodeLapPacketHandler(handler_RaceTrackNodeLap);
      this.handler.RaceTrackPlayerLap += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerLapHandler(handler_RaceTrackPlayerLap);
      this.handler.RaceTrackPlayerSplitTime += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerSplitTimeHandler(handler_RaceTrackPlayerSplitTime);
      handler.RequestRaceTrackingNodeLap();
    }

    private LapData Current
    {
      get
      {
        if (laps.Count <= currentLapIdx)
        {
          laps.Add(new LapData());
          laps[currentLapIdx].Lap = currentLap;
        }
        return laps[currentLapIdx];
      }
    }

    void handler_RaceTrackNodeLap(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackNodeLap e)
    {
      foreach (NodeLapInfo nodeLap in e.LapInfo)
      {
        if (nodeLap.PlayerId == playerId)
        {
          Current.Lap = currentLap = nodeLap.Lap;
        }
      }
    }

    void handler_RaceTrackPlayerSplitTime(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerSplitTime e)
    {
      if (e.PlayerId == playerId)
      {
        if (e.Split > 1 && !Current.Splits[0].HasValue)
        {
          log.Debug("not recording split for this lap");
          // invalid lap, ignore
          return;
        }

        Current.Splits[e.Split - 1] = e.SplitTime;
        Console.WriteLine("{0}: lap {1}/split {2}: {3}", e.PlayerId, Current.Lap, e.Split, e.SplitTime.TotalSeconds);
        raceStatsDisplay.Render(laps, bestLapIdx);
      }
    }

    void handler_RaceTrackPlayerLap(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerLap e)
    {
      if (e.PlayerId == playerId)
      {
        if (!Current.Splits[0].HasValue)
        {
          log.Debug("not recording laptime for this lap");
          // invalid lap, ignore
          return;
        }

        Current.LapTime = e.LapTime;
        Current.Lap = e.LapsDone;
        Console.WriteLine("{0}: lap {1}: {2}", e.PlayerId, Current.Lap, e.LapTime.TotalSeconds);

        if (Current.LapTime < laps[bestLapIdx].LapTime)
        {
          bestLapIdx = currentLapIdx;
          Console.WriteLine("Is Best");
        }
        currentLapIdx++;
        currentLap = e.LapsDone + 1;
        raceStatsDisplay.Render(laps, bestLapIdx);
      }
    }
  }
}
