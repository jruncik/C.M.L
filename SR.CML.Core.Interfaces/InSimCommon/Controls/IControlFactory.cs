﻿/* ------------------------------------------------------------------------- *
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


namespace SR.CML.Core.InSimCommon.Controls
{
	// PluginId {46B960EA-864B-467e-9AC6-43E884185692}
	public interface IControlFactory : IDisposable
	{
		IButton CreateButton();
		IButton CreateButton(IInSimDriver driver);

		ILabel CreateLabel(IInSimDriver driver);
		ILabel CreateLabel();

		ICountDown CreateCountDown();

		IEditBox CreateEditBox(IInSimDriver driver);
	}
}
