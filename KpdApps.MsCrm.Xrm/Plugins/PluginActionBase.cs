using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Text;

namespace KpdApps.MsCrm.Xrm.Plugins
{
    public abstract class PluginActionBase
    {
        public PluginState State { get; set; }

        public ParameterCollection SharedVariables => State.SharedVariables;

        public IOrganizationService Service => State.Service;

        public IOrganizationService AdminService => State.AdminService;

        private PluginActionBehavior[] _behaviors;

        public string ErrorMessage => _errorMessageBuilder.ToString();
        private StringBuilder _errorMessageBuilder = new StringBuilder();

        public string TraceMessage => _traceMessageBuilder.ToString();
        private StringBuilder _traceMessageBuilder = new StringBuilder();

        /// <summary>
        /// 
        /// </summary>
        public PluginActionBase(PluginState state)
        {
            State = state;
            _behaviors = Attribute.GetCustomAttributes(GetType()).Select(s => (PluginActionBehavior)s).ToArray();
        }

        protected bool PreValidate()
        {
            if (_behaviors.Length == 0)
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

                if (behavior.PreImages != null)
                {
                    foreach (var preImage in behavior.PreImages)
                    {
                        if (!context.PreEntityImages.ContainsKey(preImage))
                        {
                            TraceActionError($"Pre-Image '{preImage}' not registered for plug-in");
                        }
                    }
                }

                if (behavior.PostImages != null)
                {
                    foreach (var postImage in behavior.PostImages)
                    {
                        if (!context.PostEntityImages.ContainsKey(postImage))
                        {
                            TraceActionError($"Post-Image '{postImage}' not registered for plug-in");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(behavior.EntityName))
                {
                    if ((behavior.TargetType == PluginTargetType.Relationship)
                        && (string.Compare(((Relationship)context.InputParameters["Relationship"]).SchemaName, behavior.EntityName, true) == 0))
                        return true;

                    if ((behavior.TargetType == PluginTargetType.TargetEntity)
                        && (string.Compare(((Entity)context.InputParameters["Target"]).LogicalName, behavior.EntityName, true) == 0))
                        return true;

                    if ((behavior.TargetType == PluginTargetType.EntityMoniker)
                        && (string.Compare(((EntityReference)context.InputParameters["EntityMoniker"]).LogicalName, behavior.EntityName, true) == 0))
                        return true;

                    if ((behavior.TargetType == PluginTargetType.TargetEntityReference)
                        && (string.Compare(((EntityReference)context.InputParameters["Target"]).LogicalName, behavior.EntityName, true) == 0))
                        return true;
                }
            }

            TraceActionError($"No behavior found for the specified target:");
            //RelationShip
            if (context.InputParameters.ContainsKey("Relationship"))
            {
                TraceActionError($"Relationship: {((Relationship)context.InputParameters["Relationship"]).SchemaName}");
            }
            //Target
            else if (context.InputParameters.ContainsKey("Target"))
            {
                TraceActionError($"Target: {((Entity)context.InputParameters["Target"]).LogicalName}");
            }
            //EntityMoniker
            else if (context.InputParameters.ContainsKey("EntityMoniker"))
            {
                TraceActionError($"EntityMoniker: {((Relationship)context.InputParameters["Relationship"]).SchemaName}");
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

        protected void TraceActionError(string message)
        {
            _traceMessageBuilder.AppendLine($"{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")} : {message}");
            _errorMessageBuilder.AppendLine($"{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")} : {message}");
        }

        protected void TraceAction(string message)
        {
            _traceMessageBuilder.AppendLine($"{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")} : {message}");
        }
    }
}