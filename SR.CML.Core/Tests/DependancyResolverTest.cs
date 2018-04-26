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

using SR.CML.Core.Plugins;

using NUnit.Framework;

namespace SR.CML.Core.Tests
{
	[TestFixture]
	public class DependancyResolverTest
	{
		private Assembly _assembly;

		private PluginHolder _pluginCore;
		private PluginHolder _pluginFakeCore;

		private PluginHolder _plugin1DependsOnCore;
		private PluginHolder _plugin2DependsOnCore;

		private PluginHolder _plugin3DependsOn1;
		private PluginHolder _plugin4DependsOn3AndCore;
		private PluginHolder _plugin5DependsOn3And1AndCore;

		private PluginHolder _plugin6Unresolved;
		private PluginHolder _plugin7Dependson1and6;

		[TestFixtureSetUp]
		public void InitTests()
		{
			_assembly = Assembly.GetExecutingAssembly();
		}

		[SetUp]
		public void InitTest()
		{
			_pluginCore = new PluginHolder(_assembly, "Core",
				new PluginAttribute("00000000-0000-0000-0001-000000000000", "Core", "Core plugin desc", new String[0]{}));

			_pluginFakeCore	= new PluginHolder(_assembly, "Fake_Core",
				new PluginAttribute("00000000-0000-0000-0002-000000000000", "FakeCore", "Fake Core plugin desc", new String[0]{}));


			_plugin1DependsOnCore = new PluginHolder(_assembly, "Plugin_1",
				new PluginAttribute("00000000-0000-0000-0000-000000000001", "Plugin_1", "Plugin 1 depends on Core", new String[1]{"00000000-0000-0000-0001-000000000000"}));

			_plugin2DependsOnCore = new PluginHolder(_assembly, "Plugin_2",
				new PluginAttribute("00000000-0000-0000-0000-000000000002", "Plugin_2", "Plugin 2 depends on Core", new String[1]{"00000000-0000-0000-0001-000000000000"}));

			_plugin3DependsOn1 = new PluginHolder(_assembly, "Plugin_3",
				new PluginAttribute("00000000-0000-0000-0000-000000000003", "Plugin_3", "Plugin 3 depends on 1", new String[1]{"00000000-0000-0000-0000-000000000001"}));

			_plugin4DependsOn3AndCore = new PluginHolder(_assembly, "Plugin_4",
				new PluginAttribute("00000000-0000-0000-0000-000000000004", "Plugin_4", "Plugin 4 depends on Core, 3", new String[2]{"00000000-0000-0000-0000-000000000003", "00000000-0000-0000-0001-000000000000"}));

			_plugin5DependsOn3And1AndCore = new PluginHolder(_assembly, "Plugin_5",
				new PluginAttribute("00000000-0000-0000-0000-000000000005", "Plugin_5", "Plugin 5 depends on Core, 3, 1", new String[3]{"00000000-0000-0000-0000-000000000003", "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0001-000000000000"}));

			_plugin6Unresolved = new PluginHolder(_assembly, "Plugin_6",
				new PluginAttribute("00000000-0000-0000-0000-000000000006", "Plugin_6", "Plugin 6 depends on Nothing", new String[1]{"00000000-0000-0000-0000-100000000000"}));

			_plugin7Dependson1and6 = new PluginHolder(_assembly, "Plugin_7",
				new PluginAttribute("00000000-0000-0000-0000-000000000007", "Plugin_7", "Plugin 7 depends on 1 and 6 ", new String[2]{"00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000006"}));
		}

		[TearDown]
		public void Cleanup()
		{
			_pluginCore.Dispose();
			_pluginFakeCore.Dispose();
			_plugin1DependsOnCore.Dispose();
			_plugin2DependsOnCore.Dispose();
			_plugin3DependsOn1.Dispose();
			_plugin4DependsOn3AndCore.Dispose();
			_plugin5DependsOn3And1AndCore.Dispose();
			_plugin6Unresolved.Dispose();
			_plugin7Dependson1and6.Dispose();
		}
		[Test]
		public void FindCorePlugin()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_pluginCore);

			Assert.True(resolver.ResolveDependancy());

			Assert.AreEqual(_pluginCore, resolver.RootPlugin);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TwoCorePluginException()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_pluginCore);
			resolver.RegisterPlugin(_pluginFakeCore);

			resolver.ResolveDependancy();
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void AnyCorePluginException()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_plugin1DependsOnCore);
			resolver.RegisterPlugin(_plugin2DependsOnCore);

			resolver.ResolveDependancy();
		}

		[Test]
		public void TwoDependsOnCorePlugin()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_pluginCore);
			resolver.RegisterPlugin(_plugin1DependsOnCore);
			resolver.RegisterPlugin(_plugin2DependsOnCore);

			Assert.True(resolver.ResolveDependancy());

			Assert.AreEqual(_pluginCore, _plugin1DependsOnCore.LatestDependsOn);
			Assert.AreEqual(_pluginCore, _plugin2DependsOnCore.LatestDependsOn);

			Assert.AreEqual(2, _pluginCore.Dependants.Count);
			Assert.True(Contains(_plugin1DependsOnCore, _pluginCore.Dependants));
			Assert.True(Contains(_plugin2DependsOnCore, _pluginCore.Dependants));
		}

		[Test]
		public void ComplexDependancyTest()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_pluginCore);
			resolver.RegisterPlugin(_plugin1DependsOnCore);
			resolver.RegisterPlugin(_plugin2DependsOnCore);
			resolver.RegisterPlugin(_plugin3DependsOn1);
			resolver.RegisterPlugin(_plugin4DependsOn3AndCore);
			resolver.RegisterPlugin(_plugin5DependsOn3And1AndCore);

			Assert.True(resolver.ResolveDependancy());

			TestLastDependsOn(resolver.RootPlugin);
			TestDependants();
			TestDependsOn();
		}

		[Test]
		public void ComplexDependancyTestRandom1()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_plugin2DependsOnCore);
			resolver.RegisterPlugin(_plugin4DependsOn3AndCore);
			resolver.RegisterPlugin(_plugin5DependsOn3And1AndCore);
			resolver.RegisterPlugin(_plugin3DependsOn1);
			resolver.RegisterPlugin(_pluginCore);
			resolver.RegisterPlugin(_plugin1DependsOnCore);

			Assert.True(resolver.ResolveDependancy());

			TestLastDependsOn(resolver.RootPlugin);
			TestDependants();
			TestDependsOn();
		}

		[Test]
		public void ComplexDependancyTestRandom2()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_plugin5DependsOn3And1AndCore);
			resolver.RegisterPlugin(_plugin2DependsOnCore);
			resolver.RegisterPlugin(_pluginCore);
			resolver.RegisterPlugin(_plugin3DependsOn1);
			resolver.RegisterPlugin(_plugin1DependsOnCore);
			resolver.RegisterPlugin(_plugin4DependsOn3AndCore);

			Assert.True(resolver.ResolveDependancy());

			TestLastDependsOn(resolver.RootPlugin);
			TestDependants();
			TestDependsOn();
		}

		[Test]
		public void ComplexDependancyTestRandom3Unresolved()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_plugin1DependsOnCore);
			resolver.RegisterPlugin(_plugin5DependsOn3And1AndCore);
			resolver.RegisterPlugin(_pluginCore);
			resolver.RegisterPlugin(_plugin6Unresolved);
			resolver.RegisterPlugin(_plugin2DependsOnCore);
			resolver.RegisterPlugin(_plugin3DependsOn1);
			resolver.RegisterPlugin(_plugin4DependsOn3AndCore);

			Assert.False(resolver.ResolveDependancy());

			TestLastDependsOn(resolver.RootPlugin);
			TestDependants();
			TestDependsOn();

			Assert.AreEqual(1, resolver.UnresolvedPlugins.Count);
			Assert.AreEqual(_plugin6Unresolved, resolver.UnresolvedPlugins[0]);
		}

		[Test]
		public void UnresolvedPlugin()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_pluginCore);
			resolver.RegisterPlugin(_plugin1DependsOnCore);
			resolver.RegisterPlugin(_plugin6Unresolved);

			Assert.False(resolver.ResolveDependancy());

			Assert.AreEqual(1, resolver.UnresolvedPlugins.Count);
			Assert.AreEqual(_plugin6Unresolved, resolver.UnresolvedPlugins[0]);
		}

		[Test]
		public void WaitongUnresolvedPlugin()
		{
			PluginDependancyResolver resolver = new PluginDependancyResolver();
			resolver.RegisterPlugin(_pluginCore);
			resolver.RegisterPlugin(_plugin1DependsOnCore);
			resolver.RegisterPlugin(_plugin6Unresolved);
			resolver.RegisterPlugin(_plugin7Dependson1and6);

			Assert.False(resolver.ResolveDependancy());

			Assert.AreEqual(2, resolver.UnresolvedPlugins.Count);
			Assert.AreEqual(_plugin7Dependson1and6, resolver.UnresolvedPlugins[0]);
			Assert.AreEqual(_plugin6Unresolved, resolver.UnresolvedPlugins[1]);
		}

		#region Helpers
		private void TestLastDependsOn(PluginHolder rootPlugin)
		{
			Assert.AreEqual(_pluginCore, rootPlugin);
			Assert.AreEqual(_pluginCore, _plugin1DependsOnCore.LatestDependsOn);
			Assert.AreEqual(_pluginCore, _plugin2DependsOnCore.LatestDependsOn);
			Assert.AreEqual(_plugin1DependsOnCore, _plugin3DependsOn1.LatestDependsOn);
			Assert.AreEqual(_plugin3DependsOn1, _plugin4DependsOn3AndCore.LatestDependsOn);
			Assert.AreEqual(_plugin3DependsOn1, _plugin5DependsOn3And1AndCore.LatestDependsOn);
		}

		private void TestDependsOn()
		{
			Assert.AreEqual(2, _plugin4DependsOn3AndCore.DependsOn.Count);
			Assert.True(Contains(_plugin3DependsOn1,	_plugin4DependsOn3AndCore.DependsOn));
			Assert.True(Contains(_pluginCore,			_plugin4DependsOn3AndCore.DependsOn));

			Assert.AreEqual(3, _plugin5DependsOn3And1AndCore.DependsOn.Count);
			Assert.True(Contains(_plugin3DependsOn1,	_plugin5DependsOn3And1AndCore.DependsOn));
			Assert.True(Contains(_plugin1DependsOnCore,	_plugin5DependsOn3And1AndCore.DependsOn));
			Assert.True(Contains(_pluginCore,			_plugin5DependsOn3And1AndCore.DependsOn));

			Assert.AreEqual(1, _plugin3DependsOn1.DependsOn.Count);
			Assert.True(Contains(_plugin1DependsOnCore, _plugin3DependsOn1.DependsOn));

			Assert.AreEqual(1, _plugin2DependsOnCore.DependsOn.Count);
			Assert.True(Contains(_pluginCore, _plugin2DependsOnCore.DependsOn));

			Assert.AreEqual(1, _plugin1DependsOnCore.DependsOn.Count);
			Assert.True(Contains(_pluginCore, _plugin1DependsOnCore.DependsOn));
		}

		private void TestDependants()
		{
			Assert.AreEqual(4, _pluginCore.Dependants.Count);
			Assert.True(Contains(_plugin1DependsOnCore, _pluginCore.Dependants));
			Assert.True(Contains(_plugin2DependsOnCore, _pluginCore.Dependants));
			Assert.True(Contains(_plugin4DependsOn3AndCore, _pluginCore.Dependants));
			Assert.True(Contains(_plugin5DependsOn3And1AndCore, _pluginCore.Dependants));

			Assert.AreEqual(2, _plugin1DependsOnCore.Dependants.Count);
			Assert.True(Contains(_plugin3DependsOn1, _plugin1DependsOnCore.Dependants));
			Assert.True(Contains(_plugin5DependsOn3And1AndCore, _plugin1DependsOnCore.Dependants));

			Assert.AreEqual(2, _plugin3DependsOn1.Dependants.Count);
			Assert.True(Contains(_plugin4DependsOn3AndCore, _plugin3DependsOn1.Dependants));
			Assert.True(Contains(_plugin4DependsOn3AndCore, _plugin3DependsOn1.Dependants));

			Assert.AreEqual(0, _plugin2DependsOnCore.Dependants.Count);
			Assert.AreEqual(0, _plugin4DependsOn3AndCore.Dependants.Count);
			Assert.AreEqual(0, _plugin5DependsOn3And1AndCore.Dependants.Count);
		}

		private bool Contains(PluginHolder item, IList<PluginHolder> collection)
		{
			return collection.Contains(item);
		}

		#endregion
	}
}
