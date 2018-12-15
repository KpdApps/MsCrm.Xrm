﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KpdApps.MsCrm.Xrm.Helpers
{
    public static class MetadataHelper
    {
        private static List<EntityMetadata> _entities = new List<EntityMetadata>();

        private static readonly object Locker = new object();

        public static EntityMetadata GetEntityMetadata(string entityName, IOrganizationService service)
        {
            lock (Locker)
            {
                if (_entities == null)
                    _entities = new List<EntityMetadata>();
            }

            EntityMetadata metadata = _entities.FirstOrDefault(a => string.Compare(a.SchemaName, entityName, StringComparison.InvariantCultureIgnoreCase) == 0) ?? LoadEntityMetadata(entityName, service);

            return metadata;
        }

        private static EntityMetadata LoadEntityMetadata(string entityName, IOrganizationService service)
        {
            lock (Locker)
            {
                RetrieveEntityRequest request = new RetrieveEntityRequest
                {
                    LogicalName = entityName,
                    EntityFilters = EntityFilters.Entity | EntityFilters.Attributes
                };

                RetrieveEntityResponse metadata = (RetrieveEntityResponse)service.Execute(request);

                if (metadata.EntityMetadata == null)
                    return null;

                _entities.Add(metadata.EntityMetadata);
                return metadata.EntityMetadata;
            }
        }

        public static string GetPickListLabel(string entityName, string fieldName, int value, IOrganizationService service)
        {
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = fieldName,
                RetrieveAsIfPublished = true
            };

            RetrieveAttributeResponse response = (RetrieveAttributeResponse)service.Execute(attributeRequest);
            EnumAttributeMetadata attributeMetadata = (EnumAttributeMetadata)response.AttributeMetadata;
            foreach (OptionMetadata optionMeta in attributeMetadata.OptionSet.Options)
            {
                if (optionMeta.Value == value)
                    return optionMeta.Label.UserLocalizedLabel.Label;
            }
            return string.Empty;
        }
    }
}
