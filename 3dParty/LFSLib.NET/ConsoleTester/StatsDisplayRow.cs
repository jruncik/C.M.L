using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Enums;

namespace FullMotion.LiveForSpeed
{
  abstract class StatsDisplayRow
  {
   
    protected InSimHandler handler;
    protected byte connectionId;
    protected byte height;
    protected byte lastId;
    protected List<LfsButton> buttons = new List<LfsButton>();

    public StatsDisplayRow(InSimHandler handler, byte connectionId, byte startId)
    {
      this.connectionId = connectionId;
      this.handler = handler;
      lastId = startId;
    }

    public byte ButtonCount { get { return (byte)buttons.Count; } }
    public byte Height { get { return height; } }
    protected void Show()
    {
      foreach(LfsButton b in buttons)
      {
        handler.ShowButton(b,connectionId);
      }
    }

    public void Hide()
    {
      foreach(LfsButton b in buttons)
      {
        handler.DeleteButton(b,connectionId);
      }
    }

    protected LfsButton CreateLabelButton()
    {
      LfsButton b = CreateButton();
      b.Width = RaceStatsDisplay.LABEL_WIDTH;
      b.Left = RaceStatsDisplay.LEFT;
      return b;
    }

    protected LfsButton CreateTimeButton()
    {
      LfsButton b = CreateButton();
      b.Width = RaceStatsDisplay.TIME_WIDTH;
      b.Left = RaceStatsDisplay.LEFT + RaceStatsDisplay.LABEL_WIDTH;
      return b;
    }

    private LfsButton CreateButton()
    {
      LfsButton b = new LfsButton(lastId++);
      b.Color = ButtonColor.Dark;
      b.TextColor = RaceStatsDisplay.DEFAULT_COLOR;
      b.Height = RaceStatsDisplay.ROW_HEIGHT;
      b.TextAlignment = ButtonTextAlignment.Right;
      buttons.Add(b);
      return b;
    }

    protected string FormatTime(TimeSpan t)
    {
      int secFraction = t.Milliseconds / 10;
      return string.Format("{0}:{1:00}.{2:00}", t.Minutes, t.Seconds, secFraction);
    }

 
  }
}
