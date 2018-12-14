using Microsoft.Xrm.Sdk;

namespace KpdApps.MsCrm.Xrm.Helpers
{
    public static class SharedVariablesHelper
    {
        public static string GenerateSharedVariableName(Entity entity)
        {
            return GenerateSharedVariableName(entity.ToEntityReference());
        }

        public static string GenerateSharedVariableName(EntityReference entityReference)
        {
            return $"{entityReference.LogicalName}_{entityReference.Id}";
        }
    }
}
