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


namespace SR.CML.Common
{
	[Serializable]
	public class EventConfiguration
	{
		private String _serverPassword;
		public String ServerPassword {
			get { return _serverPassword; }
			set { _serverPassword = value; }
		}

		LfsTrack _track;
		public LfsTrack Track {
			get { return _track; }
			set { _track = value; }
		}

		LfsWheather _wheather;
		public LfsWheather Wheather {
			get { return _wheather; }
			set { _wheather = value; }
		}

		LfsWind _wind;
		public LfsWind Wind {
			get { return _wind; }
			set { _wind = value; }
		}

		//EventType _eventType;
		//public EventType EventType {
		//    get { return _eventType; }
		//    set { _eventType = value; }
		//}

		//Int32 _laps;
		//public Int32 Laps {
		//    get { return _laps; }
		//    set { _laps = value; }
		//}

		Int32 _maxCars;
		public Int32 MaxCars {
			get { return _maxCars; }
			set { _maxCars = value; }
		}

		//TimeSpan _hours;
		//public TimeSpan Hours {
		//    get { return _hours; }
		//    set { _hours = value; }
		//}
	}
}
