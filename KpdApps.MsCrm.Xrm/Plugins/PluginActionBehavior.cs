using System;
using System.Collections.Generic;

namespace KpdApps.MsCrm.Xrm.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PluginActionBehavior : Attribute
    {
        public PluginTargetType TargetType { get; private set; }
        public string EntityName { get; private set; }
        public string TargetEntityReferenceName { get; private set; }
        public string MessageName { get; private set; }
        public string[] PreImages { get; private set; }
        public string[] PostImages { get; private set; }
        public Dictionary<string, Type> InputParameters { get; private set; }

        public PluginActionBehavior(PluginTargetType targetType, string messageName, string entityName = null, string targetEntityReferenceName = null,
                                        string[] preImages = null, string[] postImages = null, Dictionary<string, Type> inputParameters = null)
        {
            TargetType = targetType;
            MessageName = messageName;
            EntityName = entityName;
            TargetEntityReferenceName = targetEntityReferenceName;
            PreImages = preImages;
            PostImages = postImages;
            InputParameters = inputParameters;
        }
    }
}
