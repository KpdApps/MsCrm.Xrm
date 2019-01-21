using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace KpdApps.MsCrm.Xrm.Plugins
{
    public abstract class PluginActionBase
    {
        public PluginState State { get; set; }

        public ParameterCollection SharedVariables => State.SharedVariables;

        public IOrganizationService Service => State.Service;

        public IOrganizationService AdminService => State.AdminService;

        private PluginActionBehavior[] _behaviors;

        /// <summary>
        /// 
        /// </summary>
        public PluginActionBase(PluginState state)
        {
            State = state;
            _behaviors = Attribute.GetCustomAttributes(GetType()).Select(s => (PluginActionBehavior)s).ToArray();
        }

        private bool PreValidate()
        {
            if (!_behaviors.Any())
            {
                return true;
            }

            IPluginExecutionContext context = (IPluginExecutionContext)State.Provider.GetService(typeof(IPluginExecutionContext));
            foreach (PluginActionBehavior behavior in _behaviors)
            {
                if (string.Compare(behavior.MessageName, context.MessageName, true) != 0)
                {
                    continue;
                }

                foreach (var preImage in behavior.PreImages)
                {
                    if (!context.PreEntityImages.ContainsKey(preImage))
                        throw new Exception($"Pre-Image '{preImage}' not registered for plug-in");
                }
            }

            return false;
        }

        public virtual bool TryExecute()
        {
            if (PreValidate())
            {
                Execute();
                return true;
            }

            return false;
        }

        public abstract void Execute();
    }
}
