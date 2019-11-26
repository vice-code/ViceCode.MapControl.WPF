using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF
{
    public class RoadMode : MapMode
    {
        private string tileUriFormat = string.Empty;
        private string subdomains = string.Empty;
        private const string uriFormatKey = "ROADWITHLABELS";

        internal override string TileUriFormat => tileUriFormat.Replace("{Culture}", Culture);

        internal override string Subdomains => subdomains;

        public override ModeBackground ModeBackground => ModeBackground.Light;

        internal override PlatformServices.MapStyle? MapStyle => new Microsoft.Maps.MapControl.WPF.PlatformServices.MapStyle?(PlatformServices.MapStyle.Road);

        internal override void AsynchronousConfigurationLoaded(
      MapConfigurationSection config,
      object userState)
        {
            if (config is null)
                return;
            var flag1 = false;
            var str = config["ROADWITHLABELS"];
            if (str.IndexOf("{0-3}") != -1)
            {
                subdomains = "0,1,2,3";
                str = str.Replace("{0-3}", "{subdomain}");
            }
            if (str.IndexOf("{0-7}") != -1)
            {
                subdomains = "0,2,4,6 1,3,5,7";
                str = str.Replace("{0-7}", "{subdomain}");
            }
            var flag2 = flag1 || str != tileUriFormat;
            tileUriFormat = str;
            if (!flag2)
                return;
            RebuildTileSource();
        }
    }
}
