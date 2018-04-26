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

namespace SR.CML.Core.InSimCommon
{
	[Flags]
	public enum MouseButton
	{
		None	= 0x00,
		Left	= 0x01,
		Middle	= 0x02,
		Right	= 0x04
	}

	[Flags]
	public enum MouseButtonAndKey
	{
		None	= 0x00,
		Control	= 0x01,
		Shift	= 0x02
	}
}
