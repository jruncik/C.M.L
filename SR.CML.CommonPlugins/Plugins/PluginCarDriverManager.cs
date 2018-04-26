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

using SR.CML.CommonPlugins.CarDriverManager;

using FullMotion.LiveForSpeed.InSim;
using log4net;

using System.Diagnostics;

namespace SR.CML.CommonPlugins
{
	[Plugin("F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3",
			"CarDrivermanager", "Handle cars and drivers",
			new String[2]{	"863E4F77-94E9-43fb-8A46-323220837A0D",		// MessagingGuid
							"536550BD-BE60-4fd2-91B7-098FCF43F5B1"})]	// CMLCoreGuid

	internal class PluginCarDriverManager : PluginBase, ICarDriverManager
	{
		private CarDriverManagerBase	_carDriverManager;
		private CMLMode					_mode;

		public PluginCarDriverManager()
		{
			_log		= LogManager.GetLogger(typeof(PluginCarDriverManager));
			_logDebug	= _log.IsDebugEnabled;

			_mode				= CMLMode.None;
			_carDriverManager	= null;
		}

		protected override void InitializeInternal()
		{
			_mode = CMLMode.Simple;

			if (_carDriverManager==null) {
				_carDriverManager = CarDriverManagerBase.CreateCarDriverManager(_pluginManager, _mode);
			}
		}

		protected override void ActivateInternal()
		{
			_carDriverManager.Activate();
			BindEvents();
		}

		protected override void DeactivateInternal()
		{
			_carDriverManager.Deactivate();
			UnbindEvents();
		}

		protected override void DisposeInternal()
		{
			if (_carDriverManager!=null) {
				_carDriverManager.Dispose();
				_carDriverManager = null;
			}
		}

		#region ICarDriverManager Members

		public event EventHandler<DriverStateEventArgs> DriverStateChanged;

		public event EventHandler<AiDriverStateChangedEventArgs> AiDriverStateChanged;

		public event EventHandler<CarStateChangedEventArgs> CarStateChanged;

		public IInSimCar GetCarByLFSName(String name)
		{
			return _carDriverManager.GetCarByLFSName(name);
		}

		public IInSimCar GetCarByPlayerID(Byte playerId)
		{
			return _carDriverManager.GetCarByPlayerID(playerId);
		}

		public IInSimCar GetCarByConnectionID(Byte connectionId)
		{
			return _carDriverManager.GetCarByConnectionID(connectionId);
		}

		public IInSimDriver GetDriverByConnectionID(Byte connectinId) {
			return _carDriverManager.GetDriverByConnectionID(connectinId);
		}

		public IList<IInSimDriverAi> GetAiDrivers()
		{
			return _carDriverManager.GetAiDrivers();
		}

		public void AddAiDriver()
		{
			_carDriverManager.AddAiDriver();
		}

		public void RemoveAllAiDrivers()
		{
			_carDriverManager.RemoveAllAiDrivers();
		}

		public bool IsAiDriver(byte driverId)
		{
			return _carDriverManager.IsAiDriver(driverId);
		}

		public IList<IInSimCar> Cars
		{
			get { return _carDriverManager.Cars; }
		}

		public IList<IInSimDriver> Drivers
		{
			get { return _carDriverManager.Drivers; }
		}

		#endregion

		private void BindEvents()
		{
			Debug.Assert(_carDriverManager!=null);

			_carDriverManager.AiDriverStateChanged	+= new EventHandler<AiDriverStateChangedEventArgs>(CarDriverManager_AiDriverStateChanged);
			_carDriverManager.CarStateChanged		+= new EventHandler<CarStateChangedEventArgs>(CarDriverManager_CarStateChanged);
			_carDriverManager.DriverStateChanged	+= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
		}

		private void UnbindEvents()
		{
			Debug.Assert(_carDriverManager!=null);

			_carDriverManager.AiDriverStateChanged	-= new EventHandler<AiDriverStateChangedEventArgs>(CarDriverManager_AiDriverStateChanged);
			_carDriverManager.CarStateChanged		-= new EventHandler<CarStateChangedEventArgs>(CarDriverManager_CarStateChanged);
			_carDriverManager.DriverStateChanged	-= new EventHandler<DriverStateEventArgs>(CarDriverManager_DriverStateChanged);
		}

		private void CarDriverManager_CarStateChanged(object sender, CarStateChangedEventArgs e)
		{
			if (CarStateChanged!=null) {
				CarStateChanged(sender, e);
			}
		}

		private void CarDriverManager_AiDriverStateChanged(object sender, AiDriverStateChangedEventArgs e)
		{
			if (AiDriverStateChanged!=null) {
				AiDriverStateChanged(sender, e);
			}
		}

		private void CarDriverManager_DriverStateChanged(object sender, DriverStateEventArgs e)
		{
			if (DriverStateChanged!=null) {
				DriverStateChanged(sender, e);
			}
		}
	}
}
