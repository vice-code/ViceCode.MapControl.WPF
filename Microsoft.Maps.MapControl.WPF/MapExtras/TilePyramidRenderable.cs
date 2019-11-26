using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapExtras
{
    internal class TilePyramidRenderable
    {
        public static readonly ChooseLevelOfDetailSettings ChooseLevelOfDetailSettingsDownloadNormal = (renderLevelOfDetail, levelOfDetail, minimumLevelOfDetail) =>
       {
           var ofDetailSettings = new LevelOfDetailSettings(levelOfDetail >= renderLevelOfDetail - 5, 1.0, new int?(int.MinValue));
           if (levelOfDetail == Math.Max(renderLevelOfDetail - 2, minimumLevelOfDetail))
               ofDetailSettings.DownloadPriority = new int?(1000);
           else if (levelOfDetail == renderLevelOfDetail)
               ofDetailSettings.DownloadPriority = new int?(0);
           else if (levelOfDetail >= renderLevelOfDetail - 5)
               ofDetailSettings.DownloadPriority = new int?(-1000);
           return new LevelOfDetailSettings?(ofDetailSettings);
       };
        public static readonly ChooseLevelOfDetailSettings ChooseLevelOfDetailSettingsDownloadInMotion = (renderLevelOfDetail, levelOfDetail, minimumLevelOfDetail) =>
       {
           var ofDetailSettings = new LevelOfDetailSettings(levelOfDetail >= renderLevelOfDetail - 5, 1.0, new int?());
           if (levelOfDetail == Math.Max(renderLevelOfDetail - 5, minimumLevelOfDetail))
               ofDetailSettings.DownloadPriority = new int?(1000);
           else if (levelOfDetail == renderLevelOfDetail - 3)
               ofDetailSettings.DownloadPriority = new int?(0);
           else if (levelOfDetail == renderLevelOfDetail)
               ofDetailSettings.DownloadPriority = new int?(-1000);
           return new LevelOfDetailSettings?(ofDetailSettings);
       };
        public static readonly ChooseLevelOfDetailSettings ChooseLevelOfDetailSettingsDownloadNothing = (renderLevelOfDetail, levelOfDetail, minimumLevelOfDetail) => new LevelOfDetailSettings?(new LevelOfDetailSettings(levelOfDetail >= renderLevelOfDetail - 5, 1.0, new int?()));
        private readonly TilePriorityCalculator tilePriorityCalculator = new TilePriorityCalculator();
        private readonly List<TileId> visibleTilesAtRenderLod = new List<TileId>();
        private readonly HashSet<TileId> visibleTilesAtEffectiveRenderLod = new HashSet<TileId>();
        private readonly Point2D[] visibleRenderablePoly = new Point2D[10];
        private readonly Point2D[] screenPoly = new Point2D[10];
        private ICollection<TileStatus> _tileStatuses = new List<TileStatus>();
        private readonly Func<TileStatus, bool> VisibleTilePredicate = tile => tile.Visible;
        private readonly Func<TileStatus, bool> MissingTilePredicate = tile =>
       {
           if (tile.WillNeverBeAvailable)
               return false;
           if (tile.Available)
               return !tile.FullyOpaque;
           return true;
       };
        private const int MaxOversampleLevels = 5;
        private readonly Canvas tilePyramidCanvas;
        private TileSource tileSource;
        private TilePyramidDescriptor tilePyramidDescriptor;
        private TilePyramid tilePyramid;
        private int _effectiveRenderLod;
        private bool?[] lodAvailabilities;
        private Rect screenRect;
        private Matrix3D tilePyramidToViewportTransform;
        private ChooseLevelOfDetailSettings chooseLevelOfDetailSettings;

        public TilePyramidRenderable(Canvas parentCanvas)
        {
            tilePyramidCanvas = parentCanvas;
            tilePyramidToViewportTransform = Matrix3D.Identity;
            screenRect = new Rect(0.0, 0.0, 0.0, 0.0);
            chooseLevelOfDetailSettings = ChooseLevelOfDetailSettingsDownloadNormal;
        }

        public TileSource TileSource
        {
            set
            {
                if (tileSource is object)
                {
                    tilePyramid.DetachTileSource();
                    tilePyramid.NeedsRender -= new EventHandler(TilePyramid_NeedsRender);
                    tilePyramid = null;
                    tilePyramidDescriptor = null;
                    tileSource = null;
                }
                tileSource = value;
                if (tileSource is object)
                {
                    tilePyramidDescriptor = new TilePyramidDescriptor(tileSource.FinestLodWidth, tileSource.FinestLodHeight, tileSource.MinimumLevelOfDetail, tileSource.TileWidth, tileSource.TileHeight);
                    tilePyramid = new TilePyramid(tilePyramidDescriptor, tileSource, tilePyramidCanvas)
                    {
                        ChooseLevelOfDetailSettings = chooseLevelOfDetailSettings
                    };
                    tilePyramid.NeedsRender += new EventHandler(TilePyramid_NeedsRender);
                    lodAvailabilities = new bool?[tileSource.MaximumLevelOfDetail + 1];
                }
                else
                {
                    tilePyramidDescriptor = null;
                    tilePyramid = null;
                    lodAvailabilities = null;
                }
            }
            get => tileSource;
        }

        public bool ShowBackgroundTiles { get; set; } = true;

        public Matrix3D NormalizedTilePyramidToToViewportTransform
        {
            set
            {
                tilePyramidToViewportTransform = value;
                tilePyramidToViewportTransform.ScalePrepend(new Vector3D(1.0 / tilePyramidDescriptor.FinestLevelWidth, 1.0 / tilePyramidDescriptor.FinestLevelHeight, 1.0));
                FireNeedsRender();
            }
        }

        public double Opacity
        {
            get => tilePyramid.Opacity;
            set => tilePyramid.Opacity = value;
        }

        public void Render(Point2D viewportSize)
        {
            if (tileSource is null)
                return;
            tilePyramidDescriptor.GetVisibleTiles(viewportSize, ref tilePyramidToViewportTransform, false, visibleTilesAtRenderLod, visibleRenderablePoly, screenPoly, out var screenSpacePolyVertexCount, out var preciseRenderLod, out var renderLod, out var finestLodNeeded);
            UpdateLodAvailabilitiesAndEffectiveRenderLod(renderLod);
            if (visibleTilesAtRenderLod.Count > 0)
                tilePriorityCalculator.Initialize(tilePyramidDescriptor, ref tilePyramidToViewportTransform, viewportSize, visibleTilesAtRenderLod[0]);
            tilePyramid.Render(viewportSize, ref tilePyramidToViewportTransform, visibleTilesAtRenderLod, tilePriorityCalculator, preciseRenderLod, renderLod, ShowBackgroundTiles);
            _tileStatuses = tilePyramid.GetTiles();
        }

        public ChooseLevelOfDetailSettings ChooseLevelOfDetailSettings
        {
            get => chooseLevelOfDetailSettings;
            set
            {
                chooseLevelOfDetailSettings = value;
                if (tilePyramid is null)
                    return;
                tilePyramid.ChooseLevelOfDetailSettings = chooseLevelOfDetailSettings;
            }
        }

        public event EventHandler NeedsRender;

        public bool IsIdle
        {
            get
            {
                if (tilePyramid is object)
                    return !_tileStatuses.Where(MissingTilePredicate).Any(tile => tile.TileId.LevelOfDetail == _effectiveRenderLod);
                return true;
            }
        }

        public bool HasSomeTiles
        {
            get
            {
                if (tilePyramid is null)
                    return true;
                if (visibleTilesAtEffectiveRenderLod.Count > 0)
                    return _tileStatuses.Count(VisibleTilePredicate) >= visibleTilesAtEffectiveRenderLod.Count / 4;
                return false;
            }
        }

        private void TilePyramid_NeedsRender(object sender, EventArgs e) => FireNeedsRender();

        private void FireNeedsRender()
        {
            if (NeedsRender is null)
                return;
            NeedsRender(this, EventArgs.Empty);
        }

        private void UpdateLodAvailabilitiesAndEffectiveRenderLod(int renderLod)
        {
            var val1_1 = int.MinValue;
            var val1_2 = int.MaxValue;
            for (var index = 0; index < lodAvailabilities.Length; ++index)
                lodAvailabilities[index] = new bool?();
            foreach (var tileStatuse in _tileStatuses)
            {
                if (tileStatuse.Available)
                {
                    lodAvailabilities[tileStatuse.TileId.LevelOfDetail] = new bool?(true);
                    val1_1 = Math.Max(val1_1, tileStatuse.TileId.LevelOfDetail);
                }
                else if (tileStatuse.WillNeverBeAvailable)
                {
                    if (!lodAvailabilities[tileStatuse.TileId.LevelOfDetail].HasValue)
                        lodAvailabilities[tileStatuse.TileId.LevelOfDetail] = new bool?(false);
                    val1_2 = Math.Min(val1_2, tileStatuse.TileId.LevelOfDetail);
                }
            }
            _effectiveRenderLod = Math.Min(renderLod, Math.Max(val1_1, val1_2 - 1));
            visibleTilesAtEffectiveRenderLod.Clear();
            if (visibleTilesAtRenderLod.Count <= 0 || _effectiveRenderLod < tileSource.MinimumLevelOfDetail)
                return;
            var num = renderLod - _effectiveRenderLod;
            foreach (var tileId in visibleTilesAtRenderLod)
                visibleTilesAtEffectiveRenderLod.Add(new TileId(_effectiveRenderLod, tileId.X >> num, tileId.Y >> num));
        }
    }
}
