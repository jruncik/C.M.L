/* ------------------------------------------------------------------------- *
 * Copyright (C) 2008-2009 Jaroslav Runcik
 *
 * Jaroslav Runcik <J [dot] Runcik [at] seznam [dot] cz>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * ------------------------------------------------------------------------- */

using System;
using System.IO;
using System.Xml;

using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;

using System.Diagnostics;

namespace SR.CML.ReactionTester
{
	[Plugin("3897BB7A-E05F-4954-B2E6-08AE4A793F96", "Reactiontester", "Measurement of driver reaction time")]
	public class ReactionTester : IPlugin
	{
		private bool			_disposed			= false;
		private PluginState		_state				= PluginState.Undefined;
		private IPluginManager	_pluginManager		= null;
		private ICMLCore		_cmlCore			= null;
		private InSimHandler	_inSimHandler		= null;

		private ObstacleInfo		_obstacle		= null;
		private TestedDriverInfo	_testDriver		= null;

		private ILabel				_result			= null;

		private TimeSpan	_obstacleMovedTime		= TimeSpan.Zero;
		private TimeSpan	_testDriverBreakingTime	= TimeSpan.Zero;
		private bool		_measuring				= false;
		private bool		_startBreaking			= false;

		private StreamWriter	_writer				= null;

		public ReactionTester()
		{
		}

		~ReactionTester()
		{
			Debug.Assert(_disposed, "ReactionTester, call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!_disposed) {
				if (disposing) {
					if (_obstacle!=null) {
						_obstacle.Dispose();
						_obstacle = null;
					}

					if (_testDriver!=null) {
						_testDriver.Dispose();
						_testDriver = null;
					}
				}
				_disposed = true;
			}
		}

		#region IPlugin

		public PluginState State
		{
			get { return _state; }
		}

		public void Initialize(IPluginManager pluginManager)
		{
			Debug.Assert(_state == PluginState.Undefined);
			Debug.Assert(pluginManager != null);

			_pluginManager = pluginManager;

			IPlugin plugin = _pluginManager.GetPlugin(new Guid("536550BD-BE60-4fd2-91B7-098FCF43F5B1"));
			Debug.Assert(plugin!=null);
			_cmlCore = plugin as ICMLCore;
			Debug.Assert(_cmlCore!=null);

			if (_cmlCore!=null) {
				_inSimHandler = _cmlCore.InSimHandler;
			}

			_state = PluginState.Initialized;
		}

		public bool Activate()
		{
			if (_state == PluginState.Activated) {
				return true;
			}

			if (_inSimHandler==null) {
				return false;
			}

			_inSimHandler.SendMessage("/ndebug=no");
			_cmlCore.InSimHandler.TrackingInterval = 50;
			
			double tolerance = 0.1;
			// read tolerance	
			XmlDocument doc = new XmlDocument();
			doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"config.lfscnfg");
			XmlElement elem = doc.DocumentElement.SelectSingleNode("reactionTester") as XmlElement;
			if (elem!=null) {
				tolerance = Double.Parse(elem.GetAttribute("tolerance"));
			}

			_obstacle	= new ObstacleInfo(_cmlCore, tolerance);
			_testDriver	= new TestedDriverInfo(_cmlCore);

			_obstacle.StateChanged	+= new EventHandler<ObstacleStateEventArgs>(_obstacle_StateChanged);
			_testDriver.Breaking	+= new EventHandler<BreakEventArgs>(_testDriver_DriverBreaking);

			_result		= _cmlCore.ControlFactory.CreateLabel();
			_result.Left	= 80;
			_result.Top		= 30;
			_result.Width	= 40;
			_result.Height	= 16;
			_result.Color	= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;

			_obstacleMovedTime		= TimeSpan.Zero;
			_testDriverBreakingTime	= TimeSpan.Zero;
			_measuring				= false;
			_startBreaking			= false;

			_writer = new StreamWriter("results.out");

			_state = PluginState.Activated;

			return true;
		}

		public bool Deactivate()
		{
			if (_state == PluginState.Deactivated) {
				return true;
			}

			if (_result!=null) {
				_result.Delete();
				_result = null;
			}
			
			if (_obstacle!=null) {
				_obstacle.StateChanged -= new EventHandler<ObstacleStateEventArgs>(_obstacle_StateChanged);
				_obstacle.Dispose();
				_obstacle = null;
			}

			if (_testDriver!=null) {
				_testDriver.Breaking -= new EventHandler<BreakEventArgs>(_testDriver_DriverBreaking);

				_testDriver.Dispose();
				_testDriver = null;
			}

			_state = PluginState.Deactivated;

			return true;
		}

		private void _testDriver_DriverBreaking(object sender, BreakEventArgs e)
		{
			if (_measuring) {
				if (!_startBreaking && e.Breaking) {
					_startBreaking	= true;
					_testDriverBreakingTime = new TimeSpan(DateTime.Now.Ticks); 
				} else {
					if (_startBreaking) {
						TimeSpan diff = _testDriverBreakingTime - _obstacleMovedTime;
						_result.Text = String.Format("{0}:{1}:{2}.{3} ", diff.Hours, diff.Minutes, diff.Seconds, diff.Milliseconds);
						_writer.WriteLine(String.Format("{0}	- {1}:{2}:{3}.{4} ", _testDriverBreakingTime, diff.Hours, diff.Minutes, diff.Seconds, diff.Milliseconds));
						_writer.Flush();
						_result.Show();
						if (_obstacle!=null) {
							_obstacle.TestDriveFinishedBreaking();
						}
						_measuring = false;
					}	
				}
			}
		}

		void _obstacle_StateChanged(object sender, ObstacleStateEventArgs e)
		{
			switch (e.NewState) {
				case ObstacleState.Moved : {
					_obstacleMovedTime = new TimeSpan(DateTime.Now.Ticks);
					_measuring		= true;
					_startBreaking	= false;
				} break;

				case ObstacleState.NextRun : {
					_result.Hide();
					_testDriverBreakingTime = TimeSpan.Zero;
					_obstacleMovedTime		= TimeSpan.Zero;
					_measuring				= false;
					_startBreaking			= false;
				} break;
			}
		}

		#endregion
	}
}
