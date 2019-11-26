using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapExtras
{
    internal class TilePyramidDescriptor
    {
        private readonly int coarsestLod;
        private readonly int tileHeight;
        private Point2D[] vertices;
        private double polygonCrossProductZ;
        private Point4D[] verticesSS;
        private Point2D[] verticesSSTexCoords;
        private Point4D[] clippedVerticesSS;
        private Point2D[] clippedVerticesSSTexCoords;
        private Point4D[] tempVertexBuffer;
        private Point2D[] tempTextureCoordBuffer;

        public TilePyramidDescriptor(
          long finestLevelOfDetailWidth,
          long finestLevelOfDetailHeight,
          int coarsestLevelOfDetail,
          int tileWidth,
          int tileHeight)
        {
            if (finestLevelOfDetailHeight < 1L)
                throw new ArgumentOutOfRangeException(nameof(finestLevelOfDetailHeight));
            if (finestLevelOfDetailWidth < 1L)
                throw new ArgumentOutOfRangeException(nameof(finestLevelOfDetailWidth));
            if (tileWidth < 1)
                throw new ArgumentOutOfRangeException(nameof(tileWidth));
            if (tileHeight < 1)
                throw new ArgumentOutOfRangeException(nameof(tileHeight));
            FinestLevelWidth = finestLevelOfDetailWidth;
            FinestLevelHeight = finestLevelOfDetailHeight;
            FinestLevelOfDetail = VectorMath.CeilLog2(Math.Max(FinestLevelWidth, FinestLevelHeight));
            if (coarsestLevelOfDetail < 0 || coarsestLevelOfDetail > FinestLevelOfDetail)
                throw new InvalidOperationException("coarsest level of detail must be positive and <= the finest level of detail");
            coarsestLod = coarsestLevelOfDetail;
            TileWidth = tileWidth;
            this.tileHeight = tileHeight;
            ClipPoly = DefaultClipPoly;
        }

        public Point2D[] DefaultClipPoly
        {
            get
            {
                return new Point2D[4]
                {
          new Point2D(0.0, 0.0),
          new Point2D(0.0,  FinestLevelHeight),
          new Point2D( FinestLevelWidth,  FinestLevelHeight),
          new Point2D( FinestLevelWidth, 0.0)
                };
            }
        }

        public Point2D[] ClipPoly
        {
            get => (Point2D[])vertices.Clone();
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length < 3)
                    throw new ArgumentException("value must contain at least 3 elements");
                vertices = value;
                polygonCrossProductZ = (vertices[2].X - vertices[1].X) * (vertices[0].Y - vertices[1].Y) - (vertices[2].Y - vertices[1].Y) * (vertices[0].X - vertices[1].X);
                verticesSS = new Point4D[vertices.Length];
                verticesSSTexCoords = new Point2D[vertices.Length];
                clippedVerticesSS = new Point4D[vertices.Length + 6];
                clippedVerticesSSTexCoords = new Point2D[vertices.Length + 6];
                tempVertexBuffer = new Point4D[vertices.Length + 6];
                tempTextureCoordBuffer = new Point2D[vertices.Length + 6];
            }
        }

        public int TileWidth { get; }

        public int TileHeight => tileHeight;

        public int FinestLevelOfDetail { get; }

        public long FinestLevelWidth { get; }

        public long FinestLevelHeight { get; }

        public double LevelOfDetailBias { get; set; }

        public long GetLevelOfDetailWidth(int lod) => VectorMath.DivPow2RoundUp(FinestLevelWidth, FinestLevelOfDetail - lod);

        public long GetLevelOfDetailHeight(int lod) => VectorMath.DivPow2RoundUp(FinestLevelHeight, FinestLevelOfDetail - lod);

        public long GetLevelOfDetailWidthInTiles(int lod) => VectorMath.DivRoundUp(VectorMath.DivPow2RoundUp(FinestLevelWidth, FinestLevelOfDetail - lod), (uint)TileWidth);

        public long GetLevelOfDetailHeightInTiles(int lod) => VectorMath.DivRoundUp(VectorMath.DivPow2RoundUp(FinestLevelHeight, FinestLevelOfDetail - lod), (uint)tileHeight);

        public TileEdgeFlags GetTileEdgeFlags(TileId tileId) => new TileEdgeFlags(tileId.X == 0L, tileId.X == GetLevelOfDetailWidthInTiles(tileId.LevelOfDetail) - 1L, tileId.Y == 0L, tileId.Y == GetLevelOfDetailHeightInTiles(tileId.LevelOfDetail) - 1L);

        public void GetVisibleTiles(
      Point2D viewportSize,
      ref Matrix3D modelToViewportTransform,
      bool showBackFace,
      IList<TileId> tiles,
      Point2D[] visiblePolyAtFinestLod,
      Point2D[] screenSpacePoly,
      out int screenSpacePolyVertexCount,
      out double preciseRenderLod,
      out int renderLod,
      out double finestLodNeeded)
        {
            if (screenSpacePoly.Length < vertices.Length + 6 || visiblePolyAtFinestLod.Length < vertices.Length + 6)
                throw new ArgumentException("Screen space poly and visible poly at finest LOD must have room for the number of vertices in the clip poly plus 6.");
            tiles.Clear();
            preciseRenderLod = double.MinValue;
            renderLod = int.MinValue;
            finestLodNeeded = double.MinValue;
            for (var index = 0; index < vertices.Length; ++index)
            {
                verticesSS[index] = VectorMath.Transform(modelToViewportTransform, new Point4D(vertices[index].X, vertices[index].Y, 0.0, 1.0));
                verticesSSTexCoords[index].X = vertices[index].X;
                verticesSSTexCoords[index].Y = vertices[index].Y;
            }
            VectorMath.ClipConvexPolygon(new RectangularSolid(0.0, 0.0, 0.0, viewportSize.X, viewportSize.Y, 1.0), verticesSS, verticesSSTexCoords, vertices.Length, clippedVerticesSS, clippedVerticesSSTexCoords, out screenSpacePolyVertexCount, tempVertexBuffer, tempTextureCoordBuffer);
            if (screenSpacePolyVertexCount <= 0)
                return;
            for (var index = 0; index < screenSpacePolyVertexCount; ++index)
            {
                screenSpacePoly[index].X = (clippedVerticesSS[index].X /= clippedVerticesSS[index].W);
                screenSpacePoly[index].Y = (clippedVerticesSS[index].Y /= clippedVerticesSS[index].W);
                visiblePolyAtFinestLod[index].X = clippedVerticesSSTexCoords[index].X;
                visiblePolyAtFinestLod[index].Y = clippedVerticesSSTexCoords[index].Y;
            }
            if (!showBackFace)
            {
                var num = (clippedVerticesSS[2].X - clippedVerticesSS[1].X) * (clippedVerticesSS[0].Y - clippedVerticesSS[1].Y) - (clippedVerticesSS[2].Y - clippedVerticesSS[1].Y) * (clippedVerticesSS[0].X - clippedVerticesSS[1].X);
                if (num < 0.0 && polygonCrossProductZ >= 0.0 || num >= 0.0 && polygonCrossProductZ < 0.0)
                    return;
            }
            CalculateRenderLod(screenSpacePolyVertexCount, out preciseRenderLod, out finestLodNeeded);
            renderLod = CalculateFinestLevelOfDetailToUse(preciseRenderLod);
            var detailWidthInTiles = GetLevelOfDetailWidthInTiles(renderLod);
            var detailHeightInTiles = GetLevelOfDetailHeightInTiles(renderLod);
            if (detailWidthInTiles == 1L && detailHeightInTiles == 1L)
                tiles.Add(new TileId(renderLod, 0L, 0L));
            else
                IntersectClippedPolyWithTileGrid(tiles, clippedVerticesSSTexCoords, screenSpacePolyVertexCount, FinestLevelOfDetail, renderLod, detailWidthInTiles, detailHeightInTiles, TileWidth, tileHeight);
        }

        private static void IntersectClippedPolyWithTileGrid(
          IList<TileId> tiles,
          Point2D[] clippedVerticesSSTexCoords,
          int clippedVerticesSSCount,
          int finestLod,
          int lod,
          long tileGridWidth,
          long tileGridHeight,
          double tileWidth,
          double tileHeight)
        {
            var val1_1 = double.MaxValue;
            var val1_2 = double.MinValue;
            var val1_3 = double.MaxValue;
            var val1_4 = double.MinValue;
            var num1 = 1.0 / (uint)(1 << finestLod - lod) / tileWidth;
            var num2 = 1.0 / (uint)(1 << finestLod - lod) / tileHeight;
            for (var index = 0; index < clippedVerticesSSCount; ++index)
            {
                clippedVerticesSSTexCoords[index].X *= num1;
                clippedVerticesSSTexCoords[index].Y *= num2;
                val1_1 = Math.Min(val1_1, clippedVerticesSSTexCoords[index].X);
                val1_2 = Math.Max(val1_2, clippedVerticesSSTexCoords[index].X);
                val1_3 = Math.Min(val1_3, clippedVerticesSSTexCoords[index].Y);
                val1_4 = Math.Max(val1_4, clippedVerticesSSTexCoords[index].Y);
            }
            var num3 = 0.01 / Math.Min(tileWidth, tileHeight);
            var num4 = VectorMath.Clamp((long)Math.Floor(val1_1 - num3), 0L, tileGridWidth);
            var num5 = VectorMath.Clamp((long)Math.Ceiling(val1_2 + num3), 0L, tileGridWidth);
            var num6 = VectorMath.Clamp((long)Math.Floor(val1_3 - num3), 0L, tileGridHeight);
            var num7 = VectorMath.Clamp((long)Math.Ceiling(val1_4 + num3), 0L, tileGridHeight);
            var length1 = num5 - num4;
            var length2 = num7 - num6;
            if (length1 <= 2L && length2 <= 2L)
            {
                for (var y = num6; y < num7; ++y)
                {
                    for (var x = num4; x < num5; ++x)
                        tiles.Add(new TileId(lod, x, y));
                }
            }
            else
            {
                var flagArray = new bool[length2, length1];
                var num8 = 2.0 * (0.5 + num3) * (0.5 + num3);
                var index1 = clippedVerticesSSCount - 1;
                for (var index2 = 0; index2 < clippedVerticesSSCount; ++index2)
                {
                    var verticesSsTexCoord1 = clippedVerticesSSTexCoords[index1];
                    var verticesSsTexCoord2 = clippedVerticesSSTexCoords[index2];
                    var num9 = (long)Math.Floor(verticesSsTexCoord1.X - num3);
                    var num10 = (long)Math.Ceiling(verticesSsTexCoord1.X + num3);
                    var num11 = (long)Math.Floor(verticesSsTexCoord1.Y - num3);
                    var num12 = (long)Math.Ceiling(verticesSsTexCoord1.Y + num3);
                    for (var index3 = num11; index3 < num12; ++index3)
                    {
                        for (var index4 = num9; index4 < num10; ++index4)
                        {
                            if (index4 >= 0L && index4 < tileGridWidth && (index3 >= 0L && index3 < tileGridHeight))
                                flagArray[index3 - num6, index4 - num4] = true;
                        }
                    }
                    var num13 = VectorMath.Clamp((long)Math.Floor(Math.Min(verticesSsTexCoord1.X, verticesSsTexCoord2.X) - num3), 0L, tileGridWidth);
                    var num14 = VectorMath.Clamp((long)Math.Ceiling(Math.Max(verticesSsTexCoord1.X, verticesSsTexCoord2.X) + num3), 0L, tileGridWidth);
                    var num15 = VectorMath.Clamp((long)Math.Floor(Math.Min(verticesSsTexCoord1.Y, verticesSsTexCoord2.Y) - num3), 0L, tileGridHeight);
                    var num16 = VectorMath.Clamp((long)Math.Ceiling(Math.Max(verticesSsTexCoord1.Y, verticesSsTexCoord2.Y) + num3), 0L, tileGridHeight);
                    for (var index3 = num15; index3 < num16; ++index3)
                    {
                        for (var index4 = num13; index4 < num14; ++index4)
                        {
                            if (!flagArray[index3 - num6, index4 - num4])
                            {
                                var point = new Point2D(index4 + 0.5, index3 + 0.5);
                                if (VectorMath.LinePointDistanceSquared(verticesSsTexCoord1, verticesSsTexCoord2, point, out var inLineSegment) <= num8)
                                {
                                    if (inLineSegment)
                                        flagArray[index3 - num6, index4 - num4] = true;
                                    else if (VectorMath.OrientedBoundingBoxIntersectsAxisAlignedBoundingBox(new Point2D(verticesSsTexCoord1.X, verticesSsTexCoord1.Y), new Point2D(verticesSsTexCoord2.X, verticesSsTexCoord2.Y), 2.0 * num3, new Rect(index4, index3, 1.0, 1.0)))
                                        flagArray[index3 - num6, index4 - num4] = true;
                                }
                            }
                        }
                    }
                    index1 = index2;
                }
                for (var index2 = 0; index2 < length2; ++index2)
                {
                    var val1_5 = int.MaxValue;
                    var val1_6 = int.MinValue;
                    for (var val2 = 0; val2 < length1; ++val2)
                    {
                        if (flagArray[index2, val2])
                        {
                            val1_5 = Math.Min(val1_5, val2);
                            val1_6 = Math.Max(val1_6, val2 + 1);
                        }
                    }
                    if (val1_5 < val1_6)
                    {
                        for (var index3 = val1_5; index3 < val1_6; ++index3)
                            flagArray[index2, index3] = true;
                    }
                }
                for (var y = num6; y < num7; ++y)
                {
                    for (var x = num4; x < num5; ++x)
                    {
                        if (flagArray[y - num6, x - num4])
                            tiles.Add(new TileId(lod, x, y));
                    }
                }
            }
        }

        private int CalculateFinestLevelOfDetailToUse(double renderLod)
        {
            renderLod = renderLod - Math.Floor(renderLod) < 0.5849625 ? Math.Floor(renderLod) : Math.Ceiling(renderLod);
            return VectorMath.Clamp((int)renderLod, coarsestLod, FinestLevelOfDetail);
        }

        private void CalculateRenderLod(
          int numClippedVerticesSS,
          out double renderLod,
          out double finestLodNeeded)
        {
            var index1 = numClippedVerticesSS - 1;
            var num1 = 0.0;
            var num2 = double.MaxValue;
            var num3 = 0;
            for (var index2 = 0; index2 < numClippedVerticesSS; ++index2)
            {
                var num4 = clippedVerticesSS[index2].X - clippedVerticesSS[index1].X;
                var num5 = clippedVerticesSS[index2].Y - clippedVerticesSS[index1].Y;
                var num6 = clippedVerticesSSTexCoords[index2].X - clippedVerticesSSTexCoords[index1].X;
                var num7 = clippedVerticesSSTexCoords[index2].Y - clippedVerticesSSTexCoords[index1].Y;
                if (num4 != 0.0 || num5 != 0.0)
                {
                    var val2 = Math.Sqrt((num6 * num6 + num7 * num7) / (num4 * num4 + num5 * num5));
                    num1 += val2;
                    ++num3;
                    num2 = Math.Min(num2, val2);
                }
                index1 = index2;
            }
            var texelToPixelRatio = num1 / num3;
            renderLod = CalculateRenderLodFromTexelToPixelRatio(texelToPixelRatio);
            finestLodNeeded = CalculateRenderLodFromTexelToPixelRatio(num2);
        }

        private double CalculateRenderLodFromTexelToPixelRatio(double texelToPixelRatio) => FinestLevelOfDetail - Math.Log(texelToPixelRatio, 2.0) + LevelOfDetailBias;
    }
}
