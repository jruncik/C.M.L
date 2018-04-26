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
using System.Collections.Generic;

using SR.CML.Core.Plugins;
using SR.CML.Core.Plugins.Exceptions;

using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using log4net;

using System.Diagnostics;

namespace SR.CML.CzechLeague
{
	internal class PlayerList : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(PlayerList));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool				_disposed			= false;
		private List<IButton>		_buttons			= new List<IButton>();
		private ILabel				_background			= null;
		private ILabel				_logo				= null;
		private ICarDriverManager	_carDriverManager	= null;
		private IControlFactory		_controlFactory		= null;
		private bool				_displayed			= false;

		internal event EventHandler<PlayerListEventArgs> GettingButtonText;
		internal event EventHandler<PlayerListButtonClickEventArgs> Click;

		private IList<DriverInfo> _drivers = new List<DriverInfo>(0);
		internal IList<DriverInfo> Drivers {
			set { _drivers = value; }
		}

		private bool _displayAllPalyers = true;

		private Byte _left = 20;
		internal Byte Left {
			get { return _left; }
			set { _left = value; }
		}

		private Byte _top = 20;
		internal Byte Top {
			get { return _top; }
			set { _top = value; }
		}

		private Byte _ItemWidth = 36;
		internal Byte ItemWidth {
			get { return _ItemWidth; }
			set { _ItemWidth = value; }
		}

		private Byte _space			= 1;
		private Byte _itemHeight	= 4;

		internal PlayerList(IPluginManager pluginManager, bool displayAllPlayers)
		{
			LogDebug("Creating PlayerList");
			Debug.Assert(pluginManager!=null);
			if (pluginManager==null) {
				_log.Fatal("Plugin manager is null!");
				throw new ArgumentNullException("Plugin manager is null!");
			}
			_displayAllPalyers = displayAllPlayers;
			GetRequestedPlugins(pluginManager);
			 BindEvents();
		}

		~PlayerList()
		{
			Debug.Assert(_disposed, "PlayerList, call dispose before the application is closed!");
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
					UnbindEvents();
					DisposeButtons();
				}
				_disposed = true;
			}
		}

		private void DisposeButtons() {
			LogDebug("Disposing Buttons");
			if (_buttons != null) {
				foreach (IButton button in _buttons) {
					button.Click -= new EventHandler<ButtonClickEventArgs>(button_Click);
					button.Delete();
				}
				_buttons.Clear();
				_buttons = null;
			}

			DisposeFrame();
		}

		private void DisposeFrame() {
			LogDebug("Disposing Frame");
			if (_logo!=null) {
				_logo.Delete();
				_logo = null;
			}

			if (_background!=null) {
				_background.Delete();
				_background = null;
			}
		}

		private void LogDebug(String message) {
			if (_logDebug == false) {
				return;
			}

			_log.Debug(message);
		}

		private void LogDebugFormat(String message, params Object[] args) {
			if (_logDebug == false) {
				return;
			}

			_log.DebugFormat(message, args);
		}

		private void GetRequestedPlugins(IPluginManager pluginManager) {
			_carDriverManager = pluginManager.GetPlugin(CmlPlugins.CarDriverManagerGuid) as ICarDriverManager;
			if (_carDriverManager == null) {
				_log.Fatal("Car and Driver Manager wasn't found!");
				throw new PluginActivateException("Car and Driver Manager wasn't found");
			}

			_controlFactory = pluginManager.GetPlugin(CmlPlugins.ControlFactoryGuid) as IControlFactory;
			if (_controlFactory == null) {
				_log.Fatal("Control factory wasn't found!");
				throw new PluginActivateException("Control factory wasn't found");
			}
		}

		private void BindEvents() {
			_carDriverManager.DriverStateChanged += new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
		}

		private void UnbindEvents() {
			_carDriverManager.DriverStateChanged -= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
		}

		private void CarDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e) {
			if (e.OldState == DriverState.Undefined && e.NewState == DriverState.Connected) {
				Refresh();
			}

			if (e.NewState == DriverState.Disconnecting) {
				Refresh();
			}
		}

		private void CreateFrame(Byte width, Byte height) {
			DisposeFrame();
			LogDebug("Creating frame");
			_background			= _controlFactory.CreateLabel();
			_background.Left	= (Byte)(_left - 2);
			_background.Top		= (Byte)(_top - 2);
			_background.Width	= (Byte)(width + 4);
			_background.Height	= height;
			_background.Color	= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;

			_logo			= _controlFactory.CreateLabel();
			_logo.Left		= (Byte)(_left + width - 36);
			_logo.Top		= (Byte)(_top + height - 3 - 4);
			_logo.Width		= 36;
			_logo.Height	= 4;
			_logo.Text		= "^9Powered by ^7EQWorry^9 + ^2Es^3car^7got^9 © (2010)";
			_logo.Color		= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Transparent;
		}

		private void CreateButtons() {
			LogDebug("Creating buttons...");
			DisposeButtons();
			_buttons = new List<IButton>(_drivers.Count);

			Byte height = 0;
			if (_drivers.Count>0) {
				height = (Byte)((160 /_drivers.Count) - (_drivers.Count-1) * _space);
				height = Math.Min(_itemHeight, height);
			}
			Byte top = _top;

			foreach (DriverInfo driverInfo in _drivers) {
				if (!_displayAllPalyers && !driverInfo.CanParticipate) {
					continue;
				}
				IButton button = _controlFactory.CreateButton();
				button.Left				= _left;
				button.Top				= top;
				button.Width			= _ItemWidth;
				button.Height			= _itemHeight;
				button.Color			= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;
				button.TextAlignment	= FullMotion.LiveForSpeed.InSim.Enums.ButtonTextAlignment.Left;
				button.Text				= GetPlayerButtonText(driverInfo);
				button.Tag				= driverInfo.LfsUserName;
				button.Click += new EventHandler<ButtonClickEventArgs>(button_Click);

				top = (Byte)(top + button.Height + _space);
				_buttons.Add(button);
				LogDebugFormat("Button.Text '{0}'", button.Text);
			}

			height = (Byte)Math.Max(6, height + 3 * _space + (top - _top));
			LogDebug("Buttons created.");
			CreateFrame(_ItemWidth, height);
		}

		private String GetPlayerButtonText(DriverInfo driverInfo) {
			String text = driverInfo.ColorizedNickName;
			if (GettingButtonText!=null) {
				PlayerListEventArgs args = new PlayerListEventArgs(driverInfo, text);
				GettingButtonText(this, args);
				text = args.Text;
			}
			return text;
		}

		internal void Show() {
			lock (this) {
				_displayed = true;
				CreateButtons();

				_background.Show();
				_logo.Show();
				foreach (IButton button in _buttons) {
					button.Show();
				}
			}
		}

		internal void Hide() {
			lock (this) {
				if (!_displayed) {
					return;
				}
				_displayed = false;
				foreach (IButton button in _buttons) {
					button.Hide();
				}
				_logo.Hide();
				_background.Hide();
				DisposeButtons();
			}
		}

		internal void Refresh() {
			if (_displayed == false) {
				return;
			}

			lock (this) {
				Hide();
				Show();
			}
		}

		internal void Update() {
			if (_displayed == false) {
				return;
			}

			lock (this) {
				Int32 index = 0;
				DriverInfo driverInfo = null;
				foreach (IButton button in _buttons) {
					if (index < _drivers.Count) {
						driverInfo = _drivers[index];
						button.Text = GetPlayerButtonText(driverInfo);
						button.Tag = driverInfo.LfsUserName;
					} else {
						Debug.Assert(false);
						_log.Fatal("Update: Different count of drivers and buttons!");
					}
					++index;
				}
			}
		}

		private DriverInfo GetDriverInfo(String lfsUserName) {
			foreach (DriverInfo driverInfo in _drivers) {
				if (driverInfo.LfsUserName==lfsUserName) {
					return driverInfo;
				}
			}
			_log.FatalFormat("Driver '{0}' wasn't found", lfsUserName);
			throw new ArgumentNullException(String.Format("Driver '{0}' wasn't found", lfsUserName));
		}

		private void button_Click(object sender, ButtonClickEventArgs e) {

			if (Click==null) {
				return;
			}

			IButton button = sender as IButton;
			if (button==null) {
				Debug.Assert(false, "Clicked on non button control?");
				_log.Fatal("Clicked on non button control");
				return;
			}
			if (e.InSimDriver != null) {
				Click(button, new PlayerListButtonClickEventArgs(e.InSimDriver));
			}
		}
	}
}
