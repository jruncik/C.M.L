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
using System.Reflection;
using System.Collections.Generic;

using SR.CML.Core.Actions;
using SR.CML.Core.Plugins;

using NUnit.Framework;

namespace SR.CML.Core.Tests
{
	[TestFixture]
	public class TreeTraverserTest
	{
		private static String PluginClassTypeName = "SR.CML.Core.Tests.TestPlugin";
		private static String Core = "Core";
		private static String Plugin_1 = "Plugin_1";
		private static String Plugin_2 = "Plugin_2";
		private static String Plugin_3 = "Plugin_3";
		private static String Plugin_4 = "Plugin_4";
		private static String Plugin_5 = "Plugin_5";

		private PluginHolder		_rootPlugin;
		private IList<PluginHolder>	_plugins;

		[SetUp]
		public void InitTest()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			_plugins = new List<PluginHolder>();

			PluginHolder pluginCore = new PluginHolder(assembly, PluginClassTypeName,
				new PluginAttribute("00000000-0000-0000-0001-000000000000", Core, "Core plugin desc", new String[0]{}));
			_plugins.Add(pluginCore);

			PluginHolder plugin1DependsOnCore = new PluginHolder(assembly, PluginClassTypeName,
				new PluginAttribute("00000000-0000-0000-0000-000000000001", Plugin_1, "Plugin 1 depends on Core", new String[1]{"00000000-0000-0000-0001-000000000000"}));
			_plugins.Add(plugin1DependsOnCore);

			PluginHolder plugin2DependsOnCore = new PluginHolder(assembly, PluginClassTypeName,
				new PluginAttribute("00000000-0000-0000-0000-000000000002", Plugin_2, "Plugin 2 depends on Core", new String[1]{"00000000-0000-0000-0001-000000000000"}));
			_plugins.Add(plugin2DependsOnCore);

			PluginHolder plugin3DependsOn1 = new PluginHolder(assembly, PluginClassTypeName,
				new PluginAttribute("00000000-0000-0000-0000-000000000003", Plugin_3, "Plugin 3 depends on 1", new String[1]{"00000000-0000-0000-0000-000000000001"}));
			_plugins.Add(plugin3DependsOn1);

			PluginHolder plugin4DependsOn3AndCore = new PluginHolder(assembly, PluginClassTypeName,
				new PluginAttribute("00000000-0000-0000-0000-000000000004", Plugin_4, "Plugin 4 depends on Core, 3", new String[2]{"00000000-0000-0000-0000-000000000003", "00000000-0000-0000-0001-000000000000"}));
			_plugins.Add(plugin4DependsOn3AndCore);

			PluginHolder plugin5DependsOn3And1AndCore = new PluginHolder(assembly, PluginClassTypeName,
				new PluginAttribute("00000000-0000-0000-0000-000000000005", Plugin_5, "Plugin 5 depends on Core, 3, 1", new String[3]{"00000000-0000-0000-0000-000000000003", "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0001-000000000000"}));
			_plugins.Add(plugin5DependsOn3And1AndCore);

			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(pluginCore);
			resolver.RegisterPlugin(plugin1DependsOnCore);
			resolver.RegisterPlugin(plugin2DependsOnCore);
			resolver.RegisterPlugin(plugin3DependsOn1);
			resolver.RegisterPlugin(plugin4DependsOn3AndCore);
			resolver.RegisterPlugin(plugin5DependsOn3And1AndCore);

			resolver.ResolveDependancy();

			_rootPlugin = resolver.RootPlugin;
		}

		[TearDown]
		public void Cleanup()
		{
			foreach (PluginHolder holder in _plugins) {
				holder.Dispose();
			}
			_plugins.Clear();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TraverseNullActionExceptionTop()
		{
			PluginTreeTraverser traverser = new PluginTreeTraverser(_rootPlugin);

			traverser.TraverseFromTop(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TraverseNullActionExceptionBottom()
		{
			PluginTreeTraverser traverser = new PluginTreeTraverser(_rootPlugin);

			traverser.TraverseFromBottom(null);
		}

		[Test]
		public void TraverseFromTop()
		{
			PluginTreeTraverser traverser = new PluginTreeTraverser(_rootPlugin);
			PluginsCollectorAction collector = new PluginsCollectorAction();

			traverser.TraverseFromTop(collector);

			Assert.AreEqual(6, collector.Plugins.Count);
			Assert.True(collector.EachPluginCalledOnyOnce);

			Assert.AreEqual(Core, collector.Plugins[0].Name);
			Assert.AreEqual(Plugin_1, collector.Plugins[1].Name);
			Assert.AreEqual(Plugin_2, collector.Plugins[2].Name);
			Assert.AreEqual(Plugin_3, collector.Plugins[3].Name);
			Assert.AreEqual(Plugin_4, collector.Plugins[4].Name);
			Assert.AreEqual(Plugin_5, collector.Plugins[5].Name);
		}

		[Test]
		public void TraverseFromBottom()
		{
			PluginTreeTraverser traverser = new PluginTreeTraverser(_rootPlugin);
			PluginsCollectorAction collector = new PluginsCollectorAction();

			traverser.TraverseFromBottom(collector);

			Assert.AreEqual(6, collector.Plugins.Count);
			Assert.True(collector.EachPluginCalledOnyOnce);

			Assert.AreEqual(Plugin_4, collector.Plugins[0].Name);
			Assert.AreEqual(Plugin_5, collector.Plugins[1].Name);
			Assert.AreEqual(Plugin_3, collector.Plugins[2].Name);
			Assert.AreEqual(Plugin_1, collector.Plugins[3].Name);
			Assert.AreEqual(Plugin_2, collector.Plugins[4].Name);
			Assert.AreEqual(Core, collector.Plugins[5].Name);
		}
	}
}
