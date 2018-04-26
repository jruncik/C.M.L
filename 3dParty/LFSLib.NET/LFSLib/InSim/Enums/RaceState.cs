using System;
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim.Enums
{
  /// <summary>
  /// State of the current race
  /// </summary>
  public enum RaceState : byte
  {
    // 0 - no race / 1 - race / 2 - qualifying
    /// <summary>
    /// No race
    /// </summary>
    NoRace,
    /// <summary>
    /// Racing
    /// </summary>
    Race,
    /// <summary>
    /// Qualifying
    /// </summary>
    Qualifying
  }
}
