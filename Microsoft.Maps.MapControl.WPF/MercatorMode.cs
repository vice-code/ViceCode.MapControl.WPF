using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF
{
    public class MercatorMode : MapMode
    {
        internal override string TileUriFormat => string.Empty;

        internal override string Subdomains => string.Empty;

        public override ModeBackground ModeBackground => ModeBackground.Light;

        internal override PlatformServices.MapStyle? MapStyle => new Microsoft.Maps.MapControl.WPF.PlatformServices.MapStyle?();

        internal override void AsynchronousConfigurationLoaded(
      MapConfigurationSection config,
      object userState)
        {
        }
    }
}
