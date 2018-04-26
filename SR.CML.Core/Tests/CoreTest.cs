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

using SR.CML.Core;
using SR.CML.Core.Plugins;
using SR.CML.Core.InSimCommon;

using NUnit.Framework;

namespace SR.CML.Core.Tests
{
	[TestFixture]
	public class CoreTest
	{
		private Core _core;

		[SetUp]
		public void InitTest()
		{
			_core = new Core(".\\");
		}

		[TearDown]
		public void Cleanup()
		{
			_core.Dispose();
			_core = null;
		}

		[Test]
		public void LoadedAllPlugins()
		{
			IPluginManager pluginManager = _core.PluginManager;

			Assert.AreEqual(6, pluginManager.Plugins.Count);

			Assert.NotNull(pluginManager.GetPlugin(new Guid("00000000-0000-0000-0001-000000000000")));	// Core

			Assert.NotNull(pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000001")));	// Plugin 1
			Assert.NotNull(pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000002")));	// Plugin 2
			Assert.NotNull(pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000003")));	// Plugin 3
			Assert.NotNull(pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000004")));	// Plugin 4
			Assert.NotNull(pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000005")));	// Plugin 5
		}

		[Test]
		public void PluginNotFound()
		{
			IPluginManager pluginManager = _core.PluginManager;
			IPlugin plugin = pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000000"));

			Assert.Null(plugin);
		}

		[Test]
		public void CorePlugin()
		{
			IPluginManager pluginManager = _core.PluginManager;
			IPlugin plugin = pluginManager.GetPlugin(new Guid("00000000-0000-0000-0001-000000000000"));

			Assert.NotNull(plugin);
		}

		[Test]
		public void ActivatePlugin1()
		{
			IPluginManager pluginManager = _core.PluginManager;

			Assert.True(pluginManager.ActivatePlugin(new Guid("00000000-0000-0000-0000-000000000001")));

			//plugin1 Depends On Core
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0001-000000000000")).State);
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000001")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000002")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000003")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000004")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000005")).State);
		}

		[Test]
		public void ActivatePlugin4()
		{
			IPluginManager pluginManager = _core.PluginManager;

			Assert.True(pluginManager.ActivatePlugin(new Guid("00000000-0000-0000-0000-000000000004")));

			//plugin4 Depends On 3 And Core, plugin3 Depends On 1
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0001-000000000000")).State);
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000001")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000002")).State);
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000003")).State);
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000004")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000005")).State);
		}

		[Test]
		public void ActivatePlugin5()
		{
			IPluginManager pluginManager = _core.PluginManager;

			Assert.True(pluginManager.ActivatePlugin(new Guid("00000000-0000-0000-0000-000000000005")));

			//plugin5 Depends On 3 And 1 And Core, plugin3 Depends On 1
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0001-000000000000")).State);
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000001")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000002")).State);
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000003")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000004")).State);
			Assert.AreEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000005")).State);
		}

		[Test]
		public void DeactivateAll()
		{
			IPluginManager pluginManager = _core.PluginManager;

			Assert.True(pluginManager.ActivatePlugin(new Guid("00000000-0000-0000-0000-000000000005")));
			pluginManager.DeactivateAll();

			//plugin5 Depends On 3 And 1 And Core, plugin3 Depends On 1
			Assert.AreEqual(PluginState.Deactivated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0001-000000000000")).State);
			Assert.AreEqual(PluginState.Deactivated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000001")).State);
            Assert.AreEqual(PluginState.Undefined, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000002")).State);
            Assert.AreEqual(PluginState.Deactivated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000003")).State);
            Assert.AreEqual(PluginState.Undefined, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000004")).State);
			Assert.AreEqual(PluginState.Deactivated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000005")).State);
		}

		[Test]
		public void ActivateUnknownPlugin()
		{
			IPluginManager pluginManager = _core.PluginManager;

			Assert.False(pluginManager.ActivatePlugin(new Guid("00000000-0000-0000-0000-000000000000")));

			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0001-000000000000")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000001")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000002")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000003")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000004")).State);
			Assert.AreNotEqual(PluginState.Activated, pluginManager.GetPlugin(new Guid("00000000-0000-0000-0000-000000000005")).State);
		}
	}
}
