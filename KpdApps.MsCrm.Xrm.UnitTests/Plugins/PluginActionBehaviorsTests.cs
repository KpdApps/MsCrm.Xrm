using FakeXrmEasy;
using KpdApps.MsCrm.Xrm.Extensions;
using KpdApps.MsCrm.Xrm.Plugins;
using KpdApps.MsCrm.Xrm.Tests;
using KpdApps.MsCrm.Xrm.UnitTests.Plugins.Behaviors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpdApps.MsCrm.Xrm.UnitTests.Plugins
{
    [TestClass]
    public class PluginActionBehaviorsTests
    {
        XrmFakedContext _fakedContext = new XrmFakedContext();
        XrmFakedPluginExecutionContext _fakedpluginContext;

        [TestMethod]
        public void TestCreateBehavior()
        {
            Entity target = new Entity("kpd_entity") { Id = Guid.NewGuid() };
            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            _fakedpluginContext = new XrmFakedPluginExecutionContext
            {
                InputParameters = inputParameters,
                MessageName = "Create",
                UserId = Guid.NewGuid()
            };

            PluginActionTestAdapter<PluginActionOnCreateBehavior> _pluginActionTestAdapter = new PluginActionTestAdapter<PluginActionOnCreateBehavior>();
            _fakedContext.ExecutePluginWith(_fakedpluginContext, _pluginActionTestAdapter);

            Assert.IsTrue(string.IsNullOrEmpty(_pluginActionTestAdapter.ErrorMessage));
        }

        [TestMethod]
        public void TestCreateBevaviorError()
        {
            Entity target = new Entity("kpd_entity") { Id = Guid.NewGuid() };
            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", target);

            target.Attributes.SetStringValue("kpd_attr", "test");
            _fakedpluginContext = new XrmFakedPluginExecutionContext
            {
                InputParameters = inputParameters,
                MessageName = "Update",
                UserId = Guid.NewGuid(),
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            PluginActionTestAdapter<PluginActionOnUpdateBehavior> _pluginActionTestAdapter = new PluginActionTestAdapter<PluginActionOnUpdateBehavior>();
            _fakedContext.ExecutePluginWith(_fakedpluginContext, _pluginActionTestAdapter);

            Assert.IsFalse(string.IsNullOrEmpty(_pluginActionTestAdapter.ErrorMessage));
        }
    }
}
