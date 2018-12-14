using FakeXrmEasy;
using KpdApps.MsCrm.Xrm.UnitTests.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KpdApps.MsCrm.Xrm.UnitTests
{
    [TestClass]
    public class AccountPluginActionsTests
    {
        [TestMethod]
        public void CompletedServicesToRealizedActionTest()
        {
            XrmFakedContext fakedContext = new XrmFakedContext();
            XrmFakedPluginExecutionContext fakedpluginContext = new XrmFakedPluginExecutionContext();

            /*Init context here*/

            //fakedContext.ExecutePluginWith<PluginActionTestAdapter<CompletedServicesToRealizedAction>>();
        }
    }
}
