using System;
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim.Flags
{
  [Flags()]
  enum PlayerType : byte
  {
    // bit 0 : female / bit 1 : AI / bit 2 : remote
    Female = 1,
    AI = 2,
    Remote = 4,
  }
}
