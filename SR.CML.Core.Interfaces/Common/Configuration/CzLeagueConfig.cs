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
using System.Xml.Serialization;
using System.Diagnostics;

namespace SR.CML.Common
{
	[Serializable]
	public class CzLeagueConfig
	{
		private Int32 _qualifyMin;
		public Int32 QualifyMin {
			get { return _qualifyMin; }
			set { _qualifyMin = value; }
		}

		private Int32 _raceLaps;
		public Int32 RaceLaps {
			get { return _raceLaps; }
			set { _raceLaps = value; }
		}

		private List<String> _admins;
		[XmlArray("Admins")]
		[XmlArrayItem("AdminLfsName", typeof(String))]
		public List<String> Admins {
			get { return _admins; }
			set { _admins = value; }
		}

		private Int32 _swapFirstDrivers;
		public Int32 SwapFirstDrivers
		{
			get { return _swapFirstDrivers; }
			set { _swapFirstDrivers = value; }
		}

		public CzLeagueConfig() {
			_admins = new List<String>();
		}
	}
}
