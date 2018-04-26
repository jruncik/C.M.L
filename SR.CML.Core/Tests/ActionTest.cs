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
	public class ActionTest
	{
		private static String PluginClassTypeName = "SR.CML.Core.Tests.Plugins.TestPluginBase";
		private static String Core = "Core";
		private static String Plugin_1 = "Plugin_1";
		private static String Plugin_2 = "Plugin_2";
		private static String Plugin_3 = "Plugin_3";
		private static String Plugin_4 = "Plugin_4";
		private static String Plugin_5 = "Plugin_5";

		private PluginTreeTraverser	_traverser;
		private PluginManager		_pluginManager;
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

			_traverser		= new PluginTreeTraverser(resolver.RootPlugin);
			_pluginManager	= new PluginManager("");

			InstanciateAndInitializePlugins();
		}

		[TearDown]
		public void Cleanup()
		{
			foreach (PluginHolder holder in _plugins) {
				holder.Dispose();
			}
			_plugins.Clear();

			_traverser = null;

			_pluginManager.Dispose();
			_pluginManager = null;
		}

		[Test]
		[ExpectedException(typeof(CreateInstanceException))]
		public void CreateInstanceFaild()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			using (PluginHolder pluginCore = new PluginHolder(assembly, "UnknownClass",
				new PluginAttribute("00000000-0000-0000-0001-000000000000", Core, "Core plugin desc", new String[0]{}))
			) {

				PluginDependancyResolver resolver = new PluginDependancyResolver();
				resolver.RegisterPlugin(pluginCore);

				resolver.ResolveDependancy();

				PluginTreeTraverser traverser = new PluginTreeTraverser(resolver.RootPlugin);

				InstanciateAction instanciateAction = new InstanciateAction();
				traverser.TraverseFromTop(instanciateAction);	// throws CreateInstanceException
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InitializeFaild()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			using (	PluginHolder pluginCore = new PluginHolder(assembly, PluginClassTypeName,
				new PluginAttribute("00000000-0000-0000-0001-000000000000", Core, "Core plugin desc", new String[0]{}))
			) {
				PluginDependancyResolver resolver = new PluginDependancyResolver();
				resolver.RegisterPlugin(pluginCore);

				resolver.ResolveDependancy();

				PluginTreeTraverser traverser = new PluginTreeTraverser(resolver.RootPlugin);

				InstanciateAction instanciateAction = new InstanciateAction();
				traverser.TraverseFromTop(instanciateAction);

				InitializeAction initializeAction = new InitializeAction(null);	// throws ArgumentNullException
			}
		}

		[Test]
		public void ActivateAll()
		{
			ActivateAction activate = new ActivateAction(true);
			_traverser.TraverseFromTop(activate);

			CheckPluginsState(PluginState.Activated);
		}

		[Test]
		public void DeactivateAll()
		{
			ActivateAction activate = new ActivateAction(true);
			_traverser.TraverseFromTop(activate);

			CheckPluginsState(PluginState.Activated);

			DeactivateAllAction deactivate = new DeactivateAllAction();
			_traverser.TraverseFromBottom(deactivate);

			CheckPluginsState(PluginState.Deactivated);
		}

		#region Helpers

		private void InstanciateAndInitializePlugins()
		{
			InstanciateAction instanciateAction = new InstanciateAction();
			_traverser.TraverseFromTop(instanciateAction);

			CheckPluginsState(PluginState.Undefined);

			InitializeAction initializeAction = new InitializeAction(_pluginManager, true);
			_traverser.TraverseFromTop(initializeAction);

			CheckPluginsState(PluginState.Initialized);
		}

		private void CheckPluginsState(PluginState state)
		{
			PluginsCollectorAction collector = new PluginsCollectorAction();
			_traverser.TraverseFromTop(collector);

			foreach(PluginHolder item in collector.Plugins) {
				Assert.AreEqual(state, item.Plugin.State);
			}
		}
		#endregion
	}
}
