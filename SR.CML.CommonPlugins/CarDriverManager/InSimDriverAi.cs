/* ---------------------------------------------------------------------- *
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
	internal class InSimDriverAi : IInSimDriverAi
	{
		private class InSimDriverAiEmpty :InSimDriverAi
		{
			public override bool IsEmpty
			{
				get { return true; }
			}
		}

		private static ILog _log		= LogManager.GetLogger(typeof(InSimDriverAi));
		private static bool _logDebug	= _log.IsDebugEnabled;

		private static IInSimDriverAi _empty	= null;
		internal IInSimDriverAi Empty
		{
			get {
				if (_empty==null) {
					_empty = new InSimDriverAiEmpty();
				}
				return _empty;
			}
		}

		private String	_name;
		private Byte	_playerId;

		private bool _usedInGrid;
		internal bool UsedInGrid
		{
			get { return _usedInGrid; }
			set { _usedInGrid = value; }
		}

		private InSimDriverAi() : this(String.Empty, 0)
		{
		}

		internal InSimDriverAi(String name, Byte playerId)
		{
			_name		= name;
			_playerId	= playerId;
			_usedInGrid = false;
		}

		#region InsimaDriverAi

		public String Name
		{
			get { return _name; }
		}

		public Byte PlayerId
		{
			get { return _playerId; }
		}

		public virtual bool IsEmpty
		{
			get { return false; }
		}

		#endregion
	}
}
