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
using System.Threading;

using SR.CML.Common;

using SR.CML.Core;
using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;

using log4net;

using System.Collections.Generic;

using System.Diagnostics;

namespace SR.CML.Test
{
	static class CMLTest
	{
		private static ILog _log		= LogManager.GetLogger(typeof(CMLTest));

		[STAThread]
		static void Main()
		{
			_log.Debug("Starting");
			SR.CML.Core.Core core = new SR.CML.Core.Core(@"Plugins");

			try {
				ConfigureServer(core);
				ShowMenu();
				Run(core);
				_log.Debug("Finished");

			} catch (Exception ex) {
				_log.Fatal(ex);

			} finally {
				core.Dispose();
				core = null;
				_log.Debug("Disposed");
			}
		}

		private static CmlConfiguration CreateConfiguration() {
			return CmlConfiguration.CreateFromFile("config.xml");
		}

		private static void ConfigureServer(SR.CML.Core.Core core) {
			CmlConfiguration config = CreateConfiguration();

			IServerSetting serverSetting = core.PluginManager.GetPlugin(CmlPlugins.ServerSettingGuid) as IServerSetting;
			if (serverSetting != null) {
				serverSetting.Config = config;
			}
		}

		private static ConsoleKeyInfo Run(SR.CML.Core.Core core) {
			ConsoleKeyInfo	cki;
			bool			quit = false;
			do {
				cki = Console.ReadKey(true);

				switch (cki.Key) {
					case ConsoleKey.A: {
						Console.WriteLine("Activating...");
						//_core.PluginManager.ActivatePlugin(new Guid("6455756F-C4EB-45b1-BBA4-2C491DD97351"));	// RallyCross
						//_core.PluginManager.ActivatePlugin(new Guid("58FD59FF-5B43-4f4b-94F1-7D1801E8815D"));	// Say hello
						core.PluginManager.ActivatePlugin(new Guid("4D756233-69DF-4a65-8D85-DAA23B168318"));	// Cz League
						//_core.PluginManager.ActivatePlugin(CmlPlugins.CarDriverManagerGuid);
					}
					break;

					case ConsoleKey.D: {
						Console.WriteLine("Deactivating...");
						core.PluginManager.DeactivateAll();
					}
					break;

					case ConsoleKey.Escape: {
						Console.WriteLine("Exit");
						quit = true;
					}
					break;
				}
			} while (!quit);
			return cki;
		}

		private static void ShowMenu() {
			Console.WriteLine("EGT - CML v 0.1");
			Console.WriteLine("---------------");
			Console.WriteLine("A   - activate");
			Console.WriteLine("D   - deactivate");
			Console.WriteLine("Esc - exit");
			Console.WriteLine(String.Empty);
		}
	}
}
