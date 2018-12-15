using KpdApps.MsCrm.Xrm.Extensions;
using Microsoft.Xrm.Sdk;
using System;

namespace KpdApps.MsCrm.Xrm.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        public readonly string UnsecureConfiguration;

        public readonly string SecureConfiguration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unsecureConfiguration"></param>
        /// <param name="secureConfiguration"></param>
        public PluginBase(string unsecureConfiguration, string secureConfiguration)
        {
            UnsecureConfiguration = unsecureConfiguration;
            SecureConfiguration = secureConfiguration;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PluginBase()
        {

        }

        public void Execute(IServiceProvider serviceProvider)
        {
            PluginState state = PluginState.CreatePluginState(serviceProvider);

            try
            {
                ExecuteInternal(state);
            }
            catch (Exception ex)
            {
                state?.TracingService.TraceError(ex);
                throw;
            }
        }

        public abstract void ExecuteInternal(PluginState state);
    }
}
