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
	public class CmlPlugins
	{
		private static Guid _CMLCore = new Guid("536550BD-BE60-4fd2-91B7-098FCF43F5B1");
		public static Guid CMLCoreGuid
		{
			get { return _CMLCore; }
		}
		public static String CMLCoreStringGuid
		{
			get { return _CMLCore.ToString(); }
		}

		private static Guid _ServerSetting = new Guid("27253220-5A44-4d8f-A1D6-D52A0FF060FB");
		public static Guid ServerSettingGuid
		{
			get { return _ServerSetting; }
		}
		public static String ServerSettingStringGuid
		{
			get { return _ServerSetting.ToString(); }
		}

		private static Guid _controlFactory = new Guid("46B960EA-864B-467e-9AC6-43E884185692");
		public static Guid ControlFactoryGuid
		{
			get { return _controlFactory; }
		}
		public static String ControlFactoryStringGuid
		{
			get { return _controlFactory.ToString(); }
		}

		private static Guid _messaging = new Guid("863E4F77-94E9-43fb-8A46-323220837A0D");
		public static Guid MessagingGuid
		{
			get { return _messaging; }
		}
		public static String MessagingStringGuid
		{
			get { return _messaging.ToString(); }
		}

		private static Guid _carDriverManager = new Guid("F7BFDAE0-0C5E-4dfe-84EC-E4576F8464E3");
		public static Guid CarDriverManagerGuid
		{
			get { return _carDriverManager; }
		}
		public static String CarDriverManagerStringGuid
		{
			get { return _carDriverManager.ToString(); }
		}

		private static Guid _raceDirector = new Guid("AFACAC8A-B63D-4006-940B-4C31999ECE37");
		public static Guid RaceDirectorGuid
		{
			get { return _raceDirector; }
		}
		public static String RaceDirectorStringGuid
		{
			get { return _raceDirector.ToString(); }
		}

		private static Guid _resultManager = new Guid("6301C0A4-8FFE-4baf-9569-07753A268AAC");
		public static Guid ResultManagerGuid
		{
			get { return _resultManager; }
		}
		public static String ResultManagerStringGuid
		{
			get { return _resultManager.ToString(); }
		}
	}
}
