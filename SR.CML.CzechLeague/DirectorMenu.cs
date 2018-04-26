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
	internal class DirectorMenu : IDisposable
	{
		private static ILog _log = LogManager.GetLogger(typeof(DirectorMenu));
		private static bool _logDebug = _log.IsDebugEnabled;

		private bool				_disposed		= false;
		private IControlFactory		_controlFactory	= null;
		private DriverInfo			_driverInfo		= null;

		private IButton				_buttonQualify	= null;
		private IButton				_buttonRace		= null;

		private IButton				_buttonSelectDrivers	= null;
		private IButton				_buttonBuildGrid		= null;

		private LeagueController	_leagueController;
		private ICarDriverManager	_carDriverManager	= null;
		private String				_qualifyButtonText	= "^3Initialize";
		private String				_raceButtonText		= "^3Initialize";

		private GridBuilderMenu _gridBuildermenu = null;

		private static String _newButtonText = String.Empty;

		private static bool _canBeMenuDisplayed = true;
		internal static bool CanBeMenuDisplayed {
			get { return _canBeMenuDisplayed; }
			set { _canBeMenuDisplayed = value; }
		}

		internal DirectorMenu(DriverInfo driverInfo, LeagueController leagueController) {
			
			Debug.Assert(leagueController!=null);
			if (leagueController==null) {
				_log.Fatal("LeagueController is null!");
				throw new ArgumentNullException("LeagueController Core is null!");
			}
			_leagueController = leagueController;

			Debug.Assert(driverInfo!=null);
			if (driverInfo==null) {
				_log.Fatal("DriverInfo is null!");
				throw new ArgumentNullException("DriverInfo Core is null!");
			}

			GetRequestedPlugins(_leagueController.PluginManager);
			_driverInfo	= driverInfo;
			_gridBuildermenu = new GridBuilderMenu(_leagueController, _driverInfo);
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

		~DirectorMenu() {
			Debug.Assert(_disposed, "DirectorMenu, call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
					DestructMenu();

					if (_gridBuildermenu != null) {
						_gridBuildermenu.Dispose();
						_gridBuildermenu = null;
					}
				}
				_disposed = true;
			}
		}

		private void ConstructMenu() {
			Debug.Assert(_driverInfo.Driver != null);
			if (_driverInfo.Driver == null) {
				_log.Fatal("Director is null!");
				return;
			}

			if (_buttonQualify == null) {
				_buttonQualify = _controlFactory.CreateButton(_driverInfo.Driver);

				_buttonQualify.Left = 30;
				_buttonQualify.Top = 5;
				_buttonQualify.Width = 30;
				_buttonQualify.Height = 10;
				_buttonQualify.Text = _qualifyButtonText;

				_buttonQualify.TextColor = FullMotion.LiveForSpeed.InSim.Enums.ButtonTextColor.Ok;
				_buttonQualify.Color = FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;
				_buttonQualify.Click += new EventHandler<ButtonClickEventArgs>(_buttonQualify_Click);
			}

			if (_buttonRace == null) {
				_buttonRace = _controlFactory.CreateButton(_driverInfo.Driver);

				_buttonRace.Left = 65;
				_buttonRace.Top = 5;
				_buttonRace.Width = 30;
				_buttonRace.Height = 10;
				_buttonRace.Text = _raceButtonText;
				_buttonRace.Click += new EventHandler<ButtonClickEventArgs>(_buttonRace_Click);

				_buttonRace.TextColor = FullMotion.LiveForSpeed.InSim.Enums.ButtonTextColor.Ok;
				_buttonRace.Color = FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;
			}

			if (_buttonSelectDrivers == null) {
				_buttonSelectDrivers = _controlFactory.CreateButton(_driverInfo.Driver);

				_buttonSelectDrivers.Left = 110;
				_buttonSelectDrivers.Top = 5;
				_buttonSelectDrivers.Width = 30;
				_buttonSelectDrivers.Height = 10;
				_buttonSelectDrivers.Text = "^3Enable drivers";

				_buttonSelectDrivers.TextColor = FullMotion.LiveForSpeed.InSim.Enums.ButtonTextColor.Ok;
				_buttonSelectDrivers.Color = FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;
				_buttonSelectDrivers.Click += new EventHandler<ButtonClickEventArgs>(_buttonSelectDrivers_Click);
			}

			if (_buttonBuildGrid == null) {
				_buttonBuildGrid = _controlFactory.CreateButton(_driverInfo.Driver);

				_buttonBuildGrid.Left = 145;
				_buttonBuildGrid.Top = 5;
				_buttonBuildGrid.Width = 30;
				_buttonBuildGrid.Height = 10;
				_buttonBuildGrid.Text = "^3Build grid menu";

				_buttonBuildGrid.TextColor = FullMotion.LiveForSpeed.InSim.Enums.ButtonTextColor.Ok;
				_buttonBuildGrid.Color = FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;
				_buttonBuildGrid.Click += new EventHandler<ButtonClickEventArgs>(_buttonBuildGrid_Click);
			}
		}

		private void DestructMenu() {
			HideMenu();

			if (_buttonQualify != null) {
				_buttonQualify.Click -= new EventHandler<ButtonClickEventArgs>(_buttonQualify_Click);
				_buttonQualify.Delete();
				_buttonQualify = null;
			}

			if (_buttonRace != null) {
				_buttonRace.Click -= new EventHandler<ButtonClickEventArgs>(_buttonRace_Click);
				_buttonRace.Delete();
				_buttonRace = null;
			}

			if (_buttonSelectDrivers != null) {
				_buttonSelectDrivers.Click -= new EventHandler<ButtonClickEventArgs>(_buttonSelectDrivers_Click);
				_buttonSelectDrivers.Delete();
				_buttonSelectDrivers = null;
			}

			if (_buttonBuildGrid != null) {
				_buttonBuildGrid.Click -= new EventHandler<ButtonClickEventArgs>(_buttonBuildGrid_Click);
				_buttonBuildGrid.Delete();
				_buttonBuildGrid = null;
			}
		}

		internal void ShowMenu() {

			if (!MenuExists()) {
				ConstructMenu();
			}

			_canBeMenuDisplayed = true;
			ShowMenuCommonButtons();
			ShowGridOrderSubMenu();
		}

		internal void ShowGridOrderSubMenu() {
			if (_leagueController.GridOrderBuilder.Displayed) {
				_gridBuildermenu.Show();
			}
		}

		private void ShowMenuCommonButtons() {
			if (_buttonQualify != null) {
				_buttonQualify.Show();
				_buttonQualify.Text = _qualifyButtonText;
			}

			if (_buttonRace != null) {
				_buttonRace.Show();
				_buttonRace.Text = _raceButtonText;
			}

			if (_buttonSelectDrivers != null) {
				_buttonSelectDrivers.Show();
			}

			if (_buttonBuildGrid != null) {
				_buttonBuildGrid.Show();
			}
		}

		internal void HideMenu() {
			_canBeMenuDisplayed = false;
			HideMenuCommonButtons();

			HideGridOrderSubMenu();
		}

		internal void HideGridOrderSubMenu() {
			_gridBuildermenu.Hide();
		}

		private void HideMenuCommonButtons() {
			if (_buttonQualify != null) {
				_buttonQualify.Hide();
			}

			if (_buttonRace != null) {
				_buttonRace.Hide();
			}

			if (_buttonSelectDrivers != null) {
				_buttonSelectDrivers.Hide();
			}

			if (_buttonBuildGrid != null) {
				_buttonBuildGrid.Hide();
			}
		}

		private bool MenuExists() {
			if (_buttonBuildGrid == null || _buttonSelectDrivers == null || _buttonQualify == null || _buttonRace == null) {
				Debug.Assert(_buttonBuildGrid == null);
				Debug.Assert(_buttonSelectDrivers == null);
				Debug.Assert(_buttonQualify == null);
				Debug.Assert(_buttonRace == null);

				return false;
			}

			return true;
		}

		internal void SetSelectedPlayerName(DriverInfo selectedPlayer) {
			_gridBuildermenu.SetSelectedPlayerName(selectedPlayer);
		}

		internal void SetInsertButtonState(bool after, bool activated) {
			_gridBuildermenu.SetInsertButtonState(after, activated);
		}

		private void _buttonQualify_Click(object sender, ButtonClickEventArgs e) {
			_leagueController.StartQualify();
		}

		void _buttonRace_Click(object sender, ButtonClickEventArgs e) {
			_leagueController.StartRace();
		}

		private void _buttonBuildGrid_Click(object sender, ButtonClickEventArgs e) {
			if (!_leagueController.GridOrderBuilder.Displayed) {
				_leagueController.HideSelectDrivers();
				_leagueController.ShowGridOrderBuilder();
			} else {
				_leagueController.HideGridOrderBuilder();
			}
		}

		private void _buttonSelectDrivers_Click(object sender, ButtonClickEventArgs e) {
			if (!_leagueController.SelectDrivers.Displayed) {
				_leagueController.HideGridOrderBuilder();
				_leagueController.ShowSelectDrivers();
			} else {
				_leagueController.HideSelectDrivers();
			}
		}

		internal void SetQualifyButtonText(String qualifyButtonText) {
			_qualifyButtonText = qualifyButtonText;
			if (_buttonQualify != null) {
				_buttonQualify.Text = qualifyButtonText;
			}
		}

		internal void SetRaceButtonText(String raceButtonText) {
			_raceButtonText = raceButtonText;
			if (_buttonRace != null) {
				_buttonRace.Text = raceButtonText;
			}
		}

	}
}
