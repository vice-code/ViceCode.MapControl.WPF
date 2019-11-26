using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapExtras
{
    internal class TilePyramid
    {
        private double tileOverlap = 0.5;
        private readonly List<Tuple<TileId, int?>> explicitRelevantTiles = new List<Tuple<TileId, int?>>();
        private readonly Dictionary<TileId, TileRecord> tiles = new Dictionary<TileId, TileRecord>();
        private readonly List<KeyValuePair<TileId, TileRecord>> tilesToRemove = new List<KeyValuePair<TileId, TileRecord>>();
        private readonly Dictionary<TileId, int?> relevantTilesByTileId = new Dictionary<TileId, int?>();
        private readonly List<Tuple<TileId, int?>> relevantTiles = new List<Tuple<TileId, int?>>();
        private bool allowHardwareAcceleration;
        private bool overlapPyramidEdges;
        private Rect? clip;
        private readonly Canvas parentCanvas;
        private readonly Canvas tilePyramidCanvas;
        private int frameCount;
        private readonly List<LevelOfDetailSettings?> levelOfDetailSettings;
        private readonly TilePyramidCoverageMap coverageMap;

        public TilePyramid(
          TilePyramidDescriptor tilePyramidDescriptor,
          TileSource tileSource,
          Canvas parentCanvas)
        {
            if (tilePyramidDescriptor is null)
                throw new ArgumentNullException(nameof(tilePyramidDescriptor));
            if (tileSource is null)
                throw new ArgumentNullException(nameof(tileSource));
            if (parentCanvas is null)
                throw new ArgumentNullException(nameof(parentCanvas));
            if (tilePyramidDescriptor.FinestLevelWidth != tileSource.FinestLodWidth || tilePyramidDescriptor.FinestLevelHeight != tileSource.FinestLodHeight)
                throw new ArgumentException("Tile pyramid descriptor and tile source sizes must match.");
            TilePyramidDescriptor = tilePyramidDescriptor;
            this.parentCanvas = parentCanvas;
            TileSource = tileSource;
            TileSource.NewTilesAvailable += new EventHandler(TileSource_NewTilesAvailable);
            var canvas = new Canvas
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                UseLayoutRounding = false,
                IsHitTestVisible = false,
                Tag = "TilePyramidCanvas"
            };
            tilePyramidCanvas = canvas;
            Canvas.SetLeft(tilePyramidCanvas, 0.0);
            Canvas.SetTop(tilePyramidCanvas, 0.0);
            this.parentCanvas.Children.Add(tilePyramidCanvas);
            ChooseLevelOfDetailSettings = DownloadFinestNLevelsOfDetailNeeded(3, true, true);
            levelOfDetailSettings = new List<LevelOfDetailSettings?>(tileSource.MaximumLevelOfDetail);
            for (var index = 0; index <= tileSource.MaximumLevelOfDetail; ++index)
                levelOfDetailSettings.Add(new LevelOfDetailSettings?(new LevelOfDetailSettings(false, 0.0, new int?())));
            coverageMap = new TilePyramidCoverageMap(tileSource.MinimumLevelOfDetail, tileSource.MaximumLevelOfDetail);
        }

        public TilePyramidDescriptor TilePyramidDescriptor { get; }

        public TileSource TileSource { get; private set; }

        public IList<Tuple<TileId, int?>> ExplicitRelevantTiles
        {
            get => new List<Tuple<TileId, int?>>(explicitRelevantTiles);
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                explicitRelevantTiles.Clear();
                explicitRelevantTiles.AddRange(value);
                if (NeedsRender is null)
                    return;
                NeedsRender(this, EventArgs.Empty);
            }
        }

        public ChooseLevelOfDetailSettings ChooseLevelOfDetailSettings { get; set; }

        public static ChooseLevelOfDetailSettings DownloadFinestNLevelsOfDetailNeeded(
          int numberOfFinestLevelsOfDetailToDownloadFirst,
          bool downloadTheRest,
          bool showTheRest)
        {
            if (numberOfFinestLevelsOfDetailToDownloadFirst <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfFinestLevelsOfDetailToDownloadFirst));
            return (renderLevelOfDetail, levelOfDetail, minimumLevelOfDetail) =>
           {
               var ofDetailSettings = new LevelOfDetailSettings(true, 1.0, new int?(renderLevelOfDetail - levelOfDetail));
               var downloadPriority = ofDetailSettings.DownloadPriority;
               var num1 = numberOfFinestLevelsOfDetailToDownloadFirst - 1;
               if ((downloadPriority.GetValueOrDefault() <= num1 ? 0 : (downloadPriority.HasValue ? 1 : 0)) != 0)
               {
                   ref LevelOfDetailSettings local = ref ofDetailSettings;
                   int? nullable1;
                   if (!downloadTheRest)
                   {
                       nullable1 = new int?();
                   }
                   else
                   {
                       var nullable2 = new int?(minimumLevelOfDetail);
                       var num2 = levelOfDetail;
                       var nullable3 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault() - num2) : new int?();
                       nullable1 = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault() - 1) : new int?();
                   }
                   local.DownloadPriority = nullable1;
                   ofDetailSettings.Visible = showTheRest;
               }
               if (ofDetailSettings.DownloadPriority.HasValue)
                   ofDetailSettings.DownloadPriority = new int?(ofDetailSettings.DownloadPriority.Value * 5);
               return new LevelOfDetailSettings?(ofDetailSettings);
           };
        }

        public event EventHandler NeedsRender;

        public double Opacity
        {
            get => parentCanvas.Opacity;
            set => parentCanvas.Opacity = value;
        }

        public double TileOverlap
        {
            get => tileOverlap;
            set
            {
                if (tileOverlap == value)
                    return;
                tileOverlap = value;
                foreach (var tile in tiles)
                    tile.Value.TileOverlap = tileOverlap;
            }
        }

        public bool OverlapPyramidEdges
        {
            get => overlapPyramidEdges;
            set
            {
                overlapPyramidEdges = value;
                foreach (var tile in tiles)
                    tile.Value.OverlapPyramidEdges = overlapPyramidEdges;
            }
        }

        public bool AllowHardwareAcceleration
        {
            get => allowHardwareAcceleration;
            set
            {
                allowHardwareAcceleration = value;
                foreach (var tile in tiles)
                    tile.Value.AllowHardwareAcceleration = allowHardwareAcceleration;
            }
        }

        public void Render(
      Point2D viewportSize,
      ref Matrix3D tilePyramidToViewport,
      IList<TileId> visibleTiles,
      TilePriorityCalculator tilePriorityCalculator,
      double preciseRenderLod,
      int renderLod,
      bool showBackgroundTiles)
        {
            if (TileSource is null)
                throw new InvalidOperationException("Tile source has been detached.");
            ++frameCount;
            for (var levelOfDetail = renderLod; levelOfDetail >= TileSource.MinimumLevelOfDetail; --levelOfDetail)
                levelOfDetailSettings[levelOfDetail] = ChooseLevelOfDetailSettings(renderLod, levelOfDetail, TileSource.MinimumLevelOfDetail);
            var clip1 = clip;
            var clip2 = TileSource.Clip;
            if ((clip1.HasValue != clip2.HasValue ? 1 : (!clip1.HasValue ? 0 : (clip1.GetValueOrDefault() != clip2.GetValueOrDefault() ? 1 : 0))) != 0)
            {
                clip = TileSource.Clip;
                if (clip.HasValue)
                {
                    var rect = clip.Value;
                    TilePyramidDescriptor.ClipPoly = new Point2D[4]
                    {
            new Point2D(rect.Left, rect.Top),
            new Point2D(rect.Left, rect.Bottom),
            new Point2D(rect.Right, rect.Bottom),
            new Point2D(rect.Right, rect.Top)
                    };
                }
                else
                    TilePyramidDescriptor.ClipPoly = TilePyramidDescriptor.DefaultClipPoly;
                foreach (var tile in tiles)
                    tile.Value.TilePyramidClip = clip;
            }
            if (visibleTiles.Count > 0)
            {
                var num1 = long.MaxValue;
                var num2 = long.MaxValue;
                var num3 = long.MinValue;
                var num4 = long.MinValue;
                foreach (var visibleTile in visibleTiles)
                {
                    num1 = Math.Min(num1, visibleTile.X);
                    num2 = Math.Min(num2, visibleTile.Y);
                    num3 = Math.Max(num3, visibleTile.X + 1L);
                    num4 = Math.Max(num4, visibleTile.Y + 1L);
                }
                coverageMap.Intialize(renderLod, num1, num2, num3, num4);
            }
            foreach (var visibleTile in visibleTiles)
            {
                if (visibleTile.LevelOfDetail >= TileSource.MinimumLevelOfDetail)
                {
                    var priority = tilePriorityCalculator.GetPriority(visibleTile);
                    var levelOfDetailSetting1 = levelOfDetailSettings[visibleTile.LevelOfDetail];
                    if (levelOfDetailSetting1.HasValue)
                    {
                        if (!tiles.TryGetValue(visibleTile, out var tileRecord))
                        {
                            tileRecord = new TileRecord(tilePyramidCanvas, TilePyramidDescriptor, visibleTile)
                            {
                                AllowHardwareAcceleration = allowHardwareAcceleration,
                                OverlapPyramidEdges = overlapPyramidEdges,
                                TileOverlap = tileOverlap,
                                TilePyramidClip = clip
                            };
                            tileRecord.NeedsRender += new EventHandler(TileRecord_NeedsRender);
                            tiles.Add(visibleTile, tileRecord);
                        }
                        tileRecord.TargetOpacity = !levelOfDetailSetting1.Value.Visible ? 0.0 : levelOfDetailSetting1.Value.TargetOpacity;
                        tileRecord.LastTouched = frameCount;
                        AddToRelevantTiles(visibleTile, levelOfDetailSetting1, priority);
                    }
                    coverageMap.MarkAsOccluder(visibleTile, false);
                    if (showBackgroundTiles)
                    {
                        var tileId = visibleTile;
                        while (tileId.LevelOfDetail > TileSource.MinimumLevelOfDetail)
                        {
                            tileId = tileId.GetParent();
                            var levelOfDetailSetting2 = levelOfDetailSettings[tileId.LevelOfDetail];
                            if (levelOfDetailSetting2.HasValue)
                            {
                                if (!tiles.TryGetValue(tileId, out var tileRecord))
                                {
                                    tileRecord = new TileRecord(tilePyramidCanvas, TilePyramidDescriptor, tileId)
                                    {
                                        AllowHardwareAcceleration = allowHardwareAcceleration,
                                        OverlapPyramidEdges = overlapPyramidEdges,
                                        TileOverlap = tileOverlap,
                                        TilePyramidClip = clip
                                    };
                                    tileRecord.NeedsRender += new EventHandler(TileRecord_NeedsRender);
                                    tiles.Add(tileId, tileRecord);
                                    AddToRelevantTiles(tileId, levelOfDetailSetting2, priority);
                                }
                                tileRecord.TargetOpacity = !levelOfDetailSetting2.Value.Visible ? 0.0 : levelOfDetailSetting2.Value.TargetOpacity;
                                if (tileRecord.LastTouched != frameCount)
                                {
                                    AddToRelevantTiles(tileId, levelOfDetailSetting2, priority);
                                    tileRecord.LastTouched = frameCount;
                                }
                            }
                            coverageMap.MarkAsOccluder(tileId, false);
                        }
                    }
                }
            }
            foreach (var tile in tiles)
            {
                if (tile.Value.LastTouched != frameCount)
                    tilesToRemove.Add(tile);
            }
            foreach (var keyValuePair in tilesToRemove)
            {
                PrepareTileForRemoval(keyValuePair.Value);
                tiles.Remove(keyValuePair.Key);
            }
            tilesToRemove.Clear();
            foreach (var explicitRelevantTile in explicitRelevantTiles)
                AddToRelevantTiles(explicitRelevantTile.Item1, explicitRelevantTile.Item2);
            foreach (var keyValuePair in relevantTilesByTileId)
                relevantTiles.Add(Tuple.Create(keyValuePair.Key, keyValuePair.Value));
            relevantTilesByTileId.Clear();
            TileSource.SetRelevantTiles(new ReadOnlyCollection<Tuple<TileId, int?>>(relevantTiles));
            relevantTiles.Clear();
            foreach (var tile in tiles)
            {
                if (tile.Value.TileRenderable is null && !tile.Value.WillNeverBeAvailable)
                {
                    TileSource.GetTile(tile.Key, out var tileRenderable, out var tileWillNeverBeAvailable);
                    if (tileWillNeverBeAvailable)
                        tile.Value.WillNeverBeAvailable = true;
                    else if (tileRenderable is object)
                        tile.Value.TileRenderable = tileRenderable;
                }
            }
            if (tiles.Count > 0)
            {
                foreach (var tile in tiles)
                {
                    if (tile.Value.FullyOpaque)
                        coverageMap.MarkAsOccluder(tile.Key, true);
                }
                coverageMap.CalculateOcclusions();
            }
            foreach (var tile in tiles)
            {
                var tileRecord = tile.Value;
                if (!levelOfDetailSettings[tile.Key.LevelOfDetail].Value.Visible || tileRecord.TileRenderable is null && !tileRecord.WillNeverBeAvailable || coverageMap.IsOccludedByDescendents(tile.Key))
                {
                    if (tileRecord.Visible)
                        tileRecord.NoLongerRendering();
                    tileRecord.Visible = false;
                }
                else
                {
                    if (!tileRecord.Visible)
                        tileRecord.Visible = true;
                    tileRecord.Render(viewportSize, ref tilePyramidToViewport, preciseRenderLod);
                }
            }
        }

        public ICollection<TileStatus> GetTiles()
        {
            var tileStatusList = new List<TileStatus>();
            foreach (var tile in tiles)
                tileStatusList.Add(new TileStatus()
                {
                    TileId = tile.Key,
                    Visible = tile.Value.Visible,
                    Available = tile.Value.TileRenderable is object,
                    WillNeverBeAvailable = tile.Value.WillNeverBeAvailable,
                    FullyOpaque = tile.Value.FullyOpaque
                });
            return tileStatusList;
        }

        public void ClearAllTiles()
        {
            foreach (var tile in tiles)
                PrepareTileForRemoval(tile.Value);
            tiles.Clear();
            TileSource.SetRelevantTiles(relevantTiles);
        }

        public void DetachTileSource()
        {
            ClearAllTiles();
            TileSource.NewTilesAvailable -= new EventHandler(TileSource_NewTilesAvailable);
            TileSource = null;
        }

        private static int? MaxPriority(int? priority0, int? priority1)
        {
            if (priority0.HasValue && priority1.HasValue)
                return new int?(Math.Max(priority0.Value, priority1.Value));
            if (priority0.HasValue)
                return priority0;
            return priority1;
        }

        private void AddToRelevantTiles(
          TileId tileId,
          LevelOfDetailSettings? levelOfDetailSettings,
          int intraLodTilePriority)
        {
            var priority = new int?();
            if (levelOfDetailSettings.HasValue)
            {
                priority = levelOfDetailSettings.Value.DownloadPriority;
                if (priority.HasValue)
                    priority = new int?(priority.Value + intraLodTilePriority);
            }
            AddToRelevantTiles(tileId, priority);
        }

        private void AddToRelevantTiles(TileId tileId, int? priority)
        {
            if (relevantTilesByTileId.TryGetValue(tileId, out var priority0))
            {
                priority = MaxPriority(priority0, priority);
                var nullable1 = priority;
                var nullable2 = priority0;
                if ((nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : (nullable1.HasValue != nullable2.HasValue ? 1 : 0)) == 0)
                    return;
                relevantTilesByTileId[tileId] = priority;
            }
            else
                relevantTilesByTileId[tileId] = priority;
        }

        private void PrepareTileForRemoval(TileRecord tr)
        {
            if (tr.Visible)
                tr.NoLongerRendering();
            tr.Visible = false;
            tr.TileRenderable = null;
            tr.NeedsRender -= new EventHandler(TileRecord_NeedsRender);
        }

        private void TileRecord_NeedsRender(object sender, EventArgs e)
        {
            if (NeedsRender is null)
                return;
            NeedsRender(this, EventArgs.Empty);
        }

        private void TileSource_NewTilesAvailable(object sender, EventArgs e)
        {
            if (NeedsRender is null)
                return;
            NeedsRender(this, EventArgs.Empty);
        }
    }
}
