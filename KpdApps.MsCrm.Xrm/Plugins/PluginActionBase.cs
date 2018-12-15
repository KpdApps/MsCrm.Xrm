using Microsoft.Xrm.Sdk;
using System;

namespace KpdApps.MsCrm.Xrm.Plugins
{
    public abstract class PluginActionBase
    {
        public PluginState State { get; set; }

        public ParameterCollection SharedVariables => State.SharedVariables;

        public IOrganizationService Service => State.Service;

        public IOrganizationService AdminService => State.AdminService;

        /// <summary>
        /// 
        /// </summary>
        public PluginActionBase(PluginState state)
        {
            State = state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// /// <param name="state"></param>
        public virtual void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
