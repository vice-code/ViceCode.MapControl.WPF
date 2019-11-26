using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Microsoft.Maps.MapExtras;

namespace Microsoft.Maps.MapControl.WPF
{
    public class MapTileLayer : Canvas, IProjectable, IAttachable
    {
        private readonly TileWrap tileWrap = TileWrap.Horizontal;
        private const int Log2DuplicatePyramidCount = 4;
        private TileSource tileSource;
        private RasterTileSource rasterTileSource;
        private TilePyramidRenderable _TilePyramidRenderable;

        public MapTileLayer() => IsHitTestVisible = false;

        public int TileWidth { get; set; } = 256;

        public int TileHeight { get; set; } = 256;

        public bool ShowBackgroundTiles { get; set; } = true;

        public TileSource TileSource
        {
            get => tileSource;
            set
            {
                tileSource = value;
                EnsureTileSource();
            }
        }

        void IAttachable.Attach()
        {
            if (_TilePyramidRenderable is object || tileSource is null)
                return;
            _TilePyramidRenderable = new TilePyramidRenderable(this)
            {
                ShowBackgroundTiles = ShowBackgroundTiles
            };
            _TilePyramidRenderable.NeedsRender += new EventHandler(_TilePyramidRenderable_NeedsRender);
            Loaded += new RoutedEventHandler(MapTileLayer_Loaded);
            EnsureTileSource();
        }

        void IAttachable.Detach()
        {
            if (_TilePyramidRenderable is null)
                return;
            _TilePyramidRenderable.TileSource = null;
            _TilePyramidRenderable.NeedsRender -= new EventHandler(_TilePyramidRenderable_NeedsRender);
            _TilePyramidRenderable = null;
            Loaded -= new RoutedEventHandler(MapTileLayer_Loaded);
        }

        void IProjectable.SetView(
          Size viewportSize,
          Matrix3D normalizedMercatorToViewport,
          Matrix3D viewportToNormalizedMercator)
        {
            if (tileWrap == TileWrap.None)
            {
                _TilePyramidRenderable.NormalizedTilePyramidToToViewportTransform = normalizedMercatorToViewport;
            }
            else
            {
                var num = 16;
                var flag1 = tileWrap == TileWrap.Horizontal || tileWrap == TileWrap.Both;
                var flag2 = tileWrap == TileWrap.Vertical || tileWrap == TileWrap.Both;
                _TilePyramidRenderable.NormalizedTilePyramidToToViewportTransform = VectorMath.ScalingMatrix3D(num, num, 1.0) * VectorMath.TranslationMatrix3D(flag1 ? -num / 2 : 0.0, flag2 ? -num / 2 : 0.0, 0.0) * normalizedMercatorToViewport;
            }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);
            InternalRender();
            return size;
        }

        private void InternalRender()
        {
            if (rasterTileSource is null || _TilePyramidRenderable is null || _TilePyramidRenderable.TileSource is null)
                return;
            _TilePyramidRenderable.Render(new Point2D(ActualWidth, ActualHeight));
        }

        private void EnsureTileSource()
        {
            if (tileSource is null || _TilePyramidRenderable is null)
                return;
            var rasterTileDownloader = (RasterTileDownloader)new GenericRasterTileDownloader(tileSource, OverlapBorderPresence.None, Dispatcher);
            rasterTileSource = tileWrap != TileWrap.None ? new RasterTileSource(2147483648L, 2147483648L, TileWidth, TileHeight, 9, rasterTileDownloader, tileWrap, 4, false) : new RasterTileSource(2147483648L, 2147483648L, TileWidth, TileHeight, 9, rasterTileDownloader, false);
            _TilePyramidRenderable.TileSource = rasterTileSource;
        }

        private void _TilePyramidRenderable_NeedsRender(object sender, EventArgs e) => InvalidateArrange();

        private void MapTileLayer_Loaded(object sender, RoutedEventArgs e)
        {
            EnsureTileSource();
            InternalRender();
        }
    }
}
