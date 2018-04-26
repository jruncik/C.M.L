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

using System.Diagnostics;

namespace SR.CML.Core.Plugins
{
	public class PluginAttribute : Attribute
	{
		private Guid _id;
		public Guid Id
		{
			get { return _id; }
		}

		private String _name;
		public String Name
		{
			get { return _name; }
		}

		private String _description;
		public String Description
		{
			get { return _description; }
		}

		private IList<Guid> _dependency = null;
		public IList<Guid> Dependency
		{
			get { return _dependency; }
		}

		public PluginAttribute(String id, String name, String description, String[] dependency)
		{
			Debug.Assert(dependency!=null);
			Debug.Assert(!String.IsNullOrEmpty(id));
			Debug.Assert(!String.IsNullOrEmpty(name));
			Debug.Assert(!String.IsNullOrEmpty(description));

			_id				= new Guid(id);
			_name			= name;
			_description	= description;

			_dependency		= new List<Guid>(dependency.Length);
			foreach (String sGuid in dependency) {
				_dependency.Add(new Guid(sGuid));
			}
		}
	}
}
