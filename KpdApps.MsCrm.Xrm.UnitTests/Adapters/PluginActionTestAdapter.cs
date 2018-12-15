using KpdApps.MsCrm.Xrm.Plugins;
using System;

namespace KpdApps.MsCrm.Xrm.UnitTests.Adapters
{
    public class PluginActionTestAdapter<T> : PluginBase
    {
        public PluginActionTestAdapter()
        {

        }

        public override void ExecuteInternal(PluginState state)
        {
            PluginActionBase action = (PluginActionBase)Activator.CreateInstance(typeof(T), state);
            action.Execute();
        }
    }
}
