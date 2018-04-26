using System;
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed
{
  public class LapData
  {
    public int Lap;
    public TimeSpan? LapTime;
    public TimeSpan?[] Splits = new TimeSpan?[3];
  }
}
