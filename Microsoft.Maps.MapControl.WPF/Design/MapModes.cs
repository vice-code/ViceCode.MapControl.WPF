namespace Microsoft.Maps.MapControl.WPF.Design
{
    internal static class MapModes
    {
        public static AerialMode Aerial => new AerialMode(false);

        public static AerialMode AerialWithLabels => new AerialMode(true);

        public static RoadMode Road => new RoadMode();
    }
}
