using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Enums;

namespace FullMotion.LiveForSpeed
{
  class StatsDisplayLapDeltaRow : StatsDisplayRow
  {
    protected LfsButton label;
    protected LfsButton time;
    protected LfsButton delta;

    public StatsDisplayLapDeltaRow(InSimHandler handler, byte connectionId, byte startId)
      : base(handler, connectionId, startId)
    {
      label = CreateLabelButton();
      label.TextAlignment = ButtonTextAlignment.Left;
      time = CreateTimeButton();
      delta = CreateTimeButton();
      height = (byte)(label.Height + delta.Height);
    }

    public void Show(int lap, TimeSpan lapTime, TimeSpan deltaTime, bool best, byte top)
    {
      if (best)
      {
        label.TextColor = time.TextColor = RaceStatsDisplay.BEST_COLOR;
      }
      else
      {
        label.TextColor = time.TextColor = RaceStatsDisplay.DEFAULT_COLOR;
      }
      string text = lap.ToString();
      label.TextAlignment = ButtonTextAlignment.Right;
      if (lap == 0)
      {
        text = "Total";
        label.TextAlignment = ButtonTextAlignment.Left;
      }
      Show(text, lapTime, deltaTime, top);
    }

    protected void Show(string text, TimeSpan lapTime, TimeSpan deltaTime, byte top)
    {
      label.Top = time.Top = top;
      delta.Top = (byte)(top + label.Height);

      label.Text = text;
      time.Text = FormatTime(lapTime);
      
      if (deltaTime.TotalMilliseconds < 0)
      {
        delta.Text = "-" + FormatTime(deltaTime.Negate());
        delta.TextColor = RaceStatsDisplay.DELTA_MINUS_COLOR;
      }
      else
      {
        delta.Text = "+" + FormatTime(deltaTime);
        delta.TextColor = RaceStatsDisplay.DELTA_PLUS_COLOR;
      }
      Show();
    }

  }
}
