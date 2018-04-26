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
using System.Xml;
using System.Xml.Serialization;

namespace SR.CML.Common
{
	[Serializable]
	public class Driver
	{
		private Car _car;
		[XmlIgnore]
		public Car Car {
			get { return _car; }
			set { _car = value; }
		}

		private String _lfsName;
		public String LfsName {
			get { return _lfsName; }
			set { _lfsName = value; }
		}
	}
}
