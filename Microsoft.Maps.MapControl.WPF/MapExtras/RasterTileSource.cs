using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Microsoft.Maps.MapExtras
{
    internal class RasterTileSource : TileSource
    {
        private static readonly Pool<Size, Image> imagePool = new Pool<Size, Image>();
        private Matrix3D transform = Matrix3D.Identity;
        private readonly RelevantTileSet relevantTileSet = new RelevantTileSet();
        private readonly Dictionary<TileId, TileRenderable> tileRenderables = new Dictionary<TileId, TileRenderable>();
        private readonly long finestLodWidth;
        private readonly long finestLodHeight;
        private readonly int tileWidth;
        private readonly int tileHeight;
        private readonly int minimumLevelOfDetail;
        private int maximumLevelOfDetail;
        private readonly TransformTileId tileIdTransform;
        private TilePyramidDescriptor tilePyramidDescriptor;
        private RasterTileCache rasterTileImageCache;

        public RasterTileSource(long finestLodWidth, long finestLodHeight, int tileWidth, int tileHeight, int minimumLevelOfDetail, RasterTileDownloader rasterTileDownloader, bool useGlobalMemoryCache = true)
        {
            if (finestLodWidth <= 0L)
                throw new ArgumentOutOfRangeException(nameof(finestLodWidth));
            if (finestLodHeight <= 0L)
                throw new ArgumentOutOfRangeException(nameof(finestLodHeight));
            if (tileWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(tileWidth));
            if (tileHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(tileHeight));
            if (rasterTileDownloader is null)
                throw new ArgumentNullException(nameof(rasterTileDownloader));
            this.finestLodWidth = finestLodWidth;
            this.finestLodHeight = finestLodHeight;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.minimumLevelOfDetail = minimumLevelOfDetail;
            ConstructCommon(rasterTileDownloader, useGlobalMemoryCache);
        }

        public RasterTileSource(long logicalFinestLodWidth, long logicalFinestLodHeight, int tileWidth, int tileHeight, int logicalMinimumLevelOfDetail, RasterTileDownloader rasterTileDownloader, TileWrap tileWrap, int log2DuplicatePyramidCount, bool useGlobalMemoryCache = true)
        {
            if (logicalFinestLodWidth <= 0L)
                throw new ArgumentOutOfRangeException(nameof(logicalFinestLodWidth));
            if (logicalFinestLodHeight <= 0L)
                throw new ArgumentOutOfRangeException(nameof(logicalFinestLodHeight));
            if (tileWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(tileWidth));
            if (tileHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(tileHeight));
            if (rasterTileDownloader is null)
                throw new ArgumentNullException(nameof(rasterTileDownloader));
            if (tileWrap == TileWrap.None)
                throw new ArgumentException("tileWrap must horizontal, vertical, or both.");
            if (log2DuplicatePyramidCount < 1)
                throw new ArgumentOutOfRangeException(nameof(log2DuplicatePyramidCount));
            finestLodWidth = logicalFinestLodWidth << log2DuplicatePyramidCount;
            finestLodHeight = logicalFinestLodHeight << log2DuplicatePyramidCount;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            minimumLevelOfDetail = logicalMinimumLevelOfDetail + log2DuplicatePyramidCount;
            var logicalTilePyramidDescriptor = new TilePyramidDescriptor(logicalFinestLodWidth, logicalFinestLodHeight, logicalMinimumLevelOfDetail, tileWidth, tileHeight);
            tileIdTransform = tileId =>
           {
               var lod = tileId.LevelOfDetail - log2DuplicatePyramidCount;
               var detailWidthInTiles = logicalTilePyramidDescriptor.GetLevelOfDetailWidthInTiles(lod);
               var detailHeightInTiles = logicalTilePyramidDescriptor.GetLevelOfDetailHeightInTiles(lod);
               return new TileId(tileId.LevelOfDetail - log2DuplicatePyramidCount, tileWrap == TileWrap.Horizontal || tileWrap == TileWrap.Both ? tileId.X % detailWidthInTiles : tileId.X, tileWrap == TileWrap.Vertical || tileWrap == TileWrap.Both ? tileId.Y % detailHeightInTiles : tileId.Y);
           };
            transform = VectorMath.TranslationMatrix3D(tileWrap == TileWrap.Horizontal || tileWrap == TileWrap.Both ? -finestLodWidth / 2L : 0.0, tileWrap == TileWrap.Vertical || tileWrap == TileWrap.Both ? -finestLodHeight / 2L : 0.0, 0.0) * VectorMath.ScalingMatrix3D(1 << log2DuplicatePyramidCount, 1 << log2DuplicatePyramidCount, 1.0);
            ConstructCommon(rasterTileDownloader, useGlobalMemoryCache);
        }

        private void ConstructCommon(RasterTileDownloader rasterTileDownloader, bool useGlobalMemoryCache)
        {
            tilePyramidDescriptor = new TilePyramidDescriptor(finestLodWidth, finestLodHeight, minimumLevelOfDetail, tileWidth, tileHeight);
            maximumLevelOfDetail = tilePyramidDescriptor.FinestLevelOfDetail;
            if (minimumLevelOfDetail < 0 || minimumLevelOfDetail > maximumLevelOfDetail)
                throw new ArgumentException("minimumLevelOfDetail must be in range [0, finest level of detail].");
            rasterTileImageCache = new RasterTileCache(tilePyramidDescriptor, rasterTileDownloader, tileIdTransform, useGlobalMemoryCache);
            rasterTileImageCache.NewTilesAvailable += (sender, e) =>
           {
               if (NewTilesAvailable is null)
                   return;
               NewTilesAvailable(this, EventArgs.Empty);
           };
            TileFadeInDuration = new TimeSpan?(TimeSpan.FromMilliseconds(300.0));
        }

        public override TimeSpan? TileFadeInDuration { get; set; }

        public override long FinestLodWidth => finestLodWidth;

        public override long FinestLodHeight => finestLodHeight;

        public override int TileWidth => tileWidth;

        public override int TileHeight => tileHeight;

        public override int MinimumLevelOfDetail => minimumLevelOfDetail;

        public override int MaximumLevelOfDetail => maximumLevelOfDetail;

        public override Rect? Clip { get; set; }

        public override Matrix3D Transform => transform;

        public override event EventHandler NewTilesAvailable;

        public override void SetRelevantTiles(IList<Tuple<TileId, int?>> relevantTiles)
        {
            relevantTileSet.SetRelevantTiles(relevantTiles);
            rasterTileImageCache.SetRelevantTileSet(relevantTileSet);
            foreach (var removedTile in relevantTileSet.RemovedTiles)
            {
                if (tileRenderables.TryGetValue(removedTile, out var tileRenderable))
                {
                    if (tileRenderable.Element is Image element)
                        imagePool.Add(new Size(element.Width, element.Height), element);
                    tileRenderable.DetachFromElement();
                    tileRenderables.Remove(removedTile);
                }
            }
        }

        public override void GetTile(TileId tileId, out TileRenderable tileRenderable, out bool tileWillNeverBeAvailable)
        {
            if (!relevantTileSet.Contains(tileId))
                throw new InvalidOperationException("Cannot get a tile that is not currently set as relevant.");
            tileRenderable = null;
            tileWillNeverBeAvailable = false;
            var rasterTileCacheValue = rasterTileImageCache.GetValue(tileId);
            if (rasterTileCacheValue is null)
                return;
            if (rasterTileCacheValue.Image is null)
            {
                tileWillNeverBeAvailable = true;
            }
            else
            {
                if (tileRenderables.TryGetValue(tileId, out tileRenderable))
                    return;
                var newToCache = DateTime.UtcNow.Subtract(rasterTileCacheValue.TimeAdded).TotalMilliseconds < 500.0;
                tileRenderable = CreateTileRenderable(tileId, rasterTileCacheValue.Image, rasterTileCacheValue.TileSubregion, newToCache);
                tileRenderables.Add(tileId, tileRenderable);
            }
        }

        public Dictionary<string, string> MostRelevantTileMetadata
        {
            get
            {
                foreach (var relevantTile in relevantTileSet.RelevantTiles)
                {
                    var rasterTileCacheValue = rasterTileImageCache.GetValue(relevantTile.Item1);
                    if (rasterTileCacheValue is object)
                        return rasterTileCacheValue.Metadata;
                }
                return null;
            }
        }

        private TileRenderable CreateTileRenderable(TileId tileId, BitmapSource bitmapSource, Rect tileSubregion, bool newToCache)
        {
            FrameworkElement element;
            if (tileSubregion.X == 1.0 && tileSubregion.Y == 1.0 && (tileSubregion.Width == bitmapSource.PixelWidth - 2 && tileSubregion.Height == bitmapSource.PixelHeight - 2))
            {
                var image1 = imagePool.Get(new Size(bitmapSource.PixelWidth, bitmapSource.PixelHeight));
                if (image1 is null)
                {
                    var image2 = new Image
                    {
                        Width = bitmapSource.PixelWidth,
                        Height = bitmapSource.PixelHeight
                    };
                    image1 = image2;
                }
                image1.Source = bitmapSource;
                image1.Stretch = Stretch.None;
                image1.IsHitTestVisible = false;
                element = image1;
            }
            else
            {
                var rectangle1 = new Rectangle();
                var rectangle2 = rectangle1;
                var imageBrush1 = new ImageBrush
                {
                    ImageSource = bitmapSource,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    Stretch = Stretch.None,
                    Transform = new TranslateTransform()
                    {
                        X = (-tileSubregion.X + 1.0),
                        Y = (-tileSubregion.Y + 1.0)
                    }
                };
                var imageBrush2 = imageBrush1;
                rectangle2.Fill = imageBrush2;
                rectangle1.IsHitTestVisible = false;
                rectangle1.Width = tileSubregion.Width + 2.0;
                rectangle1.Height = tileSubregion.Height + 2.0;
                element = rectangle1;
            }
            var flag = tileId.LevelOfDetail > MinimumLevelOfDetail + 1;
            return new TileRenderable(tileId, element, flag ? TileFadeInDuration : new TimeSpan?());
        }
    }
}
