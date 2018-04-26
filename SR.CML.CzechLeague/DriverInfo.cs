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

using log4net;

using System.Diagnostics;

namespace SR.CML.CzechLeague
{
	internal class DriverInfo : IDisposable
	{
		private static ILog _log		= LogManager.GetLogger(typeof(DriverInfo));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool _disposed	= false;

		private bool _canParticipate = true;
		internal bool CanParticipate
		{
			get { return _canParticipate; }
			set {
				_log.DebugFormat("Player '{0}', CanParticipate={1}", _lfsUserName, value);
				_canParticipate = value;
			}
		}

		private bool _canRce = true;
		internal bool CanRace {
			get { return _canRce; }
			set {
				_log.DebugFormat("Player '{0}', CanRace={1}", _lfsUserName, value);
				_canRce = value;
			}
		}

		private IInSimDriver _driver = null;
		internal IInSimDriver Driver
		{
			get { return _driver; }
		}

		private String _lfsUserName = String.Empty;
		internal String LfsUserName
		{
			get { return _lfsUserName; }
		}

		private String _colorizedNickName = String.Empty;
		internal String ColorizedNickName
		{
			get { return _colorizedNickName; }
		}

		private static DriverInfo _empty;
		internal static DriverInfo Empty
		{
			get {
				if (_empty == null) {
					_empty = new DriverInfo(String.Empty);
					_empty._canParticipate = false;
				}

				return _empty;
			}
		}

		internal bool IsEmpty
		{
			get { return _driver==null; }
		}

		internal DriverInfo(String lfsUserName)
		{
			_driver				= null;
			_lfsUserName		= lfsUserName.ToLower();
			_colorizedNickName	= String.Empty;
			_canParticipate		= true;
			_log.DebugFormat("DriverInfor created for lfsUsername'{0}'", lfsUserName);
		}

		internal DriverInfo(IInSimDriver driver)
		{
			Debug.Assert(driver!=null);
			if (driver==null) {
				_log.Fatal("Driver is null!");
				throw new ArgumentNullException("Driver is null!");
			}

			SetInSimDriver(driver);

			_log.DebugFormat("DriverInfor created for Driver '{0}'", _lfsUserName);
			_canParticipate = true;
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
			if (_colorizedNickName!=String.Empty && !String.IsNullOrEmpty(_colorizedNickName)) {
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
