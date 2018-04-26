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
using System.Timers;

using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins
{
	internal class MessageInfo : IDisposable
	{
		private static ILog		_log		= LogManager.GetLogger(typeof(MessageInfo));
		private static bool		_logDebug	= _log.IsDebugEnabled;
		private bool			_disposed	= false;

		private Timer			_timer;
		private String			_lfsUsername;
		private IInSimDriver	_inSimDriver;
		private ILabel			_label;

		internal Timer Timer
		{
			get { return _timer; }
		}

		internal String LfsUserName
		{
			get { return _lfsUsername; }
			set {
				Debug.Assert(_lfsUsername==null);
				Debug.Assert(_inSimDriver==null);
				Debug.Assert(_label==null);
				_lfsUsername = value;
			}
		}

		
		internal IInSimDriver InSimDriver
		{
			get { return _inSimDriver; }
			set {
				Debug.Assert(_lfsUsername==null);
				Debug.Assert(_inSimDriver==null);
				Debug.Assert(_label==null);
				_inSimDriver = value;
			}
		}

		internal ILabel Label
		{
			get { return _label; }
			set {
				Debug.Assert(_lfsUsername==null);
				Debug.Assert(_inSimDriver==null);
				Debug.Assert(_label==null);
				_label = value;
			}
		}

		internal MessageInfo()
		{
			_timer = new Timer();
			_lfsUsername = null;
			_inSimDriver = null;
		}

		~MessageInfo()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "MessageInfo, call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void Clear()
		{
			if (_label!=null) {
				_label.Delete();
				_label = null;
			}

			_lfsUsername	= null;
			_inSimDriver	= null;
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed) {

				if (disposing) {

					if (_timer != null) {
						_timer.Stop();
						_timer.Dispose();
						_timer = null;
					}

					if (_label != null) {
						_label.Delete();
						_label = null;
					}
				}
				_disposed = true;
			}
		}
	}
}
