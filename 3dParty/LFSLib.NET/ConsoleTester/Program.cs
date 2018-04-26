using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

using FullMotion.LiveForSpeed;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.OutSim;
using FullMotion.LiveForSpeed.OutGauge;
using FullMotion.LiveForSpeed.InSim.Exceptions;

using log4net;
using log4net.Config;
using log4net.Appender;
using log4net.Layout;
using log4net.Filter;

namespace ConsoleTester
{
  class Program
  {
    static void Main(string[] args)
    {
      ConsoleAppender appender = new ConsoleAppender();
      LoggerMatchFilter filter = null;

      filter = new LoggerMatchFilter();
      filter.LoggerToMatch = "FullMotion.LiveForSpeed.InSim";
      filter.AcceptOnMatch = false;
      appender.AddFilter(filter);

      filter = new LoggerMatchFilter();
      filter.LoggerToMatch = "FullMotion.LiveForSpeed.Util";
      filter.AcceptOnMatch = false;
      appender.AddFilter(filter);

      appender.Layout = new PatternLayout("%-6r %-25C:%L %m\n");
      BasicConfigurator.Configure(appender);

      using (new CameraTester())
      {
        Console.WriteLine("Press Enter to Exit");
        Console.ReadLine();
      }
    }
  }
}
