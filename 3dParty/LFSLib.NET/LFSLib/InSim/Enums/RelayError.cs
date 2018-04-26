using System;
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim.Enums
{
    /// <summary>
    /// If you specify a wrong value, like invalid packet / hostname / adminpass / specpass, 
    /// the Relay returns an error event
    /// </summary>
  public enum RelayError : byte
  {
    /// <summary>
    /// Invalid packet sent by client (wrong structure / length)
    /// </summary>
    InvalidPacket = 1,
    /// <summary>
    /// Invalid packet sent by client (packet was not allowed to be forwarded to host)
    /// </summary>
IllegalPacket = 2,
    /// <summary>
    /// Wrong hostname given by client
    /// </summary>
WrongHostname = 3,
    /// <summary>
    /// Wrong admin pass given by client
    /// </summary>
WrongAdminPass = 4,
    /// <summary>
    /// Wrong spec pass given by client
    /// </summary>
    WrongSpecatorPas = 5,
  }
}
