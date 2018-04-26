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

using SR.CML.Core.InSimCommon;

namespace SR.CML.Common
{
	[Serializable]
	public class Host
	{
		private String _ip;
		public String IP {
			get { return _ip; }
			set { _ip = value; }
		}

		private String _adminisratorPassword;
		public String AdminisratorPassword {
			get { return _adminisratorPassword; }
			set { _adminisratorPassword = value; }
		}

		private UInt16 _inSimPort;
		public UInt16 InSimPort {
			get { return _inSimPort; }
			set { _inSimPort = value; }
		}

		private CMLMode _cmlMode;
		public CMLMode CmlMode {
			get { return _cmlMode; }
			set { _cmlMode = value; }
		}

		public Host() {
			_ip						= "127.0.0.1";
			_inSimPort				= 29999;
			_adminisratorPassword	= String.Empty;
			_cmlMode				= CMLMode.Simple;
		}
	}
}
