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

using SR.CML.Common;
using SR.CML.Core.Plugins;
using SR.CML.Core.Plugins.Exceptions;
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class Group : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(Group));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool					_disposed			= false;
		private bool					_driversCanDrive	= false;
		private List<ILabel>			_labels				= null;
		private Int32					_realDriversCount	= 0;
		private Int32					_aiDriversCount		= 0;
		private ICarDriverManager		_carDriverManager	= null;
		private IControlFactory			_controlFactory		= null;
		private ICMLCore				_cmlCore			= null;
		private IRaceDirector			_raceDirector		= null;
		private Byte					_maxDriversInGroup	= 0;


		private List<DriverInfo> _drivers = null;
		internal IList<DriverInfo> DriversInfo
		{
			get { return _drivers.AsReadOnly(); }
		}

		private static Byte _space = 1;
		internal static Byte Space
		{
			get { return _space; }
			set { _space = value; }
		}

		private static Byte _ItemWidth = 36;
		internal static Byte ItemWidth
		{
			get { return _ItemWidth; }
			set { _ItemWidth = value; }
		}

		private static Byte _itemHeight = 5;
		internal static Byte ItemHeight
		{
			get { return _itemHeight; }
			set { _itemHeight = value; }
		}

		private Byte _height = 0;
		internal Byte Height
		{
			get { return _height; }
			set { _height = value; }
		}

		private String _name = String.Empty;
		internal String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private Byte _left;
		internal Byte Left
		{
			get { return _left; }
			set { _left = value; }
		}

		private Byte _top;
		internal Byte Top
		{
			get { return _top; }
			set { _top = value; }
		}

		private bool _groupIsActive = false;
		internal bool IsActive
		{
			get { return _groupIsActive; }
			set {
				if (_groupIsActive == value) {
					return;
				}

				_driversCanDrive	= value;
				_groupIsActive		= value;

				if (value) {
					_aiDriversCount = 0;
				}

				if (_labels!=null) {
					if (_groupIsActive) {
						_labels[0].Text = "^2" + _name;
					} else {
						_labels[0].Text = _name;
					}
				}

				foreach (DriverInfo driverInfo in _drivers) {
					driverInfo.Active = _groupIsActive;
				}

				UpdateDriversNames();
			}
		}

		internal bool CanBeStarted
		{
			get { return _aiDriversCount + _realDriversCount >= _maxDriversInGroup; }
		}

		internal Int32 RequestedAiDriversCount
		{
			get { return _maxDriversInGroup - (_aiDriversCount + _realDriversCount); }
		}

		internal void RaceRestartting()
		{
			_aiDriversCount = 0;
		}

		internal Group(IPluginManager pluginManager, Byte maxDriversInGroup)
		{
			Debug.Assert(pluginManager!=null);
			if (pluginManager==null) {
				_log.Fatal("Plugin manager is null!");
				throw new ArgumentNullException("Plugin manager is null!");
			}

			_maxDriversInGroup = maxDriversInGroup;
			GetRequestedPlugins(pluginManager);

			_name				= String.Empty;
			_left				= 0;
			_top				= 0;
			_realDriversCount	= 0;
			_drivers			= new List<DriverInfo>();
			_labels				= null;

			BindEvents();
		}

		~Group()
		{
			Debug.Assert(_disposed, "Group, call dispose before the application is closed!");
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

					if(_labels!=null) {
						foreach(ILabel label in _labels) {
							label.Delete();
						}
						_labels.Clear();
						_labels = null;
					}

					List<DriverInfo> emptyDrivers = new List<DriverInfo>(_maxDriversInGroup);
					foreach (DriverInfo driverInfo in _drivers) {
						if (driverInfo.IsEmpty) {
							emptyDrivers.Add(driverInfo);
						}
					}

					foreach (DriverInfo emptyDriverInfo in emptyDrivers) {
						_drivers.Remove(emptyDriverInfo);
						emptyDriverInfo.Dispose();
					}

				}
				_disposed = true;
			}
		}

		internal void AddDriver(DriverInfo driverInfo)
		{
			Debug.Assert(driverInfo!=null);
			if (driverInfo!=null) {
				if (GetDriverInfo(driverInfo.LfsUserName)!=null) {
					Debug.Assert(false);
					if (_logDebug) {
						_log.DebugFormat("Driver '{0}' is already added in Group", driverInfo.LfsUserName);
					}
					return;
				}
			}

			_drivers.Add(driverInfo);
			_realDriversCount = _drivers.Count;
		}

		internal void Show()
		{
			Debug.Assert(_labels!=null);
			if (_labels==null) {
				_log.Fatal("Items (labels) are null");
				return;
			}

			foreach(ILabel label in _labels) {
				label.Show();
			}
		}

		internal void Hide()
		{
			Debug.Assert(_labels!=null);
			if (_labels==null) {
				_log.Fatal("Items (labels) are null");
				return;
			}

			foreach(ILabel label in _labels) {
				label.Hide();
			}
		}

		internal void SwapDriverBySide()
		{
			DriverInfo tmpDriverInfo = null;

			// First row
			tmpDriverInfo	= _drivers[0];
			_drivers[0]		= _drivers[2];
			_drivers[2]		= tmpDriverInfo;

			// Second row
			tmpDriverInfo	= _drivers[3];
			_drivers[3]		= _drivers[5];
			_drivers[5]		= tmpDriverInfo;

			UpdateDriversNames();
		}

		internal void SwapDriversByRows()
		{
			DriverInfo tmpDriverInfo = null;

			// First
			tmpDriverInfo	= _drivers[0];
			_drivers[0]		= _drivers[3];
			_drivers[3]		= tmpDriverInfo;

			// Second
			tmpDriverInfo	= _drivers[1];
			_drivers[1]		= _drivers[4];
			_drivers[4]		= tmpDriverInfo;

			// Third
			tmpDriverInfo	= _drivers[2];
			_drivers[2]		= _drivers[5];
			_drivers[5]		= tmpDriverInfo;

			UpdateDriversNames();
		}

		internal void SetGrid()
		{
			String[]	driversOrder	= new String[_maxDriversInGroup];
			Int32		index			= 0;

			foreach(DriverInfo driverInfo in _drivers) {
				driversOrder[index++] = driverInfo.LfsUserName;
			}

			_raceDirector.SetGrid(driversOrder, true);
			_driversCanDrive = false;
		}

		internal void CreateLabels()
		{
			FulfilTheGrid();

			_labels = new List<ILabel>(_maxDriversInGroup + 1);
			_height = 0;
			ILabel		label		= null;
			DriverInfo	driverInfo	= null;
			Byte		top			= _top;

			// Group Name
			label				= _controlFactory.CreateLabel();
			label.Left			= _left;
			label.Top			= top;
			label.Width			= _ItemWidth;
			label.Height		= (Byte) (_itemHeight + (_itemHeight / 1.3));
			label.Text			= _name;
			label.Color			= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;
			label.TextAlignment	= FullMotion.LiveForSpeed.InSim.Enums.ButtonTextAlignment.Center;

			top		= (Byte) (top + label.Height + (4 * _space));
			_height	= (Byte) (label.Height + (4 * _space));

			_labels.Add(label);

			Debug.Assert(_drivers.Count == _maxDriversInGroup);

			// Drivers
			for (Byte i=0; i<_maxDriversInGroup; ++i) {
				label				= _controlFactory.CreateLabel();
				label.Left			= _left;
				label.Top			= top;
				label.Width			= _ItemWidth;
				label.Height		= _itemHeight;
				label.Color			= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;
				label.TextAlignment	= FullMotion.LiveForSpeed.InSim.Enums.ButtonTextAlignment.Left;

				if (_drivers.Count>i) {
					driverInfo = _drivers[i];
					if (driverInfo.Driver!=null) {
						label.Text = _drivers[i].Driver.ColorizedNickName;
					} else {
						Debug.Assert(driverInfo.IsEmpty);
						label.Text = driverInfo.LfsUserName;
					}
				}

				top		= (Byte) (top + label.Height + _space);
				_height	= (Byte) (_height + label.Height + _space);
				_labels.Add(label);
			}
		}

		internal void SentAllDriversToSpect()
		{
			foreach (DriverInfo driverInfo in _drivers) {
					driverInfo.Active = _groupIsActive;
					if (_groupIsActive==false && driverInfo.Driver!=null) {
						_cmlCore.SendCommand(LfsCommands.Spectate(driverInfo.Driver));
					}
				}
		}

		private void GetRequestedPlugins(IPluginManager pluginManager)
		{
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

			_cmlCore = pluginManager.GetPlugin(CmlPlugins.CMLCoreGuid) as ICMLCore;
			if (_cmlCore == null) {
				_log.Fatal("CML Core wasn't found!");
				throw new PluginActivateException("CML Core wasn't found");
			}

			_raceDirector = pluginManager.GetPlugin(CmlPlugins.RaceDirectorGuid) as IRaceDirector;
			if (_raceDirector == null) {
				_log.Fatal("Race director wasn't found!");
				throw new PluginActivateException("Race director wasn't found");
			}
		}

		private DriverInfo GetDriverInfo(String lfsName)
		{
			foreach (DriverInfo driverTmp in _drivers) {
				if (driverTmp.LfsUserName == lfsName) {
					return driverTmp;
				}
			}
			return null;
		}

		private void BindEvents()
		{
			_carDriverManager.CarStateChanged		+= new EventHandler<CarStateChangedEventArgs>(_carDriverManager_CarStateChanged);
			_carDriverManager.DriverStateChanged	+= new EventHandler<DriverStateEventArgs>(_carDriverManager_DriverStateChanged);
		}

		private void UnbindEvents()
		{
			_carDriverManager.CarStateChanged		-= new EventHandler<CarStateChangedEventArgs>(_carDriverManager_CarStateChanged);
			_carDriverManager.DriverStateChanged	-= new EventHandler<DriverStateEventArgs>(_carDriverManager_DriverStateChanged);
		}

		private void FulfilTheGrid()
		{
			Int32 addCount = _maxDriversInGroup - _drivers.Count;
			while (addCount>0) {
				_drivers.Add(DriverInfo.Empty);
				--addCount;
			}
		}

		private String GetDriverNamePrefix(CarState carState)
		{
			switch (carState) {
				case CarState.OnTrack:		return "^2* ";	// break;
				case CarState.InGarage:		return "^3* ";	// break;
				case CarState.LeaveTrack:	return "^1* ";	// break;
			}
			return "- ";
		}

		private void UpdateDriversNames()
		{
			ILabel			label	= null;
			IInSimDriver	driver	= null;
			String			driverName;
			CarState		carState;

			for (Byte i=0; i<_maxDriversInGroup; ++i) {

				driverName	= _drivers[i].ColorizedNickName;
				carState	= CarState.Undefined;
				label		= _labels[i+1];	// First is group name
				driver		= _drivers[i].Driver;

				if (driver!=null) {
					Debug.Assert(driver.ColorizedNickName == _drivers[i].ColorizedNickName);
					carState = driver.Car.State;
				}

				if (_groupIsActive && driverName!=String.Empty) {
					label.Text = String.Format("{0} {1}", GetDriverNamePrefix(carState), driverName);
				} else {
					label.Text = driverName;
				}
			}
		}

		internal void CarDriverManager_AiDriverStateChanged(object sender, AiDriverStateChangedEventArgs e)
		{
			_aiDriversCount = e.AiDriversCount;
		}

		private void _carDriverManager_CarStateChanged(object sender, CarStateChangedEventArgs e)
		{
			UpdateDriversNames();
			IInSimDriver driver = e.Car.ActiveDriver;

			if (driver!=null && !_driversCanDrive && e.NewState == CarState.OnTrack) {
				_cmlCore.SendCommand(LfsCommands.Spectate(driver));
			}
		}

		private void _carDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e)
		{
			switch (e.NewState) {
				case DriverState.Disconnecting : {
					DriverInfo driverInfo = GetDriverInfo(e.Driver.LfsName);
					if (driverInfo!=null) {
						driverInfo.RemoveInSimDriver();
					}
					UpdateDriversNames();
				} break;

				case DriverState.Connected : {
					DriverInfo driverInfo = GetDriverInfo(e.Driver.LfsName);
					if (driverInfo!=null) {
						driverInfo.SetInSimDriver(e.Driver);
					}
					UpdateDriversNames();
				} break;
			}
		}

	}
}
