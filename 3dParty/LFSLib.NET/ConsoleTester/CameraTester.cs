using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim.EventHandlers;
using FullMotion.LiveForSpeed.InSim;
using System.Threading;

namespace ConsoleTester
{
  class CameraTester : InSimHandlerTestbase
  {

    byte playerId;

    ManualResetEvent trigger = new ManualResetEvent(false);
      CameraPositionInfo cam;

    public CameraTester()
      : base("127.0.0.1", 30000)
    {
      Console.WriteLine("waiting for connection");
      trigger.WaitOne();
      trigger.Reset();

      
      Console.WriteLine("Press something");
      bool loop = true;
      while (loop)
      {
        trigger.Reset();
        handler.RequestCameraPosition();
        Console.WriteLine("waiting for camera pack");
        trigger.WaitOne();
        ConsoleKeyInfo key = Console.ReadKey();
        switch (key.Key)
        {
          case ConsoleKey.Q:
            loop = false;
            break;
          case ConsoleKey.U:
            cam.InShiftUMode = !cam.InShiftUMode;
            break;
          case ConsoleKey.F:
            cam.IsFollowing = !cam.IsFollowing;
            break;
          case ConsoleKey.H:
            cam.IsHighView = !cam.IsHighView;
            break;
          case ConsoleKey.R:
            trigger.Reset();
            handler.RequestCameraPosition();
            Console.WriteLine("waiting for camera pack");
            trigger.WaitOne();
            break;
        }
        handler.SetCamera(cam);
      }
    }

    protected override void ConfigureHandler()
    {
      base.ConfigureHandler();
      handler.StateChanged += new StateChangeHandler(handler_StateChanged);
      handler.LFSCameraPosition += new CameraPositionEventHandler(handler_LFSCameraPosition);
    }

    void handler_LFSCameraPosition(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.CameraPosition e)
    {
      cam = e.GetCameraPositionInfo();
      trigger.Set();
    }

    void handler_StateChanged(InSimHandler sender, StateChangeEventArgs e)
    {
      if (e.CurrentState == InSimHandler.HandlerState.Connected)
      {
        trigger.Set();
      }
    }
  }
}
