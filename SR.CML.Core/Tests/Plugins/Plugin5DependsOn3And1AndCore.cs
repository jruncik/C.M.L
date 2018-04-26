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
	[Plugin("00000000-0000-0000-0000-000000000005",
			"Plugin_5", "Plugin 5 depends on 1 and 1 and Core",
			new String[3]{"00000000-0000-0000-0000-000000000003", "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0001-000000000000"})]
	class Plugin5DependsOn3And1AndCore : TestPluginBase
	{
	}
}
