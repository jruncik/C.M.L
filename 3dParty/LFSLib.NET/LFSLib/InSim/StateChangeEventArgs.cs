using System;
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim
{

  /// <summary>
  /// StateChangeEventArgs
  /// </summary>
  public class StateChangeEventArgs : EventArgs
  {
    InSimHandler.HandlerState previous;
    InSimHandler.HandlerState current;

    internal StateChangeEventArgs(InSimHandler.HandlerState previous, InSimHandler.HandlerState current)
    {
      this.previous = previous;
      this.current = current;
    }

    /// <summary>
    /// Gets the state of the previous.
    /// </summary>
    /// <value>The previous state of the <see cref="InSimHandler"/>.</value>
    public InSimHandler.HandlerState PreviousState { get { return previous; } }

    /// <summary>
    /// Gets the current state.
    /// </summary>
    /// <value>The  state of the <see cref="InSimHandler"/>.</value>
    public InSimHandler.HandlerState CurrentState { get { return current; } }
  }
}
