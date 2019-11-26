namespace Microsoft.Maps.MapExtras
{
    internal class RasterTileCacheKey
    {
        private readonly int rasterTileCacheId;
        private readonly TileId tileId;

        public override bool Equals(object obj)
        {
            if (obj is RasterTileCacheKey rasterTileCacheKey && rasterTileCacheId == rasterTileCacheKey.rasterTileCacheId)
                return tileId == rasterTileCacheKey.tileId;
            return false;
        }

        public override int GetHashCode() => rasterTileCacheId ^ tileId.GetHashCode();

        public RasterTileCacheKey(int rasterTileCacheId, TileId tileId)
        {
            this.rasterTileCacheId = rasterTileCacheId;
            this.tileId = tileId;
        }
    }
}
