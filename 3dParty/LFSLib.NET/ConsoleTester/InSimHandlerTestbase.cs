using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim;

namespace ConsoleTester
{
  public class InSimHandlerTestbase : IDisposable
  {
    protected InSimHandler handler;

    public InSimHandlerTestbase()
      : this("127.0.0.1",30000)
    {
    }

    public InSimHandlerTestbase(string host, int port)
    {
      handler = new InSimHandler();
      handler.Configuration.LFSHost = host;
      handler.Configuration.LFSHostPort = port;
      ConfigureHandler();
      handler.Initialize();

    }

    protected virtual void ConfigureHandler()
    {
      handler.Configuration.UseTCP = true;
      handler.Configuration.Local = true;
    }

    #region IDisposable Members

    public void Dispose()
    {
      handler.Close();
    }

    #endregion
  }
}
