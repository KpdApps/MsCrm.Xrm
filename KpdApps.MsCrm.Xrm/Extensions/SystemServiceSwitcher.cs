using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace KpdApps.MsCrm.Xrm.Extensions
{
    public sealed class SystemServiceSwitcher : IDisposable
    {
        private IOrganizationService Service { get; }

        private Guid UserCallerId { get; }

        private static readonly Dictionary<Guid, Guid> OrgSystemUserIds = new Dictionary<Guid, Guid>();

        private Guid SystemUser
        {
            get
            {
                var orgId = ((WhoAmIResponse)Service.Execute(new WhoAmIRequest())).OrganizationId;

                if (OrgSystemUserIds.ContainsKey(orgId))
                    return OrgSystemUserIds[orgId];

                var query = new QueryExpression
                {
                    EntityName = Schema.SystemUser.LogicalName,
                    NoLock = true,
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression(Schema.SystemUser.Fullname, ConditionOperator.Equal, "SYSTEM")
                        }
                    }
                };

                EntityCollection users = Service.RetrieveMultiple(query);
                if (users.Entities.Count > 0)
                    OrgSystemUserIds.Add(orgId, users[0].Id);
                else throw new Exception("SystemUser \"SYSTEM\" was not found.");

                return OrgSystemUserIds[orgId];
            }
        }

        public SystemServiceSwitcher(IOrganizationService service)
        {
            Service = service;
            OrganizationServiceProxy proxy = Service as OrganizationServiceProxy;
            if (proxy != null)
            {
                UserCallerId = proxy.CallerId;
                proxy.CallerId = SystemUser;
            }
        }

        public void Dispose()
        {
            if (Service != null && ((OrganizationServiceProxy)Service).CallerId != UserCallerId)
                ((OrganizationServiceProxy)Service).CallerId = UserCallerId;
        }
    }
}
