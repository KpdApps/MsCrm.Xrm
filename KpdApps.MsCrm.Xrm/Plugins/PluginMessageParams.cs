using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace KpdApps.MsCrm.Xrm.Plugins
{
    public class PluginMessageParams
    {
        public PluginTargetType EntityType { get; private set; }
        public string EntityName { get; private set; }
        public string TargetEntityReferenceName { get; private set; }
        public string MessageName { get; private set; }
        public string[] PreImages { get; private set; }
        public string[] PostImages { get; private set; }
        public Dictionary<string, Type> InputParameters { get; private set; }
        public bool IsActive { get; private set; }

        private PluginMessageParams(string messageName, Dictionary<string, Type> inputParameters, string[] preImages, string[] postImages, bool isActive = true)
        {
            MessageName = messageName;
            PreImages = preImages ?? (new string[] { });
            PostImages = postImages ?? (new string[] { });
            InputParameters = inputParameters ?? new Dictionary<string, Type>();
            IsActive = isActive;
        }

        public static PluginMessageParams CreateMessageParams(string messageName, bool isActive = true)
        {
            return new PluginMessageParams(messageName, null, null, null, isActive);
        }
        public static PluginMessageParams CreateMessageParams(string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters, bool isActive = true)
        {
            return new PluginMessageParams(messageName, inputParameters, preImages, postImages, isActive);
        }
        public static PluginMessageParams CreateForTargetEntity(string entityName, string messageName, bool isActive = true)
        {
            return CreateForTargetEntity(entityName, messageName, null, null, null, isActive);
        }
        public static PluginMessageParams CreateForTargetEntity(string entityName, string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters, bool isActive = true)
        {
            var r = new PluginMessageParams(messageName, inputParameters, preImages, postImages, isActive)
            {
                EntityType = PluginTargetType.TargetEntity,
                EntityName = entityName
            };

            if (!r.InputParameters.ContainsKey("Target"))
                r.InputParameters.Add("Target", typeof(Entity));

            return r;
        }
        public static PluginMessageParams CreateForTargetEntityReference(string entityName, string messageName, bool isActive = true)
        {
            return CreateForTargetEntityReference(entityName, messageName, null, null, null, isActive);
        }
        public static PluginMessageParams CreateForTargetEntityReference(string entityName, string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters, bool isActive = true)
        {
            var r = new PluginMessageParams(messageName, inputParameters, preImages, postImages, isActive)
            {
                EntityType = PluginTargetType.TargetEntityReference,
                EntityName = entityName
            };

            if (!r.InputParameters.ContainsKey("Target"))
                r.InputParameters.Add("Target", typeof(EntityReference));

            return r;
        }
        public static PluginMessageParams CreateForTargetEntityMoniker(string entityName, string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters, bool isActive = true)
        {
            var r = new PluginMessageParams(messageName, inputParameters, preImages, postImages, isActive)
            {
                EntityType = PluginTargetType.EntityMoniker,
                EntityName = entityName
            };

            if (!r.InputParameters.ContainsKey("EntityMoniker"))
                r.InputParameters.Add("EntityMoniker", typeof(EntityReference));

            return r;
        }
        public static PluginMessageParams CreateForRelationship(string relationshipName, string messageName, bool isActive = true)
        {
            return CreateForRelationship(relationshipName, messageName, null, null, null, isActive);
        }
        public static PluginMessageParams CreateForRelationship(string relationshipName, string messageName, string[] preImages, string[] postImages, Dictionary<string, Type> inputParameters, bool isActive = true)
        {
            var r = new PluginMessageParams(messageName, inputParameters, preImages, postImages, isActive)
            {
                EntityType = PluginTargetType.Relationship,
                EntityName = relationshipName
            };

            if (!r.InputParameters.ContainsKey("Relationship"))
                r.InputParameters.Add("Relationship", typeof(Relationship));

            return r;
        }
    }
}
