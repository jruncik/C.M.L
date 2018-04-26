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
	internal class SelectDriversForRace : IDisposable
	{
		private static ILog			_log		= LogManager.GetLogger(typeof(SelectDriversForRace));
		private static bool			_logDebug	= _log.IsDebugEnabled;
		private bool				_disposed	= false;
		private PlayerList			_playerList	= null;
		private LeagueController	_leagueController;

		private bool _displayed = false;
		public bool Displayed {
			get { return _displayed; }
			set { _displayed = value; }
		}

		internal SelectDriversForRace(LeagueController leagueDirector)
		{
			if (leagueDirector == null) {
				_log.Fatal("League Director is null!");
				throw new ArgumentNullException("League Director is null!");
			}

			_leagueController = leagueDirector;
			_playerList = new PlayerList(_leagueController.PluginManager, true);
			_playerList.Drivers = _leagueController.Drivers;

			BindEvents();
		}

		private void BindEvents() {
			_playerList.GettingButtonText							+= new EventHandler<PlayerListEventArgs>(_playerList_GettingButtonText);
			_playerList.Click										+= new EventHandler<PlayerListButtonClickEventArgs>(_playerList_Click);
			_leagueController.CarDriverManager.DriverStateChanged	+= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
		}

		private void UnbindEvents() {
			_playerList.GettingButtonText							-= new EventHandler<PlayerListEventArgs>(_playerList_GettingButtonText);
			_playerList.Click										-= new EventHandler<PlayerListButtonClickEventArgs>(_playerList_Click);
			_leagueController.CarDriverManager.DriverStateChanged	-= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
		}

		~SelectDriversForRace()
		{
			Debug.Assert(_disposed, "SelectDriversForRace, call dispose before the application is closed!");
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
					if (_playerList!=null) {
						_playerList.Hide();
						_playerList.Dispose();
						_playerList = null;
					}
				}
				_disposed = true;
			}
		}

		internal void Show() {
			_displayed = true;
			_playerList.Show();
		}

		internal void Hide() {
			_displayed = false;
			_playerList.Hide();
		}

		private void Refresh() {
			_playerList.Refresh();
		}

		void CarDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e) {
			if (e.OldState == DriverState.Undefined && e.NewState == DriverState.Connected) {
				Refresh();
				return;
			}

			if (e.NewState == DriverState.Disconnecting) {
				Refresh();
				return;
			}
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

			driverInfo.CanParticipate = ! driverInfo.CanParticipate;

			if (!driverInfo.CanParticipate && driverInfo.Driver!=null) {
				_leagueController.CmlCore.SendCommand(LfsCommands.Spectate(driverInfo.Driver));
			}
			button.Text = GetButtonText(driverInfo);
		}

		private void _playerList_GettingButtonText(object sender, PlayerListEventArgs e) {
			if (e.DriverInfo == null) {
				e.Text = String.Empty;
				return;
			}
			e.Text = GetButtonText(e.DriverInfo);
		}

		private static String GetButtonText(DriverInfo driverInfo) {
			if (driverInfo.CanParticipate) {
				return "^2" + GetPlayerNameWithoutColors(driverInfo.ColorizedNickName);
			}
			return "^1" + GetPlayerNameWithoutColors(driverInfo.ColorizedNickName);
		}

		internal static String GetPlayerNameWithoutColors(String playerName) {
			if (String.IsNullOrEmpty(playerName)) {
				return String.Empty;
			}

			StringBuilder plainPlayerName = new StringBuilder(playerName.Length);
			Int32 i = 0;

			while (i < playerName.Length) {
				if (playerName[i] == '^') {
					++i;
					if (i < playerName.Length && !Char.IsNumber(playerName[i])) {
						plainPlayerName.Append('^');
						continue;
					}
					++i;
					continue;
				}
				plainPlayerName.Append(playerName[i++]);
			}

			return plainPlayerName.ToString();
		}

	}
}
