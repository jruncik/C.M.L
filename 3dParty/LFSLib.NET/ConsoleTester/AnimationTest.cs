using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Enums;
using System.Threading;

namespace ConsoleTester
{
  class AnimationTest : InSimHandlerTestbase
  {
    public AnimationTest()
    {
      LfsButton b = new LfsButton(1);
      b.Text = "Foo";
      b.Top = 50;
      b.Left = 0;
      b.Height = 8;
      b.Width = 15;
      b.Color = ButtonColor.Dark;
      for (byte i = 50; i > 30; i -= 2)
      {
        b.Top = i;
        handler.ShowButton(b, true);
        Thread.Sleep(50);
      }
    }
  }
}
