﻿using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace KpdApps.MsCrm.Xrm.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IOrganizationService"/>.
    /// </summary>
    public static class OrganizationServiceExtensions
    {
        /// <summary>
        /// Create entity.
        /// </summary>
        /// <param name="organizationService"><see cref="IOrganizationService"/>.</param>
        /// <param name="entity">Entity to create.</param>
        public static Guid CreateEntity(this IOrganizationService organizationService, Entity entity)
        {
            if (entity == null || !entity.Attributes.Any())
                return Guid.Empty;

            return organizationService.Create(entity);
        }

        /// <summary>
        /// Replace for regular Retrieve, return <see cref="Entity"/> with all collumns.
        /// </summary>
        /// <param name="organizationService"><see cref="IOrganizationService"/>.</param>
        /// <param name="entityName">Entity name.</param>
        /// <param name="entityId">Entity identifier.</param>
        /// <returns><see cref="Entity"/></returns>
        public static Entity Retrieve(this IOrganizationService organizationService, string entityName, Guid entityId)
        {
            return organizationService.Retrieve(entityName, entityId, new ColumnSet(true));
        }

        /// <summary>
        /// Replace for regular Retrieve, return <see cref="Entity"/> with all collumns.
        /// </summary>
        /// <param name="organizationService"><see cref="IOrganizationService"/>.</param>
        /// <param name="entityReference"><see cref="EntityReference"/></param>
        /// <returns><see cref="Entity"/></returns>
        public static Entity Retrieve(this IOrganizationService organizationService, EntityReference entityReference)
        {
            return organizationService.Retrieve(entityReference.LogicalName, entityReference.Id, new ColumnSet(true));
        }

        /// <summary>
        /// Replace for regular Retrieve, return <see cref="Entity"/> with all collumns.
        /// </summary>
        /// <param name="organizationService"><see cref="IOrganizationService"/>.</param>
        /// <param name="entityReference"><see cref="EntityReference"/></param>
        /// <param name="colse"><see cref="ColumnSet"/></param>
        /// <returns><see cref="Entity"/></returns>
        public static Entity Retrieve(this IOrganizationService organizationService, EntityReference entityReference, ColumnSet cols)
        {
            return organizationService.Retrieve(entityReference.LogicalName, entityReference.Id, cols);
        }

        /// <summary>
        /// Replace for regular Retrieve, return <see cref="Entity"/> with all collumns.
        /// </summary>
        /// <param name="organizationService"><see cref="IOrganizationService"/>.</param>
        /// <param name="source"><see cref="Entity"/></param>
        /// <param name="referencingAttributeName">Name of attribute of reference to retrieve.</param>
        /// <returns><see cref="Entity"/></returns>
        public static Entity Retrieve(this IOrganizationService organizationService, Entity source, string referencingAttributeName)
        {
            var reference = source[referencingAttributeName] as EntityReference;
            if (reference != null)
                return Retrieve(organizationService, reference);

            return null;
        }

        /// <summary>
        /// Replace for regular Retrieve, return <see cref="Entity"/> with all specified collumns.
        /// </summary>
        /// <param name="organizationService"><see cref="IOrganizationService"/>.</param>
        /// <param name="entityName">Entity name.</param>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="attrs">Attributes to select.</param>
        /// <returns><see cref="Entity"/></returns>
        public static Entity Retrieve(this IOrganizationService organizationService, string entityName, Guid entityId, params string[] attrs)
        {
            if (!attrs.Any())
                return organizationService.Retrieve(entityName, entityId, new ColumnSet(true));

            ColumnSet set = new ColumnSet(attrs);
            return organizationService.Retrieve(entityName, entityId, set);
        }

        /// <summary>
        /// Replace for regular Retrieve, return <see cref="Entity"/> with all specified collumns.
        /// </summary>
        /// <param name="organizationService"><see cref="IOrganizationService"/>.</param>
        /// <param name="entityRef">Entity reference.</param>
        /// <param name="attrs">Attributes to select.</param>
        /// <returns><see cref="Entity"/></returns>
        public static Entity Retrieve(this IOrganizationService organizationService, EntityReference entityRef, params string[] attrs)
        {
            return Retrieve(organizationService, entityRef.LogicalName, entityRef.Id, attrs);
        }

        /// <summary>
        /// Assign user to entity (as owner).
        /// </summary>
        /// <param name="organizationService"><see cref="IOrganizationService"/>.</param>
        /// <param name="assignee">EntityReference to SystemUser.</param>
        /// <param name="target">EntityReference to Target Entity.</param>
        /// <returns></returns>
        [Obsolete("Depricated by vendor.")]
        public static AssignResponse Assign(this IOrganizationService organizationService, EntityReference assignee, EntityReference target)
        {
            AssignRequest request = new AssignRequest
            {
                Assignee = assignee,
                Target = target
            };

            return (AssignResponse)organizationService.Execute(request);
        }

        public static SystemServiceSwitcher SwitchToSystem(this IOrganizationService service)
        {
            return new SystemServiceSwitcher(service);
        }

        public static ExecuteWorkflowResponse ExecuteWorkflowWithRetry(this IOrganizationService service, Guid entityId, Guid workflowId, int retryTimes = 1)
        {
            ExecuteWorkflowRequest request = new ExecuteWorkflowRequest
            {
                EntityId = entityId,
                WorkflowId = workflowId
            };
            return ExecuteWorkflowWithRetry(service, request, retryTimes);
        }

        public static ExecuteWorkflowResponse ExecuteWorkflowWithRetry(this IOrganizationService service, ExecuteWorkflowRequest request, int retryTimes)
        {
            ExecuteWorkflowResponse response = new ExecuteWorkflowResponse();
            int i = 0;
            while (i < retryTimes)
            {
                try
                {
                    response = (ExecuteWorkflowResponse)service.Execute(request);
                    break;
                }
                catch (SqlException)
                {
                    i++;
                    System.Threading.Thread.Sleep(i * 1000);
                }
            }

            if (i >= retryTimes)
                throw new Exception($"ExecuteWorkflowWithRetry WorkflowId({request.WorkflowId}): retry limit exceeded.");

            return response;
        }

        public static void SetStateActive(this IOrganizationService service, Entity entity)
        {
            SetStateActive(service, entity.LogicalName, entity.Id);
        }

        public static void SetStateActive(this IOrganizationService service, EntityReference entityReference)
        {
            SetStateActive(service, entityReference.LogicalName, entityReference.Id);
        }

        public static void SetStateActive(this IOrganizationService service, string entityName, Guid id)
        {
            SetStateDynamic(service, entityName, id, 0, 1);
        }

        public static void SetStateInactive(this IOrganizationService service, Entity entity)
        {
            SetStateInactive(service, entity.LogicalName, entity.Id);
        }

        public static void SetStateInactive(this IOrganizationService service, EntityReference entityReference)
        {
            SetStateInactive(service, entityReference.LogicalName, entityReference.Id);
        }

        public static void SetStateInactive(this IOrganizationService service, string entityName, Guid id)
        {
            SetStateDynamic(service, entityName, id, 1, 2);
        }

        /// <summary>
        /// Execute <see cref="SetStateRequest"/> to set entity state and status codes.
        /// </summary>
        /// <param name="service"><see cref="IOrganizationService"/></param>
        /// <param name="entityName">Logical name of entity.</param>
        /// <param name="id">Entity identifier.</param>
        /// <param name="stateCode">State code</param>
        /// <param name="statusCode">Status code</param>
        public static void SetStateDynamic(this IOrganizationService service, string entityName, Guid id, int stateCode, int statusCode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference(entityName, id),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };

            service.Execute(request);
        }

        /// <summary>
        /// Execute <see cref="SetStateRequest"/> to set entity state and status codes.
        /// </summary>
        /// <param name="service"><see cref="IOrganizationService"/></param>
        /// <param name="entityRef">Logical name of entity.</param>
        /// <param name="stateCode">State code</param>
        /// <param name="statusCode">Status code</param>
        public static void SetStateDynamic(this IOrganizationService service, EntityReference entityRef, int stateCode, int statusCode)
        {
            SetStateDynamic(service, entityRef.LogicalName, entityRef.Id, stateCode, statusCode);
        }
    }
}
