using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Microsoft.Maps.MapExtras
{
    internal class RasterTileCache
    {
        private readonly HashSet<TileId> pendingDownloads = new HashSet<TileId>();
        private static int nextRasterTileCacheId;
        private readonly int rasterTileCacheId;
        private readonly TilePyramidDescriptor tilePyramidDescriptor;
        private readonly RasterTileDownloader rasterTileDownloader;
        private readonly TransformTileId transformTileId;
        private readonly bool useGlobalMemoryCache;
        private readonly Dictionary<TileId, RasterTileCacheValue> rasterTileCacheValues;
        private readonly HashSet<TileId> relevantTransformedTileIds;
        private readonly List<TileId> rasterTileCacheValuesToRemove;

        public RasterTileCache(TilePyramidDescriptor tilePyramidDescriptor, RasterTileDownloader rasterTileDownloader, TransformTileId transformTileId, bool useGlobalMemoryCache)
        {
            this.tilePyramidDescriptor = tilePyramidDescriptor;
            this.rasterTileDownloader = rasterTileDownloader;
            this.transformTileId = transformTileId;
            this.useGlobalMemoryCache = useGlobalMemoryCache;
            rasterTileCacheId = nextRasterTileCacheId++;
            if (this.transformTileId is null)
                this.transformTileId = tileId => tileId;
            if (useGlobalMemoryCache)
                return;
            rasterTileCacheValues = new Dictionary<TileId, RasterTileCacheValue>();
            relevantTransformedTileIds = new HashSet<TileId>();
            rasterTileCacheValuesToRemove = new List<TileId>();
        }

        public event EventHandler NewTilesAvailable;

        public RasterTileCacheValue GetValue(TileId tileId)
        {
            var tileId1 = transformTileId(tileId);
            if (useGlobalMemoryCache)
                return MemoryCache.Instance.GetValue<RasterTileCacheValue>(new RasterTileCacheKey(rasterTileCacheId, tileId1));
            var rasterTileCacheValue = (RasterTileCacheValue)null;
            rasterTileCacheValues.TryGetValue(tileId1, out rasterTileCacheValue);
            return rasterTileCacheValue;
        }

        public void SetRelevantTileSet(RelevantTileSet relevantTileSet)
        {
            foreach (var relevantTile in relevantTileSet.RelevantTiles)
            {
                var tileId = transformTileId(relevantTile.Item1);
                if (!useGlobalMemoryCache)
                    relevantTransformedTileIds.Add(tileId);
                if (pendingDownloads.Contains(tileId))
                {
                    if (relevantTile.Item2.HasValue)
                    {
                        rasterTileDownloader.UpdateTileDownloadPriority(tileId, relevantTile.Item2.Value);
                    }
                    else
                    {
                        rasterTileDownloader.CancelTileDownload(tileId);
                        pendingDownloads.Remove(tileId);
                    }
                }
                else if (relevantTile.Item2.HasValue && GetValue(relevantTile.Item1) is null)
                {
                    pendingDownloads.Add(tileId);
                    rasterTileDownloader.DownloadTile(tileId, tilePyramidDescriptor.GetTileEdgeFlags(relevantTile.Item1), tileId, new RasterTileAvailableDelegate(RasterTileImageAvailable), relevantTile.Item2.Value);
                }
            }
            foreach (var removedTile in relevantTileSet.RemovedTiles)
            {
                var tileId = transformTileId(removedTile);
                if (pendingDownloads.Contains(tileId))
                {
                    rasterTileDownloader.CancelTileDownload(tileId);
                    pendingDownloads.Remove(tileId);
                }
            }
            if (useGlobalMemoryCache)
                return;
            foreach (var rasterTileCacheValue in rasterTileCacheValues)
            {
                if (!relevantTransformedTileIds.Contains(rasterTileCacheValue.Key))
                    rasterTileCacheValuesToRemove.Add(rasterTileCacheValue.Key);
            }
            foreach (var key in rasterTileCacheValuesToRemove)
                rasterTileCacheValues.Remove(key);
            rasterTileCacheValuesToRemove.Clear();
            relevantTransformedTileIds.Clear();
        }

        private void RasterTileImageAvailable(BitmapSource image, Rect tileSubregion, Dictionary<string, string> metadata, object token)
        {
            var tileId = (TileId)token;
            var rasterTileCacheValue = new RasterTileCacheValue(image, tileSubregion, metadata);
            if (useGlobalMemoryCache)
                MemoryCache.Instance.Replace(new RasterTileCacheKey(rasterTileCacheId, tileId), rasterTileCacheValue);
            else
                rasterTileCacheValues[tileId] = rasterTileCacheValue;
            if (!pendingDownloads.Remove(tileId) || NewTilesAvailable is null)
                return;
            NewTilesAvailable(this, EventArgs.Empty);
        }
    }
}
