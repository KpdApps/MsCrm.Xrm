using KpdApps.MsCrm.Xrm.Plugins;
using System;

namespace KpdApps.MsCrm.Xrm.Tests
{
    public class PluginActionTestAdapter<T> : PluginBase
    {
        public string ErrorMessage { get; private set; }
        public string TraceMessage { get; private set; }

        public PluginActionTestAdapter()
        {

        }

        public override void ExecuteInternal(PluginState state)
        {
            PluginActionBase action = (PluginActionBase)Activator.CreateInstance(typeof(T), state);
            action.TryExecute();
            ErrorMessage = action.ErrorMessage;
            TraceMessage = action.TraceMessage;
        }
    }
}
