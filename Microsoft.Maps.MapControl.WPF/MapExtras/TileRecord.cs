using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapExtras
{
    internal class TileRecord
    {
        private readonly Canvas tilePyramidCanvas;
        private readonly TilePyramidDescriptor tilePyramidDescriptor;
        private Rect? tilePyramidClip;
        private readonly TileId tileId;
        private TileRenderable tileRenderable;
        private bool allowHardwareAcceleration;
        private double tileOverlap;
        private bool overlapPyramidEdges;
        private bool visible;
        private double targetOpacity;

        public int LastTouched { get; set; }

        public TileRecord(
          Canvas layerCanvas,
          TilePyramidDescriptor tilePyramidDescriptor,
          TileId tileId)
        {
            tilePyramidCanvas = layerCanvas;
            this.tilePyramidDescriptor = tilePyramidDescriptor;
            this.tileId = tileId;
            visible = false;
            WillNeverBeAvailable = false;
        }

        public bool AllowHardwareAcceleration
        {
            get => allowHardwareAcceleration;
            set
            {
                allowHardwareAcceleration = value;
                EnsureTileRenderableConfiguration();
            }
        }

        public Rect? TilePyramidClip
        {
            get => tilePyramidClip;
            set
            {
                tilePyramidClip = value;
                EnsureTileRenderableConfiguration();
            }
        }

        public double TileOverlap
        {
            get => tileOverlap;
            set
            {
                tileOverlap = value;
                EnsureTileRenderableConfiguration();
            }
        }

        public bool OverlapPyramidEdges
        {
            get => overlapPyramidEdges;
            set
            {
                overlapPyramidEdges = value;
                EnsureTileRenderableConfiguration();
            }
        }

        public event EventHandler NeedsRender;

        public TileRenderable TileRenderable
        {
            get => tileRenderable;
            set
            {
                if (tileRenderable is object)
                    tileRenderable.BecameFullyOpaque -= new EventHandler(TileRenderable_BecameFullyOpaque);
                tileRenderable = value;
                if (tileRenderable is object)
                {
                    tileRenderable.TargetOpacity = targetOpacity;
                    tileRenderable.BecameFullyOpaque += new EventHandler(TileRenderable_BecameFullyOpaque);
                }
                EnsureTileRenderableConfiguration();
            }
        }

        private void TileRenderable_BecameFullyOpaque(object sender, EventArgs e)
        {
            if (NeedsRender is null)
                return;
            NeedsRender(this, EventArgs.Empty);
        }

        public bool WillNeverBeAvailable { get; set; }

        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                if (tileRenderable is null)
                    return;
                if (visible && tileRenderable.LayerCanvas is null)
                    tileRenderable.LayerCanvas = tilePyramidCanvas;
                if (visible || tileRenderable.LayerCanvas is null)
                    return;
                tileRenderable.LayerCanvas = null;
            }
        }

        public double TargetOpacity
        {
            get => targetOpacity;
            set
            {
                targetOpacity = value;
                if (tileRenderable is null)
                    return;
                tileRenderable.TargetOpacity = value;
            }
        }

        public bool FullyOpaque
        {
            get
            {
                if (tileRenderable is null)
                    return false;
                return tileRenderable.Opacity > 0.99;
            }
        }

        public void NoLongerRendering()
        {
            if (tileRenderable is null)
                return;
            tileRenderable.NoLongerRendering();
        }

        public void Render(Point2D viewportSize, ref Matrix3D tilePyramidToViewport, double renderLod)
        {
            if (tileRenderable is null)
                return;
            var point2D = new Point2D(tilePyramidDescriptor.TileWidth * tileId.X - 1L, tilePyramidDescriptor.TileHeight * tileId.Y - 1L);
            var finestLodScaleFactor = LodToFinestLodScaleFactor;
            var tileToViewport = VectorMath.TranslationMatrix3D(point2D.X, point2D.Y, 0.0) * VectorMath.ScalingMatrix3D(finestLodScaleFactor.X, finestLodScaleFactor.Y, 1.0) * tilePyramidToViewport;
            tileRenderable.Render(viewportSize, ref tileToViewport, renderLod);
        }

        private Point2D LodToFinestLodScaleFactor => new Point2D(tilePyramidDescriptor.FinestLevelWidth / (double)tilePyramidDescriptor.GetLevelOfDetailWidth(tileId.LevelOfDetail), tilePyramidDescriptor.FinestLevelHeight / (double)tilePyramidDescriptor.GetLevelOfDetailHeight(tileId.LevelOfDetail));

        private void EnsureTileRenderableConfiguration()
        {
            if (tileRenderable is null)
                return;
            tileRenderable.AllowHardwareAcceleration = allowHardwareAcceleration;
            tileRenderable.ZIndex = tileId.LevelOfDetail;
            var rect1 = new Rect(tileId.X * tilePyramidDescriptor.TileWidth - 1L, tileId.Y * tilePyramidDescriptor.TileHeight - 1L, tilePyramidDescriptor.TileWidth + 2, tilePyramidDescriptor.TileHeight + 2);
            rect1.Intersect(new Rect(-1.0, -1.0, tilePyramidDescriptor.GetLevelOfDetailWidth(tileId.LevelOfDetail) + 2L, tilePyramidDescriptor.GetLevelOfDetailHeight(tileId.LevelOfDetail) + 2L));
            var tileEdgeFlags = tilePyramidDescriptor.GetTileEdgeFlags(tileId);
            var rect2 = rect1;
            var num1 = overlapPyramidEdges || !tileEdgeFlags.IsLeftEdge ? 1.0 - tileOverlap : 1.0;
            rect2.X += num1;
            rect2.Width -= num1;
            var num2 = overlapPyramidEdges || !tileEdgeFlags.IsTopEdge ? 1.0 - tileOverlap : 1.0;
            rect2.Y += num2;
            rect2.Height -= num2;
            rect2.Width -= overlapPyramidEdges || !tileEdgeFlags.IsRightEdge ? 1.0 - tileOverlap : 1.0;
            rect2.Height -= overlapPyramidEdges || !tileEdgeFlags.IsBottomEdge ? 1.0 - tileOverlap : 1.0;
            if (tilePyramidClip.HasValue)
            {
                var finestLodScaleFactor = LodToFinestLodScaleFactor;
                var rect3 = new Rect(tilePyramidClip.Value.X / finestLodScaleFactor.X, tilePyramidClip.Value.Y / finestLodScaleFactor.Y, tilePyramidClip.Value.Width / finestLodScaleFactor.X, tilePyramidClip.Value.Height / finestLodScaleFactor.Y);
                rect2.Intersect(rect3);
            }
            tileRenderable.Clip = new Rect(rect2.X - tileId.X * tilePyramidDescriptor.TileWidth + 1.0, rect2.Y - tileId.Y * tilePyramidDescriptor.TileHeight + 1.0, rect2.Width, rect2.Height);
        }
    }
}
