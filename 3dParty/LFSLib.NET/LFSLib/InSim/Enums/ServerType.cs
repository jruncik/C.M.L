using System;
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim.Enums
{
  /// <summary>
  /// Server Type that InSim is connected to
  /// </summary>
  public enum ServerType : byte
  {
    /// <summary>
    /// The InSim Server is a client
    /// </summary>
    Guest = 0,
    /// <summary>
    /// The InSim Server is a game host
    /// </summary>
    Host = 1,
  }
}
