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
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using SR.CML.Core.Plugins;

using log4net;

using System.Diagnostics;

namespace SR.CML.Core
{
	public class PluginExplorer : MarshalByRefObject
	{
		private static ILog _log = LogManager.GetLogger(typeof(PluginExplorer));

		public List<PluginDescription> GetPluginList()
		{
			List<PluginDescription>	result			= new List<PluginDescription>(16);
			Type					pluginnAttrType	= typeof(PluginAttribute);
			DirectoryInfo			dictionaryIno	= new DirectoryInfo(AppDomain.CurrentDomain.RelativeSearchPath);
			FileInfo[]				rgFiles			= dictionaryIno.GetFiles("*.dll");
			bool					anyPluginFound	= false;
			Assembly				assembly		= null;
			PluginAttribute			pluginAttribute	= null;

			foreach (FileInfo fileInfo in rgFiles) {
				assembly		= Assembly.LoadFrom(fileInfo.FullName);
				anyPluginFound	= false;

				try {
					foreach (Type type in assembly.GetTypes()) {
						foreach (Attribute attribute in Attribute.GetCustomAttributes(type)) {

							if (attribute.GetType() == pluginnAttrType) {
								pluginAttribute = attribute as PluginAttribute;

								Debug.Assert(pluginAttribute!=null);
								if (pluginAttribute!=null) {
									result.Add(new PluginDescription(fileInfo.FullName));
									anyPluginFound = true;
									break;
								}
							}
						}
						if (anyPluginFound) {
							break;
						}
					}

				} catch (Exception ex) {
					Debug.Assert(false, ex.Message);
					_log.Error(ex.Message);
				}
			}
			return result;
		}
	}
}
