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

namespace SR.CML.CommonPlugins.Controls
{
	internal class ControlFactory : IControlFactory
	{
		private static ILog				_log						= LogManager.GetLogger(typeof(ControlFactory));
		private static bool				_logDebug					= _log.IsDebugEnabled;
		private static readonly Byte	MAX_BUTTONS_COUNT			= 0xF0;
		private static readonly Byte	MAX_CUSTOM_BUTTONS_COUNT	= 0x80;
		private static readonly Byte	COMMON_CONTROLS_INITIAL_ID	= (Byte)(MAX_CUSTOM_BUTTONS_COUNT + 1);

		private bool										_disposed				= false;
		private Dictionary<Byte, Dictionary<Byte, Control>>	_controlsPerConnection	= null;

		private ICarDriverManager _carAndDriverManager = null;
		public ICarDriverManager CarAndDriverManager {
			set { _carAndDriverManager = value; }
		}

		private InSimHandler _inSimHandler	= null;
		internal InSimHandler InSimHandler
		{
			get { return _inSimHandler; }
		}

		internal ControlFactory(IPluginManager pluginManager)
		{
			ICMLCore cmlCore = pluginManager.GetPlugin(CmlPlugins.CMLCoreGuid) as ICMLCore;
			Debug.Assert(cmlCore != null);
			if (cmlCore == null) {
				_log.Fatal("CML Core plugin wasn't found");
				throw new ArgumentNullException("CML Core plugin wasn't found!");
			}

			_inSimHandler = cmlCore.InSimHandler;
			Debug.Assert(_inSimHandler != null);
			if (_inSimHandler == null) {
				_log.Fatal("InSimHandler isn't set!");
				throw new ArgumentNullException("InSimHandler isn't set!");
			}
		}

		~ControlFactory()
		{
			if (_logDebug) {
				_log.Debug("Wasn't disposed before the application was closed!");
			}
			Debug.Assert(_disposed, "ControlFactory, call dispose before the application is closed!");
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
					ClearControls(true);
				}
				_disposed = true;
			}
		}

		#region IControlFactory

		public IButton CreateButton() {
			Dictionary<Byte, Control> controls = GetOrCreateControlsForConnection(255);

			if (controls.Count >= MAX_CUSTOM_BUTTONS_COUNT) {
				_log.Error("Insufficient resources");
				return Button.Empty;
			}

			Button button = new Button(this, GenerateCommonId(controls), 255);
			controls.Add(button.ControlId, button);

			return button;
		}

		public IButton CreateButton(IInSimDriver driver)
		{
			if (driver==null) {
				_log.Error("Button can't be created for null driver!");
				throw new ArgumentNullException("Driver can't be null!");
			}
			if (driver.IsEmpty) {
				_log.Error("Button can't be created for empty driver!");
				return Button.Empty;
			}

			Dictionary<Byte, Control> controls = GetOrCreateControlsForConnection(driver.ConnectionId);

			if (controls.Count>=MAX_CUSTOM_BUTTONS_COUNT) {
				_log.Error("Insufficient resources");
				return Button.Empty;
			}

			Button button = new Button(this, GenerateCustomId(controls), driver.ConnectionId);
			controls.Add(button.ControlId, button);

			return button;
		}

		public ILabel CreateLabel(IInSimDriver driver)
		{
			if (driver==null) {
				_log.Error("Label can't be created for null driver!");
				throw new ArgumentNullException("Driver can't be null!");
			}
			if (driver.IsEmpty) {
				_log.Error("Label can't be created for empty driver!");
				return Label.Empty;
			}

			Dictionary<Byte, Control> controls = GetOrCreateControlsForConnection(driver.ConnectionId);

			if (controls.Count>=MAX_CUSTOM_BUTTONS_COUNT) {
				_log.Error("Insufficient resources");
				return Label.Empty;
			}

			Label label = new Label(this, GenerateCustomId(controls), driver.ConnectionId);
			controls.Add(label.ControlId, label);

			return label;
		}

		public ILabel CreateLabel()
		{
			Dictionary<Byte, Control> controls = GetOrCreateControlsForConnection(255);
			if (controls.Count>=MAX_BUTTONS_COUNT) {
				_log.Error("Insufficient resources");
				return Label.Empty;
			}

			Label label = new Label(this, GenerateCommonId(controls), 255);
			Debug.Assert(!controls.ContainsKey(label.ControlId));
			controls.Add(label.ControlId, label);

			return label;
		}

		public ICountDown CreateCountDown()
		{
			return new CountDown(this);
		}

		public IEditBox CreateEditBox(IInSimDriver driver)
		{
			throw new NotImplementedException("Not supported now");
		}

		#endregion

		internal bool Activate()
		{
			Debug.Assert(_controlsPerConnection==null);
			_controlsPerConnection = new Dictionary<Byte,Dictionary<Byte,Control>>(256);

			BindEvents();

			return true;
		}

		internal bool Deactivate()
		{
			UnbindEvents();
			ClearControls(false);

			return true;
		}

		internal void RemoveControl(Control controlToRemove)
		{
			Dictionary<Byte, Control> controls = GetControlsForConnection(controlToRemove.ConnectionId);

			Debug.Assert(controls!=null, "There is a control which isn't added in connection dictionary?");
			if (controls==null) {
				_log.Error(String.Format("There is a control id '{0}', connId '{1}' which isn't added in dictionary?", controlToRemove.ControlId, controlToRemove.ConnectionId));
				return;
			}

			if (_logDebug) {
				_log.Debug(String.Format("Removing control Id: '{0}', ConnectionId: '{1}'", controlToRemove.ControlId, controlToRemove.ConnectionId));
			}

			Debug.Assert(controls.ContainsKey(controlToRemove.ControlId));
			if (!controls.ContainsKey(controlToRemove.ControlId)) {
				_log.Error(String.Format("You are removing control which is already removed! Id: '{0}', ConnectionId: '{1}'", controlToRemove.ControlId, controlToRemove.ConnectionId));
				return;
			}

			controls.Remove(controlToRemove.ControlId);
			// Don't call Dispose on control. It will call it to itself. (Cycle from Dispose possible ...)
		}

		private void BindEvents()
		{
			_inSimHandler.ButtonsCleared			+= new FullMotion.LiveForSpeed.InSim.EventHandlers.ButtonsClearedHandler(_inSimHandler_ButtonsCleared);
			_inSimHandler.ButtonsRequest			+= new FullMotion.LiveForSpeed.InSim.EventHandlers.ButtonsRequestHandler(_inSimHandler_ButtonsRequest);
			_inSimHandler.ButtonClick				+= new FullMotion.LiveForSpeed.InSim.EventHandlers.ButtonClickHandler(_inSimHandler_ButtonClick);
			_inSimHandler.ButtonType				+= new FullMotion.LiveForSpeed.InSim.EventHandlers.ButtonTypeHandler(_inSimHandler_ButtonType);
			_inSimHandler.RaceTrackConnectionLeave	+= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackConnectionLeaveHandler(_inSimHandler_RaceTrackConnectionLeave);
		}

		private void UnbindEvents()
		{
			_inSimHandler.ButtonsCleared			-= new FullMotion.LiveForSpeed.InSim.EventHandlers.ButtonsClearedHandler(_inSimHandler_ButtonsCleared);
			_inSimHandler.ButtonsRequest			-= new FullMotion.LiveForSpeed.InSim.EventHandlers.ButtonsRequestHandler(_inSimHandler_ButtonsRequest);
			_inSimHandler.ButtonClick				-= new FullMotion.LiveForSpeed.InSim.EventHandlers.ButtonClickHandler(_inSimHandler_ButtonClick);
			_inSimHandler.ButtonType				-= new FullMotion.LiveForSpeed.InSim.EventHandlers.ButtonTypeHandler(_inSimHandler_ButtonType);
			_inSimHandler.RaceTrackConnectionLeave	-= new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackConnectionLeaveHandler(_inSimHandler_RaceTrackConnectionLeave);
		}

		private void _inSimHandler_ButtonsCleared(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.ButtonsCleared e)
		{
			if (!e.Local) {
				ClearButonsOnClient(e.ConnectionId);
			}
		}

		private void _inSimHandler_ButtonsRequest(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.ButtonsRequest e)
		{
			_log.FatalFormat("ButtonsRequest, not implemented. ConnectionId '{0}'", e.ConnectionId);
		}

		private void _inSimHandler_ButtonClick(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.ButtonClick e)
		{
			Byte connectionID = e.ConnectionId;
			if (e.ButtonId>MAX_CUSTOM_BUTTONS_COUNT) {
				_log.DebugFormat("ButtonClick: Changing connectionID from '{0}' to '{1}'", e.ConnectionId, 255);
				connectionID = 255;
			}

			Dictionary<Byte, Control> controls = GetControlsForConnection(connectionID);
			if (controls==null) {
				_log.ErrorFormat("Controls for connection '{0}' doesn't exists", connectionID);
				return;
			}

			if (!controls.ContainsKey(e.ButtonId)) {
				_log.ErrorFormat("Button id '{0}' doesn't exists in controls '{1}'", e.ButtonId, connectionID);
				return;
			}

			Button button = controls[e.ButtonId] as Button;
			Debug.Assert(button!=null, "You can't obtain click event from non-click-able control!");
			if (button==null) {
				return;
			}

			Debug.Assert(button.ConnectionId == connectionID);
			if (button.ConnectionId != connectionID) {
				return;
			}

			MouseButton mouseButton = MouseButton.None;
			if (e.LeftMouseButtonClick) {
				mouseButton = MouseButton.Left;
			}
			if (e.RightMouseButtonClick) {
				mouseButton |= MouseButton.Right;
			}

			MouseButtonAndKey key = MouseButtonAndKey.None;
			if (e.CtrlClick) {
				key = MouseButtonAndKey.Control;
			}
			if (e.ShiftClick) {
				key |= MouseButtonAndKey.Shift;
			}

			IInSimDriver driver = null;
			Debug.Assert(_carAndDriverManager!=null);
			if (_carAndDriverManager!=null) {
				driver = _carAndDriverManager.GetDriverByConnectionID(e.ConnectionId);
			}

			button.OnClick(new ButtonClickEventArgs(mouseButton, key, e.Local, driver));
		}

		private void _inSimHandler_ButtonType(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.ButtonType e)
		{
			_log.FatalFormat("ButtonType, not implemented. ConnectionId '{0}', ButtonId '{1}', RequestId '{2}', Text '{3}'", e.ConnectionId, e.ButtonId, e.RequestId, e.Text);
		}

		private void _inSimHandler_RaceTrackConnectionLeave(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackConnectionLeave e)
		{
			ClearButonsOnClient(e.ConnectionId);
		}

		private Dictionary<Byte, Control> GetControlsForConnection(Byte connectionId)
		{
			Debug.Assert(_controlsPerConnection!=null);

			if (_controlsPerConnection.ContainsKey(connectionId)) {
				return _controlsPerConnection[connectionId];
			}

			return null;
		}

		private Dictionary<Byte, Control> GetOrCreateControlsForConnection(Byte connectionId)
		{
			Debug.Assert(_controlsPerConnection!=null);

			if (_controlsPerConnection.ContainsKey(connectionId)) {
				return _controlsPerConnection[connectionId];
			}

			_controlsPerConnection.Add(connectionId, new Dictionary<Byte,Control>(32));
			return _controlsPerConnection[connectionId];
		}

		private void ClearControls(bool dispose)
		{
			if (_controlsPerConnection==null) {
				return;
			}
			Int32 count = 0;
			foreach (Dictionary<Byte, Control> controls in _controlsPerConnection.Values) {
				foreach (Control control in controls.Values) {
					++count;
					control.OnDispose();
					control.Dispose();
				}
				controls.Clear();
			}

			_controlsPerConnection.Clear();
			_controlsPerConnection = null;

			if (dispose && count>0) {
				Debug.Assert(false, "There are undeleted controls!");
				_log.Error("There are undeleted controls");
			}
		}

		private void ClearButonsOnClient(Byte clientConnectionId)
		{
			Dictionary<Byte, Control> controls = GetControlsForConnection(clientConnectionId);
			if (controls==null) {
				return;
			}

			if (_logDebug) {
				_log.Debug(String.Format("Clearing all controls for client: '{0}'", clientConnectionId));
			}

			foreach (Control control in controls.Values) {
				Debug.Assert(control.ConnectionId == clientConnectionId);
				control.OnDispose();
				control.Dispose();
			}

			_controlsPerConnection.Remove(clientConnectionId);
		}

		private Byte GenerateCustomId(Dictionary<Byte, Control> controls) {
			lock (this) {
				Byte id = 0;
				foreach(Control control in controls.Values) {
					if (id>=MAX_CUSTOM_BUTTONS_COUNT) {
						_log.Fatal("Insufficient resources");
						throw new Exception("Insufficient resources");
					}
					if (!controls.ContainsKey(id)) {
						break;
					}
					++id;
				}
				return id;
			}
		}

		private Byte GenerateCommonId(Dictionary<Byte, Control> controls) {
			lock (this) {
				Byte id = COMMON_CONTROLS_INITIAL_ID;
				foreach (Control control in controls.Values) {
					if (id >= MAX_BUTTONS_COUNT) {
						_log.Fatal("Insufficient resources");
						throw new Exception("Insufficient resources");
					}
					if (!controls.ContainsKey(id)) {
						break;
					}
					++id;
				}
				return id;
			}
		}

	}
}
