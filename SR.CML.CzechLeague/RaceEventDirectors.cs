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

using log4net;

using System.Diagnostics;

namespace SR.CML.CzechLeague
{
	internal class RaceEventDirectors : IDisposable
	{
		private static ILog _log = LogManager.GetLogger(typeof(LeagueController));
		private static bool _logDebug = _log.IsDebugEnabled;

		private bool	_disposed			= false;
		private String	_qualifyButtonText	= "^3Initialize";
		private String	_raceButtonText		= "^3Initialize";

		private LeagueController _leagueController;

		private Dictionary<String, DriverInfo> _directors = new Dictionary<String, DriverInfo>();

		private Dictionary<String, DirectorMenu> _menu = new Dictionary<String,DirectorMenu>();

		internal RaceEventDirectors(LeagueController leagueController)
		{
			_leagueController = leagueController;
			CreateDirectores();
			BindEvents();
		}

		~RaceEventDirectors()
		{
			Debug.Assert(_disposed, "Directors, call dispose before the application is closed!");
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
					DisposeDirectors();

					if (_menu!=null) {
						foreach(DirectorMenu menu in _menu.Values) {
							menu.Dispose();
						}
						_menu.Clear();
						_menu = null;
					}
				}
				_disposed = true;
			}
		}

		private void BindEvents() {
			_leagueController.CarDriverManager.DriverStateChanged += new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
		}

		private void UnbindEvents() {
			_leagueController.CarDriverManager.DriverStateChanged -= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
		}

		private void CreateDirectores() {
			DriverInfo driverInfo = null;

			foreach (String adminName in _leagueController.ServerSetting.Config.CzLeagueConfig.Admins) {
				driverInfo = new DriverInfo(adminName);
				AddDirector(driverInfo);
			}
		}

		private void AddDirector(DriverInfo driverInfo) {
			_directors.Add(driverInfo.LfsUserName, driverInfo);
			_menu.Add(driverInfo.LfsUserName, new DirectorMenu(driverInfo, _leagueController));
		}

		private void DisposeDirectors() {
			if (_directors != null) {
				foreach (DriverInfo driverInfo in _directors.Values) {
					if (driverInfo != null) {
						driverInfo.Dispose();
					}
				}
				_directors.Clear();
				_directors = null;
			}
		}

		internal bool IsRaceDirector(String lfsName) {
			return _directors.ContainsKey(lfsName);
		}

		internal DriverInfo GetDirectorDriverInfo(String lfsName) {
			if (!_directors.ContainsKey(lfsName)) {
				return null;
			}
			return _directors[lfsName];
		}

		private void CarDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e) {
			if (!IsRaceDirector(e.Driver.LfsName)) {
				return;
			}

			DriverInfo		driverInfo	= GetDirectorDriverInfo(e.Driver.LfsName);
			DirectorMenu	menu		= null;

			if (_menu.ContainsKey(e.Driver.LfsName)) {
				menu = _menu[e.Driver.LfsName];
			} else {
				_menu.Add(driverInfo.LfsUserName, new DirectorMenu(driverInfo, _leagueController));
				menu = _menu[e.Driver.LfsName];
			}
			Debug.Assert(menu != null);
			if (menu == null) {
				return;
			}

			if (e.OldState == DriverState.Undefined && e.NewState == DriverState.Connected) {
				DriverConnected(e, driverInfo);
				menu.SetQualifyButtonText(_qualifyButtonText);
				menu.SetRaceButtonText(_raceButtonText);

				menu.ShowMenu();
				return;
			}

			if (e.NewState == DriverState.Disconnecting) {
				DriverDisconnected(e, driverInfo);
				_menu.Remove(e.Driver.LfsName);
				menu.HideMenu();
				menu.Dispose();
				return;
			}

			if (e.NewState == DriverState.Spectating) {
				menu.ShowMenu();
				return;
			}

			menu.HideMenu();
		}

		private void DriverConnected(DriverStateEventArgs e, DriverInfo driverInfo) {
			if (_directors.ContainsKey(e.Driver.LfsName)) {
				Debug.Assert(_directors[e.Driver.LfsName] != null);
				_directors[e.Driver.LfsName].SetInSimDriver(e.Driver);
			}
		}

		private void DriverDisconnected(DriverStateEventArgs e, DriverInfo driverInfo) {
			if (_directors.ContainsKey(e.Driver.LfsName)) {
				if (_directors[e.Driver.LfsName] != null) {
					_directors[e.Driver.LfsName].RemoveInSimDriver();
				}
			}
		}

		internal void SetQualifyButtonText(String qualifyButtonText) {
			_qualifyButtonText = qualifyButtonText;
			foreach (DirectorMenu menu in _menu.Values) {
				menu.SetQualifyButtonText(qualifyButtonText);
			}
		}

		internal void SetRaceButtonText(String raceButtonText) {
			_raceButtonText = raceButtonText;
			foreach (DirectorMenu menu in _menu.Values) {
				menu.SetRaceButtonText(raceButtonText);
			}
		}

		internal void ShowGridOrderSubMenus() {
			foreach (DirectorMenu menu in _menu.Values) {
				menu.ShowGridOrderSubMenu();
			}
		}

		internal void HideGridOrderSubMenus() {
			foreach (DirectorMenu menu in _menu.Values) {
				menu.HideGridOrderSubMenu();
			}
		}

		internal void SetSelectedPlayerName(DriverInfo selectedDriver) {
			foreach (DirectorMenu menu in _menu.Values) {
				menu.SetSelectedPlayerName(selectedDriver);
			}
		}

		internal void SetInsertButtonState(bool after, bool activated) {
			foreach (DirectorMenu menu in _menu.Values) {
				menu.SetInsertButtonState(after, activated);
			}
		}
	}
}
