using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF
{
    public class AerialMode : MapMode
    {
        private string aerialTileUriFormat = string.Empty;
        private string aerialWithLabelsTileUriFormat = string.Empty;
        private string aerialUriSubdomains = string.Empty;
        private string aerialWithLablesUriSubdomains = string.Empty;
        private const string aerialUriFormatKey = "AERIALWITHOUTLABELS";
        private const string aerialWithLabelsUriFormatKey = "AERIALWITHLABELS";

        public AerialMode()
          : this(false)
        {
        }

        public AerialMode(bool labels) => Labels = labels;

        public bool Labels { get; set; }

        internal override string TileUriFormat => (Labels ? aerialWithLabelsTileUriFormat : aerialTileUriFormat).Replace("{Culture}", Culture);

        internal override string Subdomains => Labels ? aerialWithLablesUriSubdomains : aerialUriSubdomains;

        internal override PlatformServices.MapStyle? MapStyle =>
            new Microsoft.Maps.MapControl.WPF.PlatformServices.MapStyle?(Labels ? PlatformServices.MapStyle.AerialWithLabels : PlatformServices.MapStyle.Aerial);

        internal override void AsynchronousConfigurationLoaded(
          MapConfigurationSection config,
          object userState)
        {
            if (config is null)
                return;
            var flag1 = false;
            var str1 = config["AERIALWITHLABELS"];
            var str2 = config["AERIALWITHOUTLABELS"];
            if (str2.IndexOf("{0-3}") != -1)
            {
                aerialUriSubdomains = "0,1,2,3";
                str2 = str2.Replace("{0-3}", "{subdomain}");
            }
            if (str2.IndexOf("{0-7}") != -1)
            {
                aerialUriSubdomains = "0,2,4,6 1,3,5,7";
                str2 = str2.Replace("{0-7}", "{subdomain}");
            }
            if (str1.IndexOf("{0-3}") != -1)
            {
                aerialWithLablesUriSubdomains = "0,1,2,3";
                str1 = str1.Replace("{0-3}", "{subdomain}");
            }
            if (str1.IndexOf("{0-7}") != -1)
            {
                aerialWithLablesUriSubdomains = "0,2,4,6 1,3,5,7";
                str1 = str1.Replace("{0-7}", "{subdomain}");
            }
            var flag2 = flag1 || str2 != aerialTileUriFormat || str1 != aerialWithLabelsTileUriFormat;
            aerialTileUriFormat = str2;
            aerialWithLabelsTileUriFormat = str1;
            if (!flag2)
                return;
            RebuildTileSource();
        }
    }
}
