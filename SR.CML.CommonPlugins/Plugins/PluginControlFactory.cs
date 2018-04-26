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
	[Plugin("46B960EA-864B-467e-9AC6-43E884185692",
		"ControlFactory", "Creates all UI controls.",
		new String[1]	{"536550BD-BE60-4fd2-91B7-098FCF43F5B1"})]	// CMLCoreGuid

		//new String[2]	{"536550BD-BE60-4fd2-91B7-098FCF43F5B1",	// CMLCoreGuid
		//				"F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3"})]	// CarDriverManagerGuid

	internal class PluginControlFactory : PluginBase, IControlFactory
	{
		private ControlFactory _controlFactory;

		public PluginControlFactory()
		{
			_log			= LogManager.GetLogger(typeof(PluginControlFactory));
			_logDebug		= _log.IsDebugEnabled;
			_controlFactory	= null;
		}

		protected override void InitializeInternal()
		{
			if (_controlFactory==null) {
				_controlFactory = new ControlFactory(_pluginManager);
			}
		}

		protected override void ActivateInternal()
		{
			_controlFactory.Activate();
		}

		protected override void DeactivateInternal()
		{
			_controlFactory.Deactivate();
		}

		protected override void DisposeInternal()
		{
			if (_controlFactory!=null) {
				_controlFactory.Dispose();
				_controlFactory = null;
			}
		}

		internal ICarDriverManager CarAndDriverManager {
			set { _controlFactory.CarAndDriverManager = value; }
		}

		#region IControlFactory Members

		public IButton CreateButton() {
			return _controlFactory.CreateButton();
		}

		public IButton CreateButton(IInSimDriver driver)
		{
			return _controlFactory.CreateButton(driver);
		}

		public ILabel CreateLabel(IInSimDriver driver)
		{
			return _controlFactory.CreateLabel(driver);
		}

		public ILabel CreateLabel()
		{
			return _controlFactory.CreateLabel();
		}

		public ICountDown CreateCountDown()
		{
			return _controlFactory.CreateCountDown();
		}

		public IEditBox CreateEditBox(IInSimDriver driver)
		{
			return _controlFactory.CreateEditBox(driver);
		}

		#endregion
	}
}
