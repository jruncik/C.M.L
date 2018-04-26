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

using FullMotion.LiveForSpeed.InSim.Enums;

using System.Diagnostics;

namespace SR.CML.CzechLeague
{
	internal class GridBuilderMenu : IDisposable
	{
		private static ILog			_log			= LogManager.GetLogger(typeof(GridBuilderMenu));
		private static bool			_logDebug		= _log.IsDebugEnabled;

		private static String		_textButtonColorEnabled = "^7";
		private bool				_disposed			= false;
		private LeagueController	_leagueController	= null;
		private DriverInfo			_driverInfo			= null;
		private IControlFactory		_controlFactory		= null;

		private ILabel	_labelSelectedDriver	= null;
		private ILabel	_labelBackground		= null;
		private IButton	_up				= null;
		private IButton	_down			= null;
		private IButton	_insertBefore	= null;
		private IButton	_insertAfter	= null;
		private IButton _sendGrid		= null;
		private IButton _swap			= null;
		private IButton _getGridFromRes	= null;

		private bool _displayed = false;
		public bool Displayed {
			get { return _displayed; }
		}

		internal GridBuilderMenu(LeagueController leagueController, DriverInfo driverInfo) {
			Debug.Assert(leagueController != null);
			_leagueController = leagueController;
			_driverInfo		= driverInfo;
			GetRequestedPlugins(_leagueController.PluginManager);
		}

		~GridBuilderMenu() {
			Debug.Assert(_disposed, "GridBuilderMenu, call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
					DestroyMenu();
				}
				_disposed = true;
			}
		}

		internal void Show() {
			if (_driverInfo.Driver == null) {
				return;
			}

			if (_driverInfo.Driver.State == DriverState.InCar) {
				return;
			}

			if (!isMenuCreated()) {
				CreateMenu();
				if (!isMenuCreated()) {
					return;
				}
			}

			_labelBackground.Show();
			_labelSelectedDriver.Show();
			_up.Show();
			_down.Show();
			_insertBefore.Show();
			_insertAfter.Show();
			_sendGrid.Show();
			_swap.Show();
			_getGridFromRes.Show();

			_displayed = true;
		}

		internal void Hide() {

			if (!_displayed || !isMenuCreated() ) {
				return;
			}

			_labelBackground.Hide();
			_labelSelectedDriver.Hide();
			_up.Hide();
			_down.Hide();
			_insertBefore.Hide();
			_insertAfter.Hide();
			_sendGrid.Hide();
			_swap.Hide();
			_getGridFromRes.Hide();

			_displayed = false;
		}

		private void GetRequestedPlugins(IPluginManager pluginManager) {
			_controlFactory = pluginManager.GetPlugin(CmlPlugins.ControlFactoryGuid) as IControlFactory;
			if (_controlFactory == null) {
				_log.Fatal("Control factory wasn't found!");
				throw new PluginActivateException("Control factory wasn't found");
			}
		}

		internal void SetInsertButtonState(bool after, bool activated) {
			IButton button = null;
			if (after) {
				button = _insertAfter;
			} else {
				button = _insertBefore;
			}

			if (button == null) {
				return;
			}

			String text = SelectDriversForRace.GetPlayerNameWithoutColors(button.Text);
			if (activated) {
				text = "^3" + text;
			} else {
				text = _textButtonColorEnabled + text;
			}
			button.Text = text;
		}

		private void CreateMenu()
		{
			if (_driverInfo.Driver == null) {
				return;
			}

			Byte leftOrigin			= 100;
			Byte left				= leftOrigin;
			Byte initialTop			= 40;
			Byte top				= 0;
			Byte height				= 7;
			Byte space				= 1;
			Byte width				= 40;
			Byte firstButtonWidth	= 14;
			ButtonColor buttonColor = ButtonColor.Light;

			top = initialTop;
			if (_labelSelectedDriver == null) {
				_labelSelectedDriver = _controlFactory.CreateLabel(_driverInfo.Driver);
				_labelSelectedDriver.Left	= left;
				_labelSelectedDriver.Top	= top;
				_labelSelectedDriver.Width	= width;
				_labelSelectedDriver.Height	= height;
				_labelSelectedDriver.Color = buttonColor;
				_labelSelectedDriver.Text	= String.Empty;
			}

			top += (Byte)(height + 6 * space);
			if (_up == null) {
				_up			= _controlFactory.CreateButton(_driverInfo.Driver);
				_up.Left	= left;
				_up.Top		= top;
				_up.Width	= firstButtonWidth;
				_up.Height	= height;
				_up.Color	= buttonColor;
				_up.Text = _textButtonColorEnabled + "Up";
			}

			top += (Byte)(height + space);
			if (_down == null) {
				_down = _controlFactory.CreateButton(_driverInfo.Driver);
				_down.Left	= left;
				_down.Top	= top;
				_down.Width	= firstButtonWidth;
				_down.Height= height;
				_down.Color	= buttonColor;
				_down.Text	= _textButtonColorEnabled + "Down";
			}

			width -= (Byte)(firstButtonWidth + space);
			left += (Byte)(firstButtonWidth + space);
			top   = (Byte)(initialTop + height + 6 * space);
			if (_insertBefore == null) {
				_insertBefore = _controlFactory.CreateButton(_driverInfo.Driver);
				_insertBefore.Left		= left;
				_insertBefore.Top		= top;
				_insertBefore.Width		= width;
				_insertBefore.Height	= height;
				_insertBefore.Color		= buttonColor;
				_insertBefore.Text		= _textButtonColorEnabled + "Insert Before";
			}

			top += (Byte)(height + space);
			if (_insertAfter == null) {
				_insertAfter = _controlFactory.CreateButton(_driverInfo.Driver);
				_insertAfter.Left		= left;
				_insertAfter.Top		= top;
				_insertAfter.Width		= width;
				_insertAfter.Height		= height;
				_insertAfter.Color		= buttonColor;
				_insertAfter.Text		=  _textButtonColorEnabled + "Insert After";
			}

			width = 40;
			left -= (Byte)(firstButtonWidth + space);
			top += (Byte)(height + 6*space);

			if (_getGridFromRes == null) {
				_getGridFromRes = _controlFactory.CreateButton(_driverInfo.Driver);
				_getGridFromRes.Left = left;
				_getGridFromRes.Top = top;
				_getGridFromRes.Width = width;
				_getGridFromRes.Height = height;
				_getGridFromRes.Color = buttonColor;
				_getGridFromRes.Text = _textButtonColorEnabled + "Get results from last event";
			}

			top += (Byte)(height + space);
			if (_swap == null) {
				_swap = _controlFactory.CreateButton(_driverInfo.Driver);
				_swap.Left	= left;
				_swap.Top	= top;
				_swap.Width	= width;
				_swap.Height= height;
				_swap.Color	= buttonColor;
				_swap.Text	=  _textButtonColorEnabled + "Swap first x drivers";
			}

			top += (Byte)(height + space);
			if (_sendGrid == null) {
				_sendGrid = _controlFactory.CreateButton(_driverInfo.Driver);
				_sendGrid.Left = left;
				_sendGrid.Top = top;
				_sendGrid.Width = width;
				_sendGrid.Height = height;
				_sendGrid.Color = buttonColor;
				_sendGrid.Text = _textButtonColorEnabled + "Send grid into game";
			}

			height	= (Byte)(top + height - initialTop + 2*space);
			top		= (Byte)(initialTop - space);
			left	= (Byte)(leftOrigin - space);
			width	= (Byte)(40 + 2* space);

			if (_labelBackground == null) {
				_labelBackground = _controlFactory.CreateLabel(_driverInfo.Driver);
				_labelBackground.Left	= left;
				_labelBackground.Top	= top;
				_labelBackground.Width	= width;
				_labelBackground.Height	= height;
				_labelBackground.Color	= ButtonColor.Dark;
				_labelBackground.Text	= String.Empty;
			}

			BindEvents();

		}

		private void BindEvents() {
			_down.Click				+= new EventHandler<ButtonClickEventArgs>(_down_Click);
			_up.Click				+= new EventHandler<ButtonClickEventArgs>(_up_Click);
			_insertBefore.Click		+= new EventHandler<ButtonClickEventArgs>(_insertBefore_Click);
			_insertAfter.Click		+= new EventHandler<ButtonClickEventArgs>(_insertAfter_Click);
			_sendGrid.Click			+= new EventHandler<ButtonClickEventArgs>(_sendGrid_Click);
			_swap.Click				+= new EventHandler<ButtonClickEventArgs>(_swap_Click);
			_getGridFromRes.Click	+= new EventHandler<ButtonClickEventArgs>(_getGridFromRes_Click);
		}

		private void UnbindEvents() {
			_down.Click				-= new EventHandler<ButtonClickEventArgs>(_down_Click);
			_up.Click				-= new EventHandler<ButtonClickEventArgs>(_up_Click);
			_insertBefore.Click		-= new EventHandler<ButtonClickEventArgs>(_insertBefore_Click);
			_insertAfter.Click		-= new EventHandler<ButtonClickEventArgs>(_insertAfter_Click);
			_sendGrid.Click			-= new EventHandler<ButtonClickEventArgs>(_sendGrid_Click);
			_swap.Click				-= new EventHandler<ButtonClickEventArgs>(_swap_Click);
			_getGridFromRes.Click	-= new EventHandler<ButtonClickEventArgs>(_getGridFromRes_Click);
		}

		private void _up_Click(object sender, ButtonClickEventArgs e) {
			String lfsDriverName = _labelSelectedDriver.Tag as String;

			_leagueController.GridOrderBuilder.MoveDriverUp(lfsDriverName);
		}

		private void _down_Click(object sender, ButtonClickEventArgs e) {
			_log.Debug("_down_Click");
			String lfsDriverName = _labelSelectedDriver.Tag as String;

			_leagueController.GridOrderBuilder.MoveDriverDown(lfsDriverName);
		}

		private void _insertBefore_Click(object sender, ButtonClickEventArgs e) {
			_log.Debug("_insertBefore_Click");
			_leagueController.GridOrderBuilder.InsertBefore = true;
		}

		private void _insertAfter_Click(object sender, ButtonClickEventArgs e) {
			_log.Debug("_insertAfter_Click");
			_leagueController.GridOrderBuilder.InsertAfter = true;
		}

		private void _sendGrid_Click(object sender, ButtonClickEventArgs e) {
			_log.Debug("_sendGrid_Click");
			_leagueController.GridOrderBuilder.BuildGrid();
		}

		private void _swap_Click(object sender, ButtonClickEventArgs e) {
			_log.Debug("_swap_Click");
			_leagueController.SwapDrivers();
		}

		void _getGridFromRes_Click(object sender, ButtonClickEventArgs e) {
			_log.Debug("_getGridFromRes_Click");
			_leagueController.GetDriversOrderFromResult();
		}


		private void DestroyMenu() {
			Hide();

			if (_labelBackground != null) {
				_labelBackground.Delete();
				_labelBackground = null;
			}

			if (_labelSelectedDriver != null) {
				_labelSelectedDriver.Delete();
				_labelSelectedDriver = null;
			}

			if (_up != null) {
				_up.Delete();
				_up = null;
			}

			if (_down != null) {
				_down.Delete();
				_down = null;
			}

			if (_insertBefore != null) {
				_insertBefore.Delete();
				_insertBefore = null;
			}

			if (_insertAfter != null) {
				_insertAfter.Delete();
				_insertAfter = null;
			}

			if (_sendGrid != null) {
				_sendGrid.Delete();
				_sendGrid = null;
			}

			if (_swap != null) {
				_swap.Delete();
				_swap = null;
			}

			if (_getGridFromRes != null) {
				_getGridFromRes.Delete();
				_getGridFromRes = null;
			}
		}

		private bool isMenuCreated() {
			if (_labelSelectedDriver != null || _up != null || _down != null || _insertBefore != null || _insertAfter != null) {
				Debug.Assert(_labelSelectedDriver != null && _up != null && _down != null && _insertBefore != null && _insertAfter != null);
				return true;
			}

			return false;
		}

		internal void SetSelectedPlayerName(DriverInfo selectedPlayer) {
			if (_labelSelectedDriver == null) {
				return;
			}

			_labelSelectedDriver.Text = selectedPlayer.ColorizedNickName;
			_labelSelectedDriver.Tag = selectedPlayer.LfsUserName;
		}
	}
}
