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

using SR.CML.Core.InSimCommon;

using FullMotion.LiveForSpeed.InSim;
using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.CarDriverManager
{
	internal class InSimDriver : IInSimDriver
	{
		private static readonly InSimDriver _empty = new InSimDriverEmpty();
		private class InSimDriverEmpty : InSimDriver
		{
			~InSimDriverEmpty() {
			}

			public override IInSimCar Car
			{
				get { return InSimCar.Empty; }
			}

			public override Byte? PlayerId
			{
				get { return 0; }
			}

			public override Byte ConnectionId
			{
				get { return 0; }
			}

			public override bool IsEmpty
			{
				get { return true; }
			}

			internal override void Activate(Byte newPlayerId)
			{
				throw new InvalidOperationException("Empty driver doesn't offer any functionality!");
			}

			internal override void Deactivate()
			{
				throw new InvalidOperationException("Empty driver doesn't offer any functionality!");
			}
		}

		private static ILog _log		= LogManager.GetLogger(typeof(InSimDriver));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private bool		_disposed				= false;
		private DriverState	_state					= DriverState.Undefined;
		private String		_lfsName				= null;
		private String		_inGameName				= null;
		private String		_colorizedInGameName	= null;
		private Byte?		_playerId				= null;
		private Byte		_connectionId			= 0;

		private InSimCar _car = InSimCar.Empty;
		internal InSimCar InSimCar
		{
			get { return _car; }
			set { _car = value; }
		}

		internal static InSimDriver Empty
		{
			get { return _empty; }
		}

		protected InSimDriver()
		{
			_lfsName				= String.Empty;
			_colorizedInGameName	= String.Empty;
			_inGameName				= String.Empty;
		}

		internal InSimDriver(String lfsName, String colorizedInGameName, Byte connectionID)
		{
			Debug.Assert(!String.IsNullOrEmpty(lfsName));
			Debug.Assert(!String.IsNullOrEmpty(colorizedInGameName));

			if (String.IsNullOrEmpty(lfsName)) {
				_log.Error("LfsName can't be empty!");
				throw new ArgumentException("LfsName can't be empty!");
			}

			if (String.IsNullOrEmpty(colorizedInGameName)) {
				_log.Error("ColorizedInGameName can't be empty!");
				throw new ArgumentException("ColorizedInGameName can't be empty!");
			}

			_playerId		= null;
			_connectionId	= connectionID;

			_lfsName				= lfsName.ToLower();
			_colorizedInGameName	= colorizedInGameName;
			_inGameName				= null;

			if (_logDebug) {
				_log.Debug(String.Format("New driver lfsName: '{0}', nickname: '{1}', connectionID: '{2}'", _lfsName, _colorizedInGameName, _connectionId));
			}
		}

		~InSimDriver()
		{
			if (!IsEmpty) {
				if (_logDebug) {
					_log.Debug("Wasn't disposed before the application was closed!");
				}
				Debug.Assert(_disposed, "InSimDriver, call dispose before the application is closed!");
				}
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

					Debug.Assert(_state==DriverState.Disconnecting);

					if (_logDebug) {
						_log.Debug(String.Format("Disposed driver lfsName: '{0}', nickname: '{1}', connectionID: '{2}'", _lfsName, _colorizedInGameName, _connectionId));
					}
				}
				_disposed = true;
			}
		}

		#region IInSimDriver

		public virtual IInSimCar Car
		{
			get { return _car; }
		}

		public String LfsName
		{
			get { return _lfsName; }
		}

		public String NickName
		{
			get {
				if (_inGameName==null) {
					_inGameName = GetPlayerNameWithoutColors(_colorizedInGameName);
				}
				return _inGameName;
			}

			internal set {
				if (_colorizedInGameName!=value) {
					if (_logDebug) {
						_log.Debug(String.Format("Driver renamed old: '{0}', new: '{1}'", _colorizedInGameName, value));
					}
					_colorizedInGameName = value;
					_inGameName = null;
				}
			}
		}

		public String ColorizedNickName
		{
			get { return _colorizedInGameName; }
		}

		public virtual Byte? PlayerId
		{
			get { return _playerId; }
		}

		internal Byte ActivePlayerId
		{
			get {
				Debug.Assert(_playerId!=null);
				if (_playerId==null) {
					return 0;
				}
				return _playerId.Value;
			}
		}

		public virtual Byte ConnectionId
		{
			get { return _connectionId; }
		}

		public DriverState State
		{
			get { return _state; }
			internal set { _state = value; }
		}

		public virtual bool IsEmpty
		{
			get { return false; }
		}

		#endregion

		internal virtual void Activate(Byte newPlayerId)
		{
			Debug.Assert(_playerId==null);
			if (_playerId!=null) {
				_log.Error(String.Format("Driver '{0}' has playerId '{1}', new playrId '{2}'", _lfsName, _playerId.Value, newPlayerId));
			}

			_playerId = newPlayerId;

			if (_logDebug) {
				_log.Debug(String.Format("Driver '{0}' activated with playerID: '{1}'", _lfsName, _playerId));
			}
		}

		internal virtual void Deactivate()
		{
			Debug.Assert(_playerId!=null);
			if (_playerId==null) {
				_log.Error(String.Format("Driver '{0}' doesn't have playerId'", _lfsName));
			}

			_playerId = null;

			if (_logDebug) {
				_log.Debug(String.Format("Driver '{0}' deactivated", _lfsName));
			}
		}

		internal static String GetPlayerNameWithoutColors(String playerName)
		{
			if (String.IsNullOrEmpty(playerName)) {
				return String.Empty;
			}

			StringBuilder	plainPlayerName	= new StringBuilder(playerName.Length);
			Int32			i			= 0;

			while (i < playerName.Length) {
				if (playerName[i]=='^') {
					++i;
					if (i<playerName.Length && !Char.IsNumber(playerName[i])) {
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
