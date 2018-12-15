﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace KpdApps.MsCrm.Xrm.Extensions
{
    /// <summary>
    ///     Extensions for Entity.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        ///     Copy all attributes (except primary key) to new Entity.
        /// </summary>
        public static Entity Clone(this Entity entity)
        {
            Entity result = new Entity(entity.LogicalName);

            //Поле не подлежащие клонированию
            StringCollection skippedFields = new StringCollection { entity.LogicalName + "id" };
            if (entity.Contains("activitytypecode"))
                skippedFields.Add("activityid");

            foreach (var attr in entity.Attributes)
            {
                if (skippedFields.Contains(attr.Key))
                    continue;

                object val = CloneAttributeValue(attr.Value);
                result.Attributes.Add(attr.Key, val);
            }

            return result;
        }

        /// <summary>
        ///     Copy selected columns to new Entity.
        /// </summary>
        /// <param name="entity">Original Entity.</param>
        /// <param name="fields">Columns to copy.</param>
        public static Entity Clone(this Entity entity, params string[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            Entity result = new Entity(entity.LogicalName);
            foreach (string field in fields)
            {
                if (!entity.Contains(field))
                    continue;

                object val = CloneAttributeValue(entity[field]);
                result.Attributes.Add(field, val);
            }

            return result;
        }

        public static void SetFieldsByMapping(this Entity target, Entity sourceEntity, Dictionary<string, string> mapping)
        {
            foreach (var sourceAttrName in mapping.Keys)
            {
                object value = null;
                if (sourceEntity.Attributes.ContainsNotNull(sourceAttrName))
                {
                    value = sourceEntity.Attributes[sourceAttrName];
                }

                var targetName = mapping[sourceAttrName];
                if (target.Attributes.Contains(targetName))
                {
                    target.Attributes[targetName] = value;
                }
                else
                {
                    target.Attributes.Add(targetName, value);
                }
            }
        }

        /// <summary>
        ///     Copies all attributes missing in current entity from source.
        /// </summary>
        /// <param name="entity">Attributes of this entity have priority over source</param>
        /// <param name="source">Source entity</param>
        /// <returns>New instance with all the attributes</returns>
        public static Entity MergeIn(this Entity entity, Entity source)
        {
            var merged = source.Clone();
            merged.Id = entity.Id;

            foreach (var attr in entity.Attributes)
            {
                merged.Attributes[attr.Key] = CloneAttributeValue(attr.Value);
            }
            return merged;
        }

        /// <summary>
        ///     Check attribute by regular Contains and Value not null.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static bool ContainsNotNull(this Entity entity, string attributeName)
        {
            return entity.Attributes.ContainsNotNull(attributeName);
        }

        /// <summary>
        ///     Checks if entity cointains any of the attributes
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeName"></param>
        /// <returns>True if countains any</returns>
        public static bool ContainsAny(this Entity entity, params string[] attrNames)
        {
            foreach (var attr in attrNames)
            {
                if (entity.Contains(attr))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Checks if entity cointains all the attributes
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeName"></param>
        /// <returns>True if contains all</returns>
        public static bool ContainsAll(this Entity entity, params string[] attrNames)
        {
            foreach (var attr in attrNames)
            {
                if (!entity.Contains(attr))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Copy attribute value.
        /// </summary>
        /// <param name="value"><see cref="OptionSetValue"/>, <see cref="Money"/>, <see cref="EntityReference"/>, <see cref="EntityCollection"/></param>
        /// <returns></returns>
        private static object CloneAttributeValue(object value)
        {
            object val = value;
            if (val is OptionSetValue)
            {
                var opt = (OptionSetValue)val;
                val = new OptionSetValue(opt.Value);
            }
            else if (val is Money)
            {
                var m = (Money)val;
                val = new Money(m.Value);
            }
            else if (val is EntityReference)
            {
                var temp = (EntityReference)val;
                val = new EntityReference(temp.LogicalName, temp.Id);
            }
            else if (val is EntityCollection)
            {
                var coll = (EntityCollection)val;
                val = new EntityCollection() { EntityName = coll.EntityName };
                foreach (Entity e in coll.Entities)
                {
                    var clone = e.Clone();
                    if (clone.LogicalName == "activityparty")
                    {
                        if (clone.Attributes.Contains("activityid"))
                        {
                            clone.Attributes.Remove("activityid");
                        }

                        if (clone.Attributes.Contains("activitypartyid"))
                        {
                            clone.Attributes.Remove("activitypartyid");
                        }
                    }
                    ((EntityCollection)val).Entities.Add(clone);
                }
            }
            return val;
        }

        /// <summary>
        ///     Serialize Entity by DataContractSerializer.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Serialized string.</returns>
        public static string Serialize(this Entity entity)
        {
            var serializer = new DataContractSerializer(typeof(Entity), null, int.MaxValue, false, false, null, new KnownTypesResolver());
            using (var sWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                var writer = new XmlTextWriter(sWriter);
                serializer.WriteObject(writer, entity);
                return sWriter.ToString();
            }
        }

        /// <summary>
        ///     Deserialize Entity by DataContractSerializer.
        /// </summary>
        /// <param name="entityXml">Serialized string.</param>
        /// <returns>Entity.</returns>
        public static Entity Deserialize(string entityXml)
        {
            var reader = new XmlTextReader(new StringReader(entityXml));
            var serializer = new DataContractSerializer(typeof(Entity), null, int.MaxValue, false, false, null, new KnownTypesResolver());
            return (Entity)serializer.ReadObject(reader);
        }

        /// <summary>
        ///     Create empty Entity (which inherit only parent logical name and identification field)
        /// </summary>
        public static Entity CreateEmpty(this Entity entity)
        {
            var result = new Entity(entity.LogicalName) { Id = entity.Id };

            var keyName = entity.LogicalName + "id";
            if (entity.LogicalName.Contains(keyName))
                result.Attributes.Add(keyName, entity.Id);

            return result;
        }

        /// <summary>
        ///     Create copy of Entity to make update request.
        /// </summary>
        /// <param name="entity">Original Entity.</param>
        /// <returns>Entity instance to make update request.</returns>
        public static Entity CloneForUpdate(this Entity entity)
        {
            return Clone(entity, entity.LogicalName + "id");
        }
    }
}
