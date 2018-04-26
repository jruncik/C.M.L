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

using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;

using log4net;

using System.Diagnostics;

namespace SR.CML.Rallycross
{
	internal class DriverInfo : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(DriverInfo));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool _disposed	= false;

		private bool _canDrive = false;
		internal bool CanDrive
		{
			get { return _canDrive; }
			set { _canDrive = value; }
		}

		private bool _active = false;
		internal bool Active
		{
			get { return _active; }
			set { _active = value; }
		}

		private IInSimDriver _driver = null;
		internal IInSimDriver Driver
		{
			get { return _driver; }
		}

		private String _lfsUserName = null;
		internal String LfsUserName
		{
			get { return _lfsUserName; }
			//set { _lfsUserName = value; } // Test remove it
		}

		private String _colorizedNickName = null;
		internal String ColorizedNickName
		{
			get { return _colorizedNickName; }
		}

		private static DriverInfo _empty;
		internal static DriverInfo Empty
		{
			get {
				if (_empty == null) {
					_empty = new DriverInfo(false);
				}

				return _empty;
			}
		}

		internal bool IsEmpty
		{
			get { return _driver==null; }
		}

		internal DriverInfo(bool canDrive)
		{
			_driver				= null;
			_lfsUserName		= String.Empty;
			_colorizedNickName	= String.Empty;
			_active				= false;
			_canDrive			= canDrive;
		}

		internal DriverInfo(IInSimDriver driver)
		{
			Debug.Assert(driver!=null);
			if (driver==null) {
				_log.Fatal("Driver is null!");
				throw new ArgumentNullException("Driver is null!");
			}

			SetInSimDriver(driver);

			_canDrive		= true;
			_active			= false;
		}

		~DriverInfo()
		{
			Debug.Assert(_disposed, "DriverInfo, call dispose before the application is closed!");
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
				}
				_disposed = true;
			}
		}

		internal void SetInSimDriver(IInSimDriver driver)
		{
			Debug.Assert(driver != null);
#if DEBUG
			if (!String.IsNullOrEmpty(_lfsUserName)) {
				Debug.Assert(_lfsUserName == driver.LfsName);
			}
			if (!String.IsNullOrEmpty(_colorizedNickName)) {
				Debug.Assert(_colorizedNickName == driver.ColorizedNickName);
			}
#endif
			_driver				= driver;
			_lfsUserName		= _driver.LfsName;
			_colorizedNickName	= _driver.ColorizedNickName;
		}

		internal void RemoveInSimDriver()
		{
			_driver = null;
		}
	}
}
