using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Enums;
using System.Threading;

namespace ConsoleTester
{
  class PositionTester : InSimHandlerTestbase
  {
    byte id = 0;
    public PositionTester()
      :base("127.0.0.1",30000)
    {
      LfsButton raceControlButton = CreateButton(50, 20, 100, 20);
      raceControlButton.Text = "race control";
      handler.ShowButton(raceControlButton, true);

      LfsButton drivingAidsButton = CreateButton(50, 40, 100, 15 );
      drivingAidsButton.Color = ButtonColor.Transparent;
      drivingAidsButton.Text = "driving aids";
      handler.ShowButton(drivingAidsButton, true);
      LfsButton drivingAidsBack = CreateButton(50, 40, 100, 20);
      handler.ShowButton(drivingAidsBack, true);

      byte center = 100;
      byte width = 40;
      LfsButton gaugeButton = CreateButton((byte)(center - width), 55, (byte)(width * 2), 5);
      gaugeButton.Color = ButtonColor.Light;
      LfsButton gaugeButton2 = CreateButton((byte)(center - width), 55, (byte)(width * 2), 5);
      gaugeButton2.Color = ButtonColor.Light;
      while (true)
      {
        handler.ShowButton(gaugeButton, true);
        handler.ShowButton(gaugeButton2, true);
        Thread.Sleep(200);
        width -= 1;
        if (width <= 0)
        {
          width = 50;
        }
        gaugeButton.Width = gaugeButton2.Width = (byte)(width * 2);
        gaugeButton.Left = gaugeButton2.Left = (byte)(center - width);
      }

    }

    public LfsButton CreateButton(byte left, byte top, byte width, byte height)
    {
      LfsButton button = new LfsButton(id++);
      button.Left = left;
      button.Top = top;
      button.Height = height;
      button.Width = width;
      button.Color = ButtonColor.Dark;
      button.TextAlignment = ButtonTextAlignment.Center;
      return button;
    }
    void handler_RaceTrackMultiCarInfo(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackMultiCarInfo e)
    {
    }

    protected override void ConfigureHandler()
    {
      base.ConfigureHandler();
      handler.Configuration.MultiCarTracking = true;
      handler.Configuration.TrackingInterval = 100;
    }
  }
}
