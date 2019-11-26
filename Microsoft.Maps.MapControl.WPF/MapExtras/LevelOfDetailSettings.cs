namespace Microsoft.Maps.MapExtras
{
    internal struct LevelOfDetailSettings
    {
        public bool Visible { get; set; }

        public double TargetOpacity { get; set; }

        public int? DownloadPriority { get; set; }

        public LevelOfDetailSettings(bool visible, double targetOpacity, int? downloadPriority)
        {
            Visible = visible;
            TargetOpacity = targetOpacity;
            DownloadPriority = downloadPriority;
        }
    }
}
