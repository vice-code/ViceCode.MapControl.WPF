using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Microsoft.Maps.MapControl.WPF.Core;
using Microsoft.Maps.MapExtras;

namespace Microsoft.Maps.MapControl.WPF
{
    public abstract class MapMode : Grid, IProjectable
    {
        private const int Log2DuplicatePyramidCount = 4;
        internal const string configurationVersion = "v1";
        internal const string configurationSection = "Services";
        private string _Culture;
        private string _SessionId;
        private readonly Canvas _TilePyramidRenderableCanvas;
        private MapExtras.TileSource _TileSource;
        private TilePyramidRenderable _TilePyramidRenderable;
        private Point? _CurrentMapInstance;
        private TileWrap _TileWrap;

        protected MapMode()
        {
            _TileWrap = TileWrap.None;
            SizeChanged += new SizeChangedEventHandler(MapMode_SizeChanged);
            _TilePyramidRenderableCanvas = new Canvas();
            Children.Add(_TilePyramidRenderableCanvas);
            _TilePyramidRenderable = new TilePyramidRenderable(_TilePyramidRenderableCanvas);
            _TilePyramidRenderable.NeedsRender += new EventHandler(_TilePyramidRenderable_NeedsRender);
        }

        internal TileWrap TileWrap
        {
            get => _TileWrap;
            set
            {
                if (_TileWrap != value && _TileSource is object)
                    throw new InvalidOperationException("Must be set immediately after map mode construction. You can't switch the tile wrap of an initialized map mode.");
                _TileWrap = value;
            }
        }

        internal bool HasSomeTiles => _TilePyramidRenderable.HasSomeTiles;

        public virtual ModeBackground ModeBackground => ModeBackground.Dark;

        internal abstract PlatformServices.MapStyle? MapStyle { get; }

        internal event EventHandler Rendered;

        void IProjectable.SetView(
          Size viewportSize,
          Matrix3D normalizedMercatorToViewport,
          Matrix3D viewportToNormalizedMercator)
        {
            if (_TileSource is object)
                SetViewImpl(viewportSize, normalizedMercatorToViewport, viewportToNormalizedMercator);
            else
                MapConfiguration.GetSection("v1", "Services", Culture, SessionId, new MapConfigurationCallback(AsynchronousConfigurationLoadedSetViewCallback), false, new SetViewParams()
                {
                    ViewportSize = viewportSize,
                    NormalizedMercatorToViewport = normalizedMercatorToViewport,
                    ViewportToNormalizedMercator = viewportToNormalizedMercator
                });
        }

        private void AsynchronousConfigurationLoadedSetViewCallback(
          MapConfigurationSection config,
          object userState)
        {
            EnsureTileSource();
            var setViewParams = (SetViewParams)userState;
            if (setViewParams is null)
                return;
            SetViewImpl(setViewParams.ViewportSize, setViewParams.NormalizedMercatorToViewport, setViewParams.ViewportToNormalizedMercator);
        }

        private void SetViewImpl(
          Size viewportSize,
          Matrix3D normalizedMercatorToViewport,
          Matrix3D viewportToNormalizedMercator)
        {
            if (TileWrap == TileWrap.None)
            {
                _TilePyramidRenderable.NormalizedTilePyramidToToViewportTransform = normalizedMercatorToViewport;
            }
            else
            {
                var num = 16;
                var flag1 = TileWrap == TileWrap.Horizontal || TileWrap == TileWrap.Both;
                var flag2 = TileWrap == TileWrap.Vertical || TileWrap == TileWrap.Both;
                _TilePyramidRenderable.NormalizedTilePyramidToToViewportTransform = VectorMath.ScalingMatrix3D(num, num, 1.0) * VectorMath.TranslationMatrix3D(flag1 ? -num / 2 : 0.0, flag2 ? -num / 2 : 0.0, 0.0) * normalizedMercatorToViewport;
            }
        }

        internal abstract void AsynchronousConfigurationLoaded(
          MapConfigurationSection config,
          object userState);

        internal string Culture
        {
            get
            {
                if (!string.IsNullOrEmpty(_Culture))
                    return _Culture;
                return CultureInfo.CurrentUICulture.Name;
            }
            set => _Culture = value;
        }

        internal string SessionId
        {
            get => _SessionId;
            set
            {
                _SessionId = value;
                MapConfiguration.GetSection("v1", "Services", Culture, SessionId, new MapConfigurationCallback(AsynchronousConfigurationLoaded), true);
            }
        }

        internal abstract string TileUriFormat { get; }

        internal abstract string Subdomains { get; }

        internal ChooseLevelOfDetailSettings ChooseLevelOfDetailSettings
        {
            get => _TilePyramidRenderable.ChooseLevelOfDetailSettings;
            set
            {
                if (_TilePyramidRenderable is null)
                    return;
                _TilePyramidRenderable.ChooseLevelOfDetailSettings = value;
            }
        }

        internal void Detach()
        {
            _TilePyramidRenderable.TileSource = null;
            _TilePyramidRenderable.NeedsRender -= new EventHandler(_TilePyramidRenderable_NeedsRender);
            _TilePyramidRenderable = null;
        }

        internal Point CurrentMapCopyInstance
        {
            set => _CurrentMapInstance = new Point?(value);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);
            InternalRender();
            return size;
        }

        private void EnsureTileSource()
        {
            if (_TileSource is object)
                return;
            InitializeTileSource();
        }

        protected void RebuildTileSource()
        {
            if (_TileSource is object)
                InitializeTileSource();
            InternalRender();
        }

        private void InitializeTileSource()
        {
            if (string.IsNullOrEmpty(TileUriFormat))
                MapConfiguration.GetSection("v1", "Services", Culture, SessionId, new MapConfigurationCallback(AsynchronousConfigurationLoaded), true);
            var tileSource = new TileSource(TileUriFormat);
            if (Subdomains is object && TryParseSubdomains(Subdomains, out var subdomains))
                tileSource.SetSubdomains(subdomains);
            var rasterTileDownloader = (RasterTileDownloader)new GenericRasterTileDownloader(tileSource, OverlapBorderPresence.None, Dispatcher);
            _TileSource = TileWrap != TileWrap.None ? new RasterTileSource(2147483648L, 2147483648L, 256, 256, 9, rasterTileDownloader, TileWrap, 4, false) : new RasterTileSource(2147483648L, 2147483648L, 256, 256, 9, rasterTileDownloader, false);
            _TilePyramidRenderable.TileSource = _TileSource;
        }

        private void MapMode_SizeChanged(object sender, SizeChangedEventArgs e) => InternalRender();

        private void _TilePyramidRenderable_NeedsRender(object sender, EventArgs e) => InvalidateArrange();

        private void InternalRender()
        {
            if (_TileSource is null || _TilePyramidRenderable is null || _TilePyramidRenderable.TileSource is null)
                return;
            _TilePyramidRenderable.Render(new Point2D(ActualWidth, ActualHeight));
            if (Rendered is null)
                return;
            Rendered(this, EventArgs.Empty);
        }

        private static bool TryParseSubdomains(string subdomainString, out string[][] subdomains)
        {
            var flag = false;
            if (!string.IsNullOrEmpty(subdomainString))
            {
                var strArray = subdomainString.Split(' ');
                subdomains = new string[strArray.Length][];
                for (var index = 0; index < strArray.Length; ++index)
                    subdomains[index] = strArray[index].Split(',');
                flag = true;
            }
            else
                subdomains = null;
            return flag;
        }

        private class SetViewParams
        {
            public Size ViewportSize;
            public Matrix3D NormalizedMercatorToViewport;
            public Matrix3D ViewportToNormalizedMercator;
        }
    }
}
