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

using FullMotion.LiveForSpeed.InSim.Enums;


namespace SR.CML.Core.InSimCommon.Controls
{
	public interface IControl : IDisposable
	{
		event EventHandler<EventArgs> Disposing;

		bool Visible
		{
			get;
			set;
		}

		Byte Left
		{
			get;
			set;
		}

		Byte Right
		{
			get;
			set;
		}

		Byte Top
		{
			get;
			set;
		}

		Byte Bottom
		{
			get;
			set;
		}

		Byte Width
		{
			get;
			set;
		}

		Byte Height
		{
			get;
			set;
		}

		ButtonColor Color
		{
			get;
			set;
		}

		ButtonTextColor TextColor
		{
			get;
			set;
		}

		ButtonTextAlignment TextAlignment
		{
			get;
			set;
		}

		bool IsEmpty
		{
			get;
		}

		Object Tag {
			get;
			set;
		}

		void Show();
		void Hide();
		void Delete();
	}
}
