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
using System.Collections.Generic;

using SR.CML.Common;
using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;
using SR.CML.Core.InSimCommon.Controls;

using FullMotion.LiveForSpeed.InSim;
using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins
{
	internal class Messaging : IMessaging
	{
		private static ILog		_log		= LogManager.GetLogger(typeof(Messaging));
		private static bool		_logDebug	= _log.IsDebugEnabled;

		private static String		CML_CONNECTED		= "^1C^7.^1M^7.^1L^7.^8 Connected";
		private static String		CML_DISCONNECTED	= "^1C^7.^1M^7.^1L^7.^8 Disconnected";

		private bool				_disposed					= false;
		private ICMLCore			_cmlCore					= null;
		private InSimHandler		_inSimHandler				= null;
		private IControlFactory		_controlFactory				= null;
		private List<MessageInfo>	_freeMessagesInfo			= null;
		private List<MessageInfo>	_usedMessagesInfo			= null;

		private MessageInfo			_commonRcmAllMessage		= null;
		private MessageInfo			_commonControlAllMessage	= null;

		private Dictionary<String, MessageInfo>			_palyerRcmMessages		= null;
		private Dictionary<IInSimDriver, MessageInfo>	_palyerControlMessages	= null;

		private Object _commonRcmAllLock			= new Object();
		private Object _commonRcmPlayerLock			= new Object();
		private Object _commonControlAllLock		= new Object();
		private Object _commonContreolPlayerLock	= new Object();


		// ToDo global RCM, Control message
		// message per driver - restart time + new message text when new message arrive

		internal Messaging(IPluginManager pluginManager)
		{
			Debug.Assert(pluginManager!=null);
			if (pluginManager==null) {
				_log.Fatal("PluginManager isn't set!");
				throw new ArgumentNullException("PluginManager isn't set!");
			}

			GetRequestedPlugins(pluginManager);

			_freeMessagesInfo		= new List<MessageInfo>(16);
			_usedMessagesInfo		= new List<MessageInfo>(16);
			_palyerRcmMessages		= new Dictionary<String, MessageInfo>(32);
			_palyerControlMessages	= new Dictionary<IInSimDriver, MessageInfo>(32);
		}

		~Messaging()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "Messages, call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed) {

				if (disposing) {

					if (_usedMessagesInfo != null) {

						Debug.Assert(_usedMessagesInfo.Count==0);

						_usedMessagesInfo.Clear();
						_usedMessagesInfo = null;
					}

					if (_freeMessagesInfo != null) {
						foreach (MessageInfo messageInfo in _freeMessagesInfo) {
							Debug.Assert(!messageInfo.Timer.Enabled);
							messageInfo.Dispose();
						}
						_freeMessagesInfo.Clear();
						_freeMessagesInfo = null;
					}
				}
				_disposed = true;
			}
		}

		internal void Activate()
		{
			if (_freeMessagesInfo == null) {
				_freeMessagesInfo = new List<MessageInfo>(16);
			}

			if (_usedMessagesInfo == null) {
				_usedMessagesInfo = new List<MessageInfo>(16);
			}
			SendMessageToAll(CML_CONNECTED);
		}

		internal void Deactivate()
		{
			ClearAllMessages();
			SendMessageToAll(CML_DISCONNECTED);
		}

		#region IMessages

		public void SendCommand(String command)
		{
			if (!_cmlCore.IsConnected) {
				_log.Error("SendCommand - lfs isn't connected");
				return;
			}

			if (String.IsNullOrEmpty(command)) {
				return;
			}

			if (_logDebug) {
				_log.DebugFormat("SendCommand: '{0}'", command);
			}
			_inSimHandler.SendMessage(command);
		}

		public void SendMessageToAll(String message)
		{
			if (!_cmlCore.IsConnected) {
				_log.Error("SendMessageToAll - lfs isn't connected");
				return;
			}

			if (String.IsNullOrEmpty(message)) {
				return;
			}

			if (_logDebug) {
				_log.DebugFormat("SendMessageToAll: '{0}'", message);
			}

			_inSimHandler.SendMessage(message);
		}

		public void SendMessageToCar(IInSimCar car, String message)
		{
			if (!_cmlCore.IsConnected) {
				_log.Error("SendMessageToCar - lfs isn't connected");
				return;
			}

			if (String.IsNullOrEmpty(message)) {
				return;
			}

			Debug.Assert(car!=null);
			if (car==null) {
				return;
			}

			Debug.Assert(car.ActiveDriver!=null);
			if (car.ActiveDriver==null) {
				return;
			}

			Debug.Assert(car.ActiveDriver.PlayerId!=null);
			if (car.ActiveDriver.PlayerId==null) {
				return;
			}

			if (_logDebug) {
				_log.DebugFormat("SendMessageToCar: '{0}', activeDriver: '{1}', id '{2}'", message, car.ActiveDriver.ColorizedNickName, car.ActiveDriver.PlayerId.Value);
			}

			_inSimHandler.SendMessageToPlayer(String.Format("{0}^8 {1}", car.ActiveDriver.ColorizedNickName, message), car.ActiveDriver.PlayerId.Value);
		}

		public void SendMessageToDriver(IInSimDriver driver, String message)
		{
			if (!_cmlCore.IsConnected) {
				_log.Error("SendMessageToDriver - lfs isn't connected");
				return;
			}

			if (String.IsNullOrEmpty(message)) {
				return;
			}

			Debug.Assert(driver!=null);
			if (driver==null) {
				return;
			}

			if (driver.PlayerId!=null) {
				if (_logDebug) {
					_log.DebugFormat("SendMessageToDriver: '{0}', activeDriver: '{1}', id '{2}'", message, driver.ColorizedNickName, driver.PlayerId.Value);
				}
				_inSimHandler.SendMessageToPlayer(String.Format("{0}^8 {1}", driver.ColorizedNickName, message), driver.PlayerId.Value);

			} else {
				Debug.Assert(driver.ConnectionId!=0);
				if (_logDebug) {
					_log.DebugFormat("SendMessageToDriver: '{0}', activeDriver: '{1}', connection id '{2}'", message, driver.ColorizedNickName, driver.ConnectionId);
				}
				_inSimHandler.SendMessageToConnection(String.Format("{0}^8 {1}", driver.ColorizedNickName, message), driver.ConnectionId);
			}
		}

		public void SendRaceControlMessageToAll(String message, Int16 seconds)
		{
			lock (_commonRcmAllLock) {
				if (!_cmlCore.IsConnected) {
					_log.Error("SendRaceControlMessageToAll - lfs isn't connected");
					return;
				}

				if (String.IsNullOrEmpty(message)) {
					return;
				}

				if (_commonRcmAllMessage != null) {
					_commonRcmAllMessage.Timer.Stop();
				} else {
					_commonRcmAllMessage = GetFreeMessageInfo();
					_commonRcmAllMessage.Timer.Elapsed += new ElapsedEventHandler(timer_RcmToAllElapsed);
				}


				SendCommand(LfsCommands.RcmMessageText(message));
				SendCommand(LfsCommands.RcmSendToAll());

				_commonRcmAllMessage.Timer.Interval = seconds * 1000;
				_commonRcmAllMessage.Timer.Start();

			}
		}

		public void SendRaceControlMessageToCar(IInSimCar car, String message, Int16 seconds)
		{
			lock(_commonRcmPlayerLock) {
				if (!_cmlCore.IsConnected) {
					_log.Error("SendRaceControlMessageToCar - lfs isn't connected");
					return;
				}

				if (car==null) {
					_log.DebugFormat("SendRaceControlMessageToCar, car is null. Message '{0}'wasn't dispalyed!", message);
					return;
				}

				if (car.ActiveDriver==null) {
					_log.DebugFormat("SendRaceControlMessageToCar, ActiveDriver is null. Message '{0}'wasn't dispalyed!", message);
					return;
				}

				if (String.IsNullOrEmpty(message)) {
					return;
				}

				MessageInfo messageInfo = null;

				if (_palyerRcmMessages.ContainsKey(car.ActiveDriver.LfsName)) {
					messageInfo = _palyerRcmMessages[car.ActiveDriver.LfsName];
					messageInfo.Timer.Stop();
					Debug.Assert(messageInfo.LfsUserName == car.ActiveDriver.LfsName);
				} else {
					messageInfo = GetFreeMessageInfo();
					messageInfo.Timer.Elapsed += new ElapsedEventHandler(timer_RcmToPlayerElapsed);
					_palyerRcmMessages.Add(car.ActiveDriver.LfsName, messageInfo);
					messageInfo.LfsUserName = car.ActiveDriver.LfsName;
				}

				SendCommand(LfsCommands.RcmMessageText(message));
				SendCommand(LfsCommands.RcmSendToPlayer(messageInfo.LfsUserName));

				messageInfo.Timer.Interval = seconds * 1000;
				messageInfo.Timer.Start();
			}
		}

		public void SendControlMessageToAll(String message, Int16 seconds)
		{
			lock (_commonControlAllLock) {
				if (!_cmlCore.IsConnected) {
					_log.Error("SendControlMessageToAll - lfs isn't connected");
					return;
				}

				if (String.IsNullOrEmpty(message)) {
					return;
				}

				if (_commonControlAllMessage != null) {
					_commonControlAllMessage.Timer.Stop();
				} else {
					_commonControlAllMessage = GetFreeMessageInfo();
					ILabel label = _controlFactory.CreateLabel();
					label.Top		= 20;
					label.Left		= 40;
					label.Width		= 120;
					label.Height	= 26;
					label.Text		= message;

					_commonControlAllMessage.Label = label;
					_commonControlAllMessage.Timer.Elapsed += new ElapsedEventHandler(timer_MessageToAllElapsed);
				}

				_commonControlAllMessage.Label.Show();

				_commonControlAllMessage.Timer.Interval = seconds * 1000;
				_commonControlAllMessage.Timer.Start();
			}
		}

		public void SendControlMessageToCar(IInSimCar car, String message, Int16 seconds)
		{
			lock(_commonContreolPlayerLock) {
				if (!_cmlCore.IsConnected) {
					_log.Error("SendControlMessageToCar - lfs isn't connected");
					return;
				}

				if (car==null) {
					_log.DebugFormat("SendControlMessageToCar, car is null. Message '{0}'wasn't dispalyed!", message);
					return;
				}

				if (car.ActiveDriver==null) {
					_log.DebugFormat("SendControlMessageToCar, ActiveDriver is null. Message '{0}'wasn't dispalyed!", message);
					return;
				}

				if (String.IsNullOrEmpty(message)) {
					return;
				}

				MessageInfo messageInfo = null;

				if (_palyerControlMessages.ContainsKey(car.ActiveDriver)) {
					messageInfo = _palyerControlMessages[car.ActiveDriver];
					messageInfo.Timer.Stop();
					Debug.Assert(messageInfo.LfsUserName == car.ActiveDriver.LfsName);
				} else {
					messageInfo = GetFreeMessageInfo();

					ILabel label = _controlFactory.CreateLabel(car.ActiveDriver);
					label.Top	= 10;
					label.Left	= 50;
					label.Width	= 100;
					label.Height= 20;
					label.Text	= message;

					messageInfo.Label = label;
					messageInfo.Timer.Elapsed += new ElapsedEventHandler(timer_ControlToPlayerElapsed);

					_palyerControlMessages.Add(car.ActiveDriver, messageInfo);
					messageInfo.InSimDriver = car.ActiveDriver;
				}

				messageInfo.Label.Show();

				messageInfo.Timer.Interval = seconds * 1000;
				messageInfo.Timer.Start();
			}
		}

		#endregion

		private void GetRequestedPlugins(IPluginManager pluginManager)
		{
			_cmlCore = pluginManager.GetPlugin(CmlPlugins.CMLCoreGuid) as ICMLCore;
			Debug.Assert(_cmlCore != null);
			if (_cmlCore == null) {
				_log.Fatal("CML Core plugin wasn't found");
				throw new ArgumentNullException("CML Core plugin wasn't found!");
			}

			_inSimHandler = _cmlCore.InSimHandler;
			Debug.Assert(_inSimHandler != null);
			if (_inSimHandler == null) {
				_log.Fatal("InSimHandler isn't set!");
				throw new ArgumentNullException("InSimHandler isn't set!");
			}

			_controlFactory = pluginManager.GetPlugin(CmlPlugins.ControlFactoryGuid) as IControlFactory;
			Debug.Assert(_controlFactory != null);
			if (_controlFactory == null) {
				_log.Fatal("Control Factory plugin wasn't found");
				throw new ArgumentNullException("Control Factory plugin wasn't found!");
			}
		}

		private MessageInfo GetMessageInfo(Timer timer)
		{
			foreach(MessageInfo messageInfo in _usedMessagesInfo) {
				if (messageInfo.Timer==timer) {
					return messageInfo;
				}
			}
			
			return null;
		}

		private MessageInfo GetFreeMessageInfo()
		{
			MessageInfo messageInfo = null;
			if (_freeMessagesInfo.Count>0) {
				messageInfo = _freeMessagesInfo[0];
				_freeMessagesInfo.RemoveAt(0);
			} else {
				messageInfo = new MessageInfo();
			}

			_usedMessagesInfo.Add(messageInfo);
			return messageInfo;
		}

		private void ReturnUsedMessageInfo(MessageInfo messageInfo)
		{
			lock(this) {
				messageInfo.Timer.Stop();
				messageInfo.Timer.Enabled = false;
				messageInfo.Clear();

				if (!_usedMessagesInfo.Contains(messageInfo)) {
					if (_freeMessagesInfo.Contains(messageInfo)) {
						Debug.Assert(false, "Returning free Message info agian!");
						_log.Error("Returning free Message info agian!");
						return;
					}

					Debug.Assert(false, "Returning unknown Message info agian!");
					_log.Error("Returning unknown Message info agian!");
					return;
				}

				_usedMessagesInfo.Remove(messageInfo);
				_freeMessagesInfo.Add(messageInfo);
			}
		}

		private void timer_RcmToAllElapsed(object sender, ElapsedEventArgs e)
		{
			lock (_commonRcmAllLock) {
				Timer timer = sender as Timer;
				Debug.Assert(timer!=null);

				MessageInfo messageInfo = GetMessageInfo(timer);
				Debug.Assert(messageInfo!=null);
				if (messageInfo==null) {
					_log.Fatal("Messade info for timer wasn't found!");
					return;
				}

				Debug.Assert(messageInfo == _commonRcmAllMessage);

				_commonRcmAllMessage.Timer.Stop();
				_commonRcmAllMessage.Timer.Elapsed -= new ElapsedEventHandler(timer_RcmToAllElapsed);

				SendCommand(LfsCommands.ClearAllRcm());

				ReturnUsedMessageInfo(_commonRcmAllMessage);
				_commonRcmAllMessage = null;
			}
		}

		private void timer_RcmToPlayerElapsed(object sender, ElapsedEventArgs e)
		{
			lock(_commonRcmPlayerLock) {
				Timer timer = sender as Timer;
				Debug.Assert(timer!=null);

				MessageInfo messageInfo = GetMessageInfo(timer);
				Debug.Assert(messageInfo!=null);
				if (messageInfo==null) {
					_log.Fatal("Messade info for timer wasn't found!");
					return;
				}

				if (!_palyerRcmMessages.ContainsKey(messageInfo.LfsUserName)) {
					Debug.Assert(false);
					_log.Error ("timer_RcmToPlayerElapsed, but Messagei info wasn't found in _palyerRcmMessages");
				}

				messageInfo.Timer.Stop();
				messageInfo.Timer.Elapsed -= new ElapsedEventHandler(timer_RcmToPlayerElapsed);

				SendCommand(LfsCommands.ClearPlayerRcm(messageInfo.LfsUserName));
				_palyerRcmMessages.Remove(messageInfo.LfsUserName);
				ReturnUsedMessageInfo(messageInfo);
			}
		}

		private void timer_MessageToAllElapsed(object sender, ElapsedEventArgs e)
		{
			lock (_commonControlAllLock) {
				Timer timer = sender as Timer;
				Debug.Assert(timer!=null);

				MessageInfo messageInfo = GetMessageInfo(timer);
				Debug.Assert(messageInfo!=null);
				if (messageInfo==null) {
					_log.Fatal("Messade info for timer wasn't found!");
					return;
				}

				Debug.Assert(messageInfo == _commonControlAllMessage);

				_commonControlAllMessage.Timer.Stop();
				_commonControlAllMessage.Timer.Elapsed -= new ElapsedEventHandler(timer_MessageToAllElapsed);

				Debug.Assert(_commonControlAllMessage.Label!=null);
				_commonControlAllMessage.Label.Hide();

				ReturnUsedMessageInfo(_commonControlAllMessage);
				_commonControlAllMessage = null;
			}
		}

		private void timer_ControlToPlayerElapsed(object sender, ElapsedEventArgs e)
		{
			lock(_commonContreolPlayerLock) {
				Timer timer = sender as Timer;
				Debug.Assert(timer!=null);

				MessageInfo messageInfo = GetMessageInfo(timer);
				Debug.Assert(messageInfo!=null);
				if (messageInfo==null) {
					_log.Fatal("Messade info for timer wasn't found!");
					return;
				}

				if (!_palyerControlMessages.ContainsKey(messageInfo.InSimDriver)) {
					Debug.Assert(false);
					_log.Error ("timer_RcmToPlayerElapsed, but Messagei info wasn't found in _palyerRcmMessages");
				}

				messageInfo.Timer.Stop();
				messageInfo.Timer.Elapsed -= new ElapsedEventHandler(timer_ControlToPlayerElapsed);

				SendCommand(LfsCommands.ClearPlayerRcm(messageInfo.LfsUserName));
				_palyerControlMessages.Remove(messageInfo.InSimDriver);
				ReturnUsedMessageInfo(messageInfo);
			}
		}

		private void ClearAllMessages()
		{
			lock (this) {
				if (_usedMessagesInfo!=null) {
					foreach (MessageInfo messageInfo in _usedMessagesInfo) {
						messageInfo.Timer.Stop();

						if (messageInfo.Label != null) {
							messageInfo.Label.Hide();
						} else if (!String.IsNullOrEmpty(messageInfo.LfsUserName)) {
							SendCommand(LfsCommands.ClearPlayerRcm(messageInfo.LfsUserName));
						}

						messageInfo.Clear();
						messageInfo.Dispose();
					}

					_usedMessagesInfo.Clear();
				}
				_usedMessagesInfo = null;
			}
			SendCommand(LfsCommands.ClearAllRcm());
		}

	}
}
