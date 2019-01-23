using KpdApps.MsCrm.Xrm.Plugins;

namespace KpdApps.MsCrm.Xrm.UnitTests.Plugins.Behaviors
{
    [PluginActionBehavior(PluginTargetType.TargetEntity, "Create", "kpd_entity")]
    public class PluginActionOnCreateBehavior : PluginActionBase
    {
        public PluginActionOnCreateBehavior(PluginState state) : base(state)
        {

        }

        public override void Execute()
        {

        }
    }
}
