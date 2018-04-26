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

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class GroupManager : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(GroupManager));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private static Byte DRIVERS_PER_BASIC_GROUP		= 6;

		private bool				_disposed			= false;
		private Rallycross			_rallycross			= null;
		private IControlFactory		_controlFactory		= null;
		private ILabel				_background			= null;
		private ILabel				_logo				= null;
		private Byte				_left				= 0;
		private Byte				_top				= 0;
		private bool				_swapRows			= false;
		private Int32				_activeGroupIndex	= 0;
		private Int32				_heatIndex			= 0;

		internal event EventHandler<ActivatingNextGroupEventArgs>	ActivatingNextGroup;

		private List<Group> _groups = null;
		internal IList<Group> Groups
		{
			get { return _groups.AsReadOnly(); }
		}

		private static Byte _groupSpace = 3;
		internal static Byte GroupSpace
		{
			get { return _groupSpace; }
			set { _groupSpace = value; }
		}

		internal Group ActiveGroupe
		{
			get { return _groups[_activeGroupIndex]; }
		}

		internal GroupManager(Rallycross rallycross)
		{
			Debug.Assert(rallycross!=null);
			if (rallycross==null) {
				_log.Fatal("Rallycross is null!");
				throw new ArgumentNullException("Rallycross is null!");
			}
			_rallycross = rallycross;

			_controlFactory = _rallycross.PluginManager.GetPlugin(CmlPlugins.ControlFactoryGuid) as IControlFactory;
			if (rallycross==null) {
				_log.Fatal("Control factory wasn't found!");
				throw new ArgumentNullException("Control factory wasn't found!");
			}

			_background	= null;
			_logo		= null;
			_swapRows	= false;
		}

		~GroupManager()
		{
			Debug.Assert(_disposed, "RallycrossDirector, call dispose before the application is closed!");
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
					ClearGroupManager();
				}
				_disposed = true;
			}
		}

		private void ClearGroupManager()
		{
			if (_logo!=null) {
				_logo.Delete();
				_logo = null;
			}

			if (_background!=null) {
				_background.Delete();
				_background = null;
			}

			ClearGroups();
		}

		internal void CreateBasicGroups()
		{
			Int32 driversCount	= _rallycross.Drivers.Count;
			//driversCount = 15;
			Int32 groupsCount	= CalculateGroupsCount(driversCount);

			_top = 140;

			CreateGroups(groupsCount, DRIVERS_PER_BASIC_GROUP);
			GenerateBasicGroupNames(groupsCount);
			RandomizeDriversInGroupes(groupsCount);
			CreateControls(groupsCount);

			_activeGroupIndex	= 0;
			_heatIndex			= 0;
			_groups[_activeGroupIndex].IsActive = true;
		}

		internal void CreateFinalGroup(String finalName)
		{
			_top = 80;

			CreateGroups(1, 9);
			_groups[0].Name = finalName;
			CreateControls(1);

			_activeGroupIndex	= 0;
			_heatIndex			= 0;
			_groups[_activeGroupIndex].IsActive = true;
		}

		private Int32 CalculateGroupsCount(Int32 driversCount)
		{
			Byte driversPerGroup	= 0;

			for (Byte i = 1; i < 7; ++i) {
				driversPerGroup = (Byte)Math.Ceiling((double)driversCount / (double)i);
				if (driversPerGroup <= 6) {
					return i;
				}
			}

			return 1;
		}

		internal void SetGridForActiveGroup()
		{
			_groups[_activeGroupIndex].SetGrid();
		}

		internal void ActivateNextGroupe()
		{
			OnActivatingNextGroup();

			_groups[_activeGroupIndex].IsActive = false;

			if (_swapRows) {
				_groups[_activeGroupIndex].SwapDriversByRows();
			} else {
				_groups[_activeGroupIndex].SwapDriverBySide();
			}

			++_activeGroupIndex;
			_activeGroupIndex = _activeGroupIndex % _groups.Count;

			if (_activeGroupIndex == 0) {
				_swapRows = !_swapRows;
				++_heatIndex;
			}

			_groups[_activeGroupIndex].IsActive = true;
		}

		internal void ShowGroups()
		{
			_background.Show();
			foreach(Group group in _groups) {
				group.Show();
			}
			_logo.Show();
		}

		internal void HideGroups()
		{
			foreach(Group group in _groups) {
				group.Hide();
			}

			_logo.Hide();
			_background.Hide();
		}

		private void ClearGroups()
		{
			if (_groups != null) {
				foreach (Group group in _groups) {
					group.Dispose();
				}
				_groups.Clear();
				_groups = null;
			}
		}

		private void RandomizeDriversInGroupes(Int32 groupCount)
		{
			Int32				driversCount	= _rallycross.Drivers.Count;
			Random				random			= new Random((Int32)DateTime.Now.Ticks);
			List<DriverInfo>	drivers			= _rallycross.Drivers.GetRange(0, driversCount);
			DriverInfo			driverInfo		= null;
			Int32				index			= 0;
			Int32				roundIndex		= 0;

			while (driversCount > 0) {
				index = random.Next(driversCount--);
				Debug.Assert(index >= 0 && index <= driversCount);

				driverInfo = drivers[index];
				_groups[roundIndex].AddDriver(driverInfo);

				drivers.RemoveAt(index);

				roundIndex = (roundIndex + 1) % groupCount;
			}
		}

		private void GenerateBasicGroupNames(Int32 groupCount)
		{
			for (Int32 i = 0; i < groupCount; ++i) {
				_groups[i].Name = String.Format("Group - {0}", (i + 1).ToString());
			}
		}

		private void CreateGroups(Int32 groupCount, Byte driversPerGroup)
		{
			if (_groups != null) {
				ClearGroupManager();
			}
			_groups = new List<Group>(groupCount);

			Int32 left = CalculateFirstGroupLeftPosition(groupCount);
			Group group = null;

			for (Int32 i = 0; i < groupCount; ++i) {
				group = new Group(_rallycross.PluginManager, driversPerGroup);
				group.Left = (Byte)left;
				group.Top = _top;
				_groups.Add(group);

				left = (Byte)(left + Group.ItemWidth + _groupSpace);
			}
		}

		private Int32 CalculateFirstGroupLeftPosition(Int32 groupCount)
		{
			Int32 left = groupCount * Group.ItemWidth;
			left = left + ((groupCount - 1) * _groupSpace);
			left = left / 2;
			left = 100 - left;

			_left = (Byte)left;
			return left;
		}

		private void CreateControls(Int32 groupCount)
		{
			Byte height	= 0;
			Byte width	= 0;

			for (Int32 i=0; i<groupCount; ++i) {
				if (i<groupCount-1) {
					width	= (Byte) (width + Group.ItemWidth + _groupSpace);
				} else {
					width	= (Byte) (width + Group.ItemWidth);
				}
			}

			foreach(Group group in _groups) {
				group.CreateLabels();
				height = Math.Max(group.Height, height);
			}

			height	+= 8;
			width	+= 4;

			_background			= _controlFactory.CreateLabel();
			_background.Left		= (Byte)(_left - 2);
			_background.Top		= (Byte)(_top - 2);
			_background.Width	= width;
			_background.Height	= height;
			_background.Color	= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Dark;

			_logo			= _controlFactory.CreateLabel();
			_logo.Left		= (Byte)(_left + width - 4 - 26);
			_logo.Top		= (Byte)(_top + height - 4 - 4);
			_logo.Width		= 26;
			_logo.Height	= 4;
			_logo.Text		= "^9Powerd by ^2Es^3car^7got^9 © (2009)";
			_logo.Color		= FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Transparent;
		}

		private void OnActivatingNextGroup()
		{
			if (ActivatingNextGroup != null) {
				Int32 nextGroupIndex = _activeGroupIndex;
				++nextGroupIndex;
				nextGroupIndex = nextGroupIndex % _groups.Count;

				ActivatingNextGroup(this, new ActivatingNextGroupEventArgs(_groups[_activeGroupIndex], _heatIndex, nextGroupIndex==0));
			}
		}
	}
}
