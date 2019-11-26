namespace Microsoft.Maps.MapExtras
{
    internal struct TileStatus
    {
        public TileId TileId { get; set; }

        public bool Visible { get; set; }

        public bool Available { get; set; }

        public bool WillNeverBeAvailable { get; set; }

        public bool FullyOpaque { get; set; }
    }
}
