using KpdApps.MsCrm.Xrm.Plugins;

namespace KpdApps.MsCrm.Xrm.UnitTests.Plugins.Behaviors
{
    [PluginActionBehavior(PluginTargetType.TargetEntity, "Update", "kpd_entity", null, new[] { "image" }, new[] { "image" })]
    public class PluginActionOnUpdateBehavior : PluginActionBase
    {
        public PluginActionOnUpdateBehavior(PluginState state) : base(state)
        {

        }

        public override void Execute()
        {

        }
    }
}
