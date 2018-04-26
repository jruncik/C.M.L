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
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class RallycrossDirectorMenu : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(RallycrossDirector));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool				_disposed				= false;
		private IControlFactory		_controlFactory			= null;
		private RallycrossDirector	_director				= null;
		private DriverInfo			_driverInfo				= null;
		private IButton				_button					= null;
		private bool				_showMenu				= false;

		// ToDo Add menu for on-line penalization

		private static String _newButtonText = String.Empty;
		internal static String NewButtonText
		{
			get { return _newButtonText; }
			set { _newButtonText = value; }
		}

		private static bool _canBeButonDisplayed = true;
		internal static bool CanBeButtonDisplayed
		{
			get { return _canBeButonDisplayed; }
			set { _canBeButonDisplayed = value; }
		}

		internal RallycrossDirectorMenu(RallycrossDirector director, DriverInfo driverInfo)
		{
			Debug.Assert(director!=null);
			if (director==null) {
				_log.Fatal("RallycrossDirector is null!");
				throw new ArgumentNullException("RallycrossDirector Core is null!");
			}

			Debug.Assert(driverInfo!=null);
			if (driverInfo==null) {
				_log.Fatal("DriverInfo is null!");
				throw new ArgumentNullException("DriverInfo Core is null!");
			}

			_controlFactory	= director.ControlFactory;
			_director	= director;
			_driverInfo	= driverInfo;

			ConstructMenu();
		}

		~RallycrossDirectorMenu()
		{
			Debug.Assert(_disposed, "RallycrossDirectorMenu, call dispose before the application is closed!");
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
					DestructMenu();
				}
				_disposed = true;
			}
		}

		internal void CarDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e)
		{
			if (e.Driver.LfsName != _driverInfo.LfsUserName) {
				return;
			}

			switch (e.NewState) {
				case DriverState.InCar:
				case DriverState.Disconnecting: {
					_showMenu = false;
					HideMenu();
				} break;

				case DriverState.Connected: {
					_showMenu = true;
					ShowMenu();
				} break;

				case DriverState.Spectating: {
					_showMenu = true;
					ShowMenu();
				} break;

				default : {
					Debug.Assert(false, "Unsupported driver state");
					_showMenu = false;
					HideMenu();
				} break;
			}
		}

		internal void SetControlButtonText(String text)
		{
			_button.Text	= text;
			_newButtonText	= text;
		}

		internal void ControlButtonShow()
		{
			ShowMenu();
		}

		internal void ControlButtonHide()
		{
			HideMenu();
		}


		private void ConstructMenu()
		{
			Debug.Assert(_driverInfo.Driver!=null);
			if (_driverInfo.Driver==null) {
				_log.Fatal("Director is null!");
				return;
			}

			if (_button==null) {
				_button = _controlFactory.CreateButton(_driverInfo.Driver);

				_button.Left	= 85;
				_button.Top		= 5;
				_button.Width	= 30;
				_button.Height	= 10;
				_button.Text	= _newButtonText;

				_button.TextColor	= FullMotion.LiveForSpeed.InSim.Enums.ButtonTextColor.Ok;
				_button.Color		= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;
				_button.Click += new EventHandler<ButtonClickEventArgs>(_button_Click);
			}
		}

		private void DestructMenu()
		{
			if (_button!=null) {
				HideMenu();
				_button.Delete();
				_button = null;
			}
		}

		private void ShowMenu()
		{
			_canBeButonDisplayed = true;
			if (_button!=null && _showMenu) {
				_button.Show();
			}
		}

		private void HideMenu()
		{
			_canBeButonDisplayed = false;
			if (_button!=null) {
				_button.Hide();
			}
		}

		private void _button_Click(object sender, ButtonClickEventArgs e)
		{
			_director.NextStage();
		}

	}
}
