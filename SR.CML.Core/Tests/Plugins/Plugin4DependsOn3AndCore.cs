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

using SR.CML.Core.Plugins;

using System.Diagnostics;

namespace SR.CML.Core.Tests.Plugins
{
	[Plugin("00000000-0000-0000-0000-000000000004",
			"Plugin_4", "Plugin 4 depends on 3 and Core",
			new String[2]{"00000000-0000-0000-0001-000000000000", "00000000-0000-0000-0000-000000000003"})]
	class Plugin4DependsOn3AndCore : TestPluginBase
	{
	}
}
