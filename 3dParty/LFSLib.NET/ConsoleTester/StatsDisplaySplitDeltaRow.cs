using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;

namespace FullMotion.LiveForSpeed
{
  class StatsDisplaySplitDeltaRow : StatsDisplayLapDeltaRow
  {
    public StatsDisplaySplitDeltaRow(InSimHandler handler, byte connectionId, byte startId)
      : base(handler, connectionId, startId)
    {
      label.Left = (byte)(label.Width - RaceStatsDisplay.SPLIT_WIDTH);
      label.Width = RaceStatsDisplay.SPLIT_WIDTH;
    }

    public void Show(int split, TimeSpan lapTime, TimeSpan deltaTime, byte top)
    {
      Show("Split " + split, lapTime, deltaTime, top);
    }
  }
}
