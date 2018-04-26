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
	public class CarType
	{
		private Guid _id;
		public Guid Id {
			get { return _id; }
			set { _id = value; }
		}

		private String _name;
		public String Name {
			get { return _name; }
			set { _name = value; }
		}

		private LfsCarType _lfsCarType;
		public LfsCarType LfsCarType {
			get { return _lfsCarType; }
			set { _lfsCarType = value; }
		}

		private CarRestriction _carRestriction;
		public CarRestriction CarRestriction {
			get { return _carRestriction; }
			set { _carRestriction = value; }
		}

		public CarType() :
			this(String.Empty, LfsCarType.None, 0, 0) {
		}

		public CarType(String name, LfsCarType lfsCarType)
			: this(name, lfsCarType, 0, 0) {
		}

		public CarType(String name, LfsCarType lfsCarType, Byte intakeRestriction)
			: this(name, lfsCarType, intakeRestriction, 0) {
		}

		public CarType(String name, LfsCarType lfsCarType, Int32 addedMass)
			: this(name, lfsCarType, 0, addedMass) {
		}

		public CarType(String name, LfsCarType lfsCarType, Byte intakeRestriction, Int32 addedMass) {
			_name			= name;
			_lfsCarType		= lfsCarType;
			_carRestriction	= new CarRestriction();
			_id				= Guid.NewGuid();

			_carRestriction.HandicapIntakeRestriction	= intakeRestriction;
			_carRestriction.HandicapMass				= (Byte)addedMass;
		}
	}
}
