/* ------------------------------------------------------------------------- *
 * Copyright (C) 2005-2008 Arne Claassen
 *
 * Arne Claassen <lfslib [at] claassen [dot] net>
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
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * ------------------------------------------------------------------------- */
using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace FullMotion.LiveForSpeed.InSim
{
	/// <summary>
	/// ConfigurationHandler is never directly invoked. It is however required for registering
	/// your Configuration Xml fragment in app.config or web.config. To register the xml fragment
  /// of &lt;lfsinsim/&gt;, add the following to your configuration, followed by the actual config:
	/// <code>
	/// &lt;configSections&gt;
	///   &lt;section name="lfsinsim" type="FullMotion.LiveForSpeed.InSim.ConfigurationHandler,LFSLib" /&gt;
	/// &lt;/configSections&gt;
	/// </code>
	/// </summary>
	public class ConfigurationHandler : IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler Members
		/// <summary>
		/// See <see cref="IConfigurationSectionHandler.Create"/>
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
      XmlSerializer ser = new XmlSerializer(typeof(FullMotion.LiveForSpeed.InSim.Configuration.LfsInSim));
			XmlNodeReader xNodeReader = new XmlNodeReader(section);
			return ser.Deserialize(xNodeReader);
		}
		#endregion
	}
}
