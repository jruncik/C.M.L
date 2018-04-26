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
using System.Text;
using System.Collections.Generic;

using SR.CML.Common;

using SR.CML.Core.Plugins;
using SR.CML.Core.Plugins.Exceptions;

using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using log4net;

using System.Diagnostics;

namespace SR.CML.CzechLeague
{
	internal class GridBuilder : IDisposable
	{
		private static ILog			_log		= LogManager.GetLogger(typeof(GridBuilder));
		private static bool			_logDebug	= _log.IsDebugEnabled;

		private bool				_disposed		= false;
		private PlayerList			_playerList		= null;

		private LeagueController _leagueController;
		internal LeagueController LeagueController {
			get { return _leagueController; }
		}

		private bool _insertBefore;
		internal bool InsertBefore {
			set {
				_insertBefore = value;
				if (_insertBefore) {
					_insertAfter = false;
				}
				HighlightInsertButtons();
			}
		}

		private bool _insertAfter;
		internal bool InsertAfter {
			set {
				_insertAfter = value;
				if (_insertAfter) {
					_insertBefore = false;
				}
				HighlightInsertButtons();
			}
		}

		private void HighlightInsertButtons() {
			if (String.IsNullOrEmpty(_selectedDriverName)) {
				return;
			}
			_leagueController.RaceEventDirectors.SetInsertButtonState(false, _insertBefore);
			_leagueController.RaceEventDirectors.SetInsertButtonState(true, _insertAfter);
		}

		private String _selectedDriverName = String.Empty;

		internal bool doInsert {
			get { return _insertBefore || _insertAfter; }
		}

		private bool _displayed = false;
		public bool Displayed {
			get { return _displayed; }
		}

		internal GridBuilder(LeagueController leagueDirector)
		{
			if (leagueDirector == null) {
				_log.Fatal("League Director is null!");
				throw new ArgumentNullException("League Director is null!");
			}

			_leagueController	= leagueDirector;
			_playerList			= new PlayerList(_leagueController.PluginManager, false);
			_playerList.Drivers	= _leagueController.Drivers;

			BindEvents();
		}

		~GridBuilder()
		{
			Debug.Assert(_disposed, "GridBuilder, call dispose before the application is closed!");
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
					DisposePlayerList();
				}
				_disposed = true;
			}
		}

		private void DisposePlayerList() {
			if (_playerList != null) {
				_playerList.Hide();
				_playerList.Dispose();
				_playerList = null;
			}
		}

		internal void Show() {
			lock (this) {
				_playerList.Show();
				_displayed = true;
				_leagueController.RaceEventDirectors.ShowGridOrderSubMenus();
			}
		}

		internal void Hide() {
			lock (this) {
				_playerList.Hide();
				_leagueController.RaceEventDirectors.HideGridOrderSubMenus();
				_displayed = false;
			}
		}

		internal void Update() {
			_playerList.Update();
		}

		internal void Refresh() {
			_playerList.Refresh();
		}

		private void CarDriverManager_CarStateChanged(object sender, CarStateChangedEventArgs e) {
			Update();
		}

		private void _playerList_Click(object sender, PlayerListButtonClickEventArgs e) {
			if (e.InSimDriver == null) {
				return;
			}

			if (!_leagueController.RaceEventDirectors.IsRaceDirector(e.InSimDriver.LfsName)) {
				return;
			}

			IButton button = sender as IButton;
			if (button==null) {
				return;
			}

			String lfsPlayerName = button.Tag as String;
			if (lfsPlayerName == null) {
				return;
			}

			DriverInfo driverInfo = _leagueController.GetDriverInfo(lfsPlayerName);
			if (driverInfo == null) {
				return;
			}

			if (doInsert && _selectedDriverName!=String.Empty) {
				if (_insertAfter) {
					Debug.Assert(_insertBefore == false);
					InsertAfterDriver(_selectedDriverName, driverInfo.LfsUserName);


				} else if (_insertBefore) {
					Debug.Assert(_insertAfter == false);
					InsertBeforeDriver(_selectedDriverName, driverInfo.LfsUserName);
				}
				InsertAfter		= false;
				InsertBefore	= false;
				return;
			}

			_selectedDriverName = driverInfo.LfsUserName;
			_leagueController.RaceEventDirectors.SetSelectedPlayerName(driverInfo);
		}

		private void _playerList_GettingButtonText(object sender, PlayerListEventArgs e) {
			if (e.DriverInfo == null) {
				e.Text = String.Empty;
				return;
			}

			e.Text = GetButtonText(e.DriverInfo);
		}

		private static String GetButtonText(DriverInfo driverInfo) {

			if (!driverInfo.CanParticipate) {
				return "^1x " + SelectDriversForRace.GetPlayerNameWithoutColors(driverInfo.ColorizedNickName);
			}

			if (driverInfo.Driver!=null && driverInfo.Driver.Car!=null) {
				switch (driverInfo.Driver.Car.State) {
					case CarState.InGarage : {
						return "^3›^7 " + driverInfo.ColorizedNickName;
					} //break;

					case CarState.OnTrack : {
						return "^2•^7 " + driverInfo.ColorizedNickName;
					} //break;
				}
			}

			return "^1‹^7 " + driverInfo.ColorizedNickName;
		}

		private void BindEvents() {
			_playerList.GettingButtonText += new EventHandler<PlayerListEventArgs>(_playerList_GettingButtonText);
			_playerList.Click += new EventHandler<PlayerListButtonClickEventArgs>(_playerList_Click);
			_leagueController.CarDriverManager.CarStateChanged += new EventHandler<CarStateChangedEventArgs>(CarDriverManager_CarStateChanged);
		}

		private void UnbindEvents() {
			_playerList.GettingButtonText -= new EventHandler<PlayerListEventArgs>(_playerList_GettingButtonText);
			_playerList.Click -= new EventHandler<PlayerListButtonClickEventArgs>(_playerList_Click);
			_leagueController.CarDriverManager.CarStateChanged -= new EventHandler<CarStateChangedEventArgs>(CarDriverManager_CarStateChanged);
		}

		internal void MoveDriverUp(String selectedDriver) {
			Int32 selectedDriverIndex = GetDriverIndex(selectedDriver);
			if (selectedDriverIndex < 1) {
				return;
			}
			Int32 previousIndex = selectedDriverIndex - 1;

			DriverInfo previousDriverInfo = _leagueController.Drivers[previousIndex];
			_leagueController.Drivers[previousIndex]		= _leagueController.Drivers[selectedDriverIndex];
			_leagueController.Drivers[selectedDriverIndex]	= previousDriverInfo;

			Update();
		}

		internal void MoveDriverDown(String selectedDriver) {
			Int32 selectedDriverIndex = GetDriverIndex(selectedDriver);
			if (selectedDriverIndex < 0 || selectedDriverIndex >= _leagueController.Drivers.Count-1) {
				return;
			}
			Int32 nextDriverIndex = selectedDriverIndex + 1;

			DriverInfo nextDriverInfo = _leagueController.Drivers[nextDriverIndex];
			_leagueController.Drivers[nextDriverIndex]		= _leagueController.Drivers[selectedDriverIndex];
			_leagueController.Drivers[selectedDriverIndex]	= nextDriverInfo;

			Update();
		}

		internal void InsertBeforeDriver(String selectedDriver, String beforeDriverName) {
			Int32 selectedDriverIndex = GetDriverIndex(selectedDriver);
			if (selectedDriverIndex < 0) {
				return;
			}
			Int32 beforeDriverIndex = GetDriverIndex(beforeDriverName);
			if (beforeDriverIndex < 0) {
				return;
			}

			DriverInfo selectedDriverInfo = _leagueController.Drivers[selectedDriverIndex];
			_leagueController.Drivers.RemoveAt(selectedDriverIndex);
			_leagueController.Drivers.Insert(beforeDriverIndex, selectedDriverInfo);

			Update();
		}

		internal void InsertAfterDriver(String selectedDriver, String afterDriverName) {
			Int32 selectedDriverIndex = GetDriverIndex(selectedDriver);
			if (selectedDriverIndex < 0) {
				return;
			}
			Int32 afterDriverIndex = GetDriverIndex(afterDriverName);
			if (afterDriverIndex < 0) {
				return;
			}
			++afterDriverIndex;

			DriverInfo selectedDriverInfo = _leagueController.Drivers[selectedDriverIndex];
			_leagueController.Drivers.RemoveAt(selectedDriverIndex);
			if (afterDriverIndex < _leagueController.Drivers.Count) {
				_leagueController.Drivers.Insert(afterDriverIndex, selectedDriverInfo);
			} else {
				_leagueController.Drivers.Add(selectedDriverInfo);
			}

			Update();
		}

		internal void BuildGrid() {
			List<String> grid = new List<String>();
			foreach (DriverInfo driver in _leagueController.Drivers) {
				grid.Add(driver.LfsUserName);
			}

			_leagueController.RaceDirector.SetGrid(grid.ToArray(), false);
		}

		private Int32 GetDriverIndex(String driverName) {
			DriverInfo driverInfo = _leagueController.GetDriverInfo(driverName);
			if (driverInfo == null) {
				return -1;
			}

			return _leagueController.Drivers.IndexOf(driverInfo);
		}
	}
}
