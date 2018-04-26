using System;
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim.Flags
{
  /// <summary>
  /// Modifier Key to press with the key sent in SingleCharPack
  /// </summary>
  [Flags()]
  enum KeyModifier : byte
  {
    /// <summary>
    /// Shift Key
    /// </summary>
    Shift = 1,
    /// <summary>
    /// Ctrl Key
    /// </summary>
    Ctrl = 2,
  }
}
