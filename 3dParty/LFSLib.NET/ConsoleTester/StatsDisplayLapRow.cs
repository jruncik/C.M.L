using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Enums;

namespace FullMotion.LiveForSpeed
{
  class StatsDisplayLapRow : StatsDisplayRow
  {
    protected LfsButton label;
    protected LfsButton time;

    public StatsDisplayLapRow(InSimHandler handler, byte connectionId, byte startId, byte top)
      : base(handler, connectionId, startId)
    {
      label = CreateLabelButton();
      time = CreateTimeButton();
      label.Top = time.Top = top;
      height = label.Height;
    }

    public byte Top { get { return label.Top; } }

    public void Show(int lap, TimeSpan lapTime, bool best)
    {
      if (best)
      {
        label.TextColor = time.TextColor = RaceStatsDisplay.BEST_COLOR;
      }
      else
      {
        label.TextColor = time.TextColor = RaceStatsDisplay.DEFAULT_COLOR;
      }
      label.Text = lap.ToString();
      time.Text = FormatTime(lapTime);
      Show();
    }
  }
}
