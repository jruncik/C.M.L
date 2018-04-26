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

using SR.CML.CommonPlugins.Controls;
using SR.CML.CommonPlugins.CarDriverManager;
using SR.CML.CommonPlugins.Results;

using FullMotion.LiveForSpeed.InSim;
using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins.Plugins
{
	[Plugin("863E4F77-94E9-43fb-8A46-323220837A0D",
		"Messaging", "Supports messaging",
		new String[2]{	"536550BD-BE60-4fd2-91B7-098FCF43F5B1",		// CMLCoreGuid
						"46B960EA-864B-467e-9AC6-43E884185692"})]	// ControlFactoryGuid

	internal class PluginMessaging : PluginBase, IMessaging
	{
		private Messaging _messaging;

		public PluginMessaging()
		{
			_log		= LogManager.GetLogger(typeof(PluginMessaging));
			_logDebug	= _log.IsDebugEnabled;
			_messaging	= null;
		}

		protected override void InitializeInternal()
		{
			if (_messaging==null) {
				_messaging = new Messaging(_pluginManager);
			}
		}

		protected override void ActivateInternal()
		{
			_messaging.Activate();
		}

		protected override void DeactivateInternal()
		{
			_messaging.Deactivate();
		}

		protected override void DisposeInternal()
		{
			if (_messaging!=null) {
				_messaging.Dispose();
				_messaging = null;
			}
		}

		#region IMessaging Members

		public void SendCommand(string command)
		{
			_messaging.SendCommand(command);
		}

		public void SendMessageToAll(string message)
		{
			_messaging.SendMessageToAll(message);
		}

		public void SendMessageToCar(IInSimCar car, string message)
		{
			_messaging.SendMessageToCar(car, message);
		}

		public void SendMessageToDriver(IInSimDriver driver, string message)
		{
			_messaging.SendMessageToDriver(driver, message);
		}

		public void SendRaceControlMessageToAll(string message, short seconds)
		{
			_messaging.SendRaceControlMessageToAll(message, seconds);
		}

		public void SendRaceControlMessageToCar(IInSimCar car, string message, short seconds)
		{
			_messaging.SendRaceControlMessageToCar(car, message, seconds);
		}

		public void SendControlMessageToAll(string message, short seconds)
		{
			_messaging.SendControlMessageToAll(message, seconds);
		}

		public void SendControlMessageToCar(IInSimCar car, string message, short seconds)
		{
			_messaging.SendControlMessageToCar(car, message, seconds);
		}

		#endregion
	}
}
