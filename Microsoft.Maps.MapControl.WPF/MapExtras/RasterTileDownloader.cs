namespace Microsoft.Maps.MapExtras
{
    internal abstract class RasterTileDownloader
    {
        public abstract void DownloadTile(
          TileId tileId,
          TileEdgeFlags tileEdgeFlags,
          object token,
          RasterTileAvailableDelegate tileAvailableDelegate,
          int priority);

        public abstract void UpdateTileDownloadPriority(TileId tileId, int priority);

        public abstract void CancelTileDownload(TileId tileId);
    }
}
