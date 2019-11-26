using System;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapExtras
{
    internal class TilePriorityCalculator
    {
        public const int NumFoveationPriorityBuckets = 5;
        private TileId VisibleTileId;
        private Point2D viewportCenter;
        private double bucketRadiusInterval;
        private int lod;
        private double tileWidthAtFinestLod;
        private double tileHeightAtFinestLod;
        private Point4D vTL0;
        private Point4D vR;
        private Point4D vD;

        public void Initialize(
          TilePyramidDescriptor tilePyramid,
          ref Matrix3D renderableToViewportTransform,
          Point2D viewportSize,
          TileId visibleTileId)
        {
            VisibleTileId = visibleTileId;
            viewportCenter = 0.5 * viewportSize;
            bucketRadiusInterval = 0.5 * Math.Sqrt(viewportSize.X * viewportSize.X + viewportSize.Y * viewportSize.Y) / 5.0 - 1.0;
            lod = visibleTileId.LevelOfDetail;
            tileWidthAtFinestLod = tilePyramid.TileWidth * (1 << tilePyramid.FinestLevelOfDetail - lod);
            tileHeightAtFinestLod = tilePyramid.TileHeight * (1 << tilePyramid.FinestLevelOfDetail - lod);
            vTL0 = VectorMath.Transform(renderableToViewportTransform, new Point4D(tileWidthAtFinestLod * visibleTileId.X, tileHeightAtFinestLod * visibleTileId.Y, 0.0, 1.0));
            var point4D1 = VectorMath.Transform(renderableToViewportTransform, new Point4D(tileWidthAtFinestLod * (visibleTileId.X + 1L), tileHeightAtFinestLod * visibleTileId.Y, 0.0, 1.0));
            var point4D2 = VectorMath.Transform(renderableToViewportTransform, new Point4D(tileWidthAtFinestLod * visibleTileId.X, tileHeightAtFinestLod * (visibleTileId.Y + 1L), 0.0, 1.0));
            vR = point4D1 - vTL0;
            vD = point4D2 - vTL0;
        }

        public int GetPriority(TileId tileId)
        {
            var point4D1 = vTL0 + (tileId.X - VisibleTileId.X) * vR + (tileId.Y - VisibleTileId.Y) * vD;
            var point4D2 = vTL0 + (tileId.X + 1L - VisibleTileId.X) * vR + (tileId.Y - VisibleTileId.Y) * vD;
            var point4D3 = vTL0 + (tileId.X + 1L - VisibleTileId.X) * vR + (tileId.Y + 1L - VisibleTileId.Y) * vD;
            var point4D4 = vTL0 + (tileId.X - VisibleTileId.X) * vR + (tileId.Y + 1L - VisibleTileId.Y) * vD;
            var num = double.MaxValue;
            if (point4D1.W > 0.0)
                num = Math.Min(num, Point2D.DistanceSquared(new Point2D(point4D1.X / point4D1.W, point4D1.Y / point4D1.W), viewportCenter));
            if (point4D2.W > 0.0)
                num = Math.Min(num, Point2D.DistanceSquared(new Point2D(point4D2.X / point4D2.W, point4D2.Y / point4D2.W), viewportCenter));
            if (point4D3.W > 0.0)
                num = Math.Min(num, Point2D.DistanceSquared(new Point2D(point4D3.X / point4D3.W, point4D3.Y / point4D3.W), viewportCenter));
            if (point4D4.W > 0.0)
                num = Math.Min(num, Point2D.DistanceSquared(new Point2D(point4D4.X / point4D4.W, point4D4.Y / point4D4.W), viewportCenter));
            return 4 - VectorMath.Clamp((int)Math.Floor(Math.Sqrt(num) / bucketRadiusInterval), 0, 4);
        }
    }
}
