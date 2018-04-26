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
	public class CarRestriction {

		private Handling _handling;
		public Handling Handling {
			get { return _handling; }
			set { _handling = value; }
		}

		private Byte _handicapIntakeRestriction;
		public Byte HandicapIntakeRestriction {
			get { return _handicapIntakeRestriction; }
			set {
				if (value > 50) {
					throw new ArgumentOutOfRangeException("Intake Restricion can be 0 - 50%");
				}
				_handicapIntakeRestriction = value;
			}
		}

		private Byte _handicapMass;
		public Byte HandicapMass {
			get { return _handicapMass; }
			set {
				if (value > 200) {
					throw new ArgumentOutOfRangeException("Added mass can be 0 - 200kg");
				}
				_handicapMass = value;
			}
		}

		public CarRestriction() {
			_handicapIntakeRestriction	= 0;
			_handicapMass				= 0;
			_handling					= Handling.None;
			
		}

		public static bool IsAutoGearsEnabled(Handling restriction) {
			return (restriction & Handling.AutoGears) == Handling.AutoGears;
		}

		public static bool IsHelpBrakeEnabled(Handling restriction) {
			return (restriction & Handling.HelpBrake) == Handling.HelpBrake;
		}

		public static bool IsAutoclutchEnabled(Handling restriction) {
			return (restriction & Handling.Autoclutch) == Handling.Autoclutch;
		}

		public static bool IsTractionConrolEnabled(Handling restriction) {
			return (restriction & Handling.TractionConrol) == Handling.TractionConrol;
		}

		public static bool IsAbsEnabled(Handling restriction) {
			return (restriction & Handling.AutoGears) == Handling.AutoGears;
		}

		public static bool IsShifterUses(Handling restriction) {
			return (restriction & Handling.Shifter) == Handling.Shifter;
		}

		private void Validate() {
			if (_handicapIntakeRestriction > 50) {
				throw new ArgumentOutOfRangeException("Intake Restriction can be 0 - 50%");
			}

			if (_handicapMass > 200) {
				throw new ArgumentOutOfRangeException("Added mass can be 0 - 200kg");
			}
		}
	}

	[Flags]
	public enum Handling
	{
		None			= 0x00,
		AutoGears		= 0x01,
		HelpBrake		= 0x02,
		Autoclutch		= 0x04,
		TractionConrol	= 0x08,
		Abs				= 0x10,
		Shifter			= 0x20
	}
}
