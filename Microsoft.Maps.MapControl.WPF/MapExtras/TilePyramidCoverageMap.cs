using System.Collections.Generic;

namespace Microsoft.Maps.MapExtras
{
    internal class TilePyramidCoverageMap
    {
        private readonly List<List<bool?>> occluderFlags = new List<List<bool?>>();
        private readonly List<List<bool>> occludedFlags = new List<List<bool>>();
        private long x0;
        private long y0;
        private long x1;
        private long y1;
        private int levelOfDetail;
        private readonly int minimumLevelOfDetail;

        public TilePyramidCoverageMap(int minimumLevelOfDetail, int maximumLevelOfDetail)
        {
            this.minimumLevelOfDetail = minimumLevelOfDetail;
            for (var index = 0; index <= maximumLevelOfDetail; ++index)
            {
                occluderFlags.Add(new List<bool?>());
                occludedFlags.Add(new List<bool>());
            }
        }

        public void Intialize(int levelOfDetail, long x0, long y0, long x1, long y1)
        {
            this.levelOfDetail = levelOfDetail;
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;
            for (var lod = levelOfDetail; lod >= minimumLevelOfDetail; --lod)
            {
                GetTileBoundsAtLod(lod, out var lodX0, out var lodY0, out var lodX1, out var lodY1);
                var occluderFlag = occluderFlags[lod];
                var occludedFlag = occludedFlags[lod];
                occluderFlag.Clear();
                occludedFlag.Clear();
                for (var index1 = lodY0; index1 < lodY1; ++index1)
                {
                    for (var index2 = lodX0; index2 < lodX1; ++index2)
                    {
                        occluderFlag.Add(new bool?());
                        occludedFlag.Add(false);
                    }
                }
            }
        }

        public void MarkAsOccluder(TileId tileId, bool occluder) => SetOccluderFlag(tileId, new bool?(occluder));

        public void CalculateOcclusions()
        {
            for (var levelOfDetail = this.levelOfDetail; levelOfDetail >= minimumLevelOfDetail; --levelOfDetail)
            {
                if (levelOfDetail != this.levelOfDetail)
                {
                    GetTileBoundsAtLod(levelOfDetail, out var lodX0, out var lodY0, out var lodX1, out var lodY1);
                    for (var y = lodY0; y < lodY1; ++y)
                    {
                        for (var x = lodX0; x < lodX1; ++x)
                        {
                            var tileId = new TileId(levelOfDetail, x, y);
                            if (GetOccluderFlag(tileId).HasValue && (IsChildIrrelevantOrOccluder(tileId, 0) && IsChildIrrelevantOrOccluder(tileId, 1) && IsChildIrrelevantOrOccluder(tileId, 2) && IsChildIrrelevantOrOccluder(tileId, 3)))
                            {
                                SetOccludedFlag(tileId, true);
                                SetOccluderFlag(tileId, new bool?(true));
                            }
                        }
                    }
                }
            }
        }

        public bool IsOccludedByDescendents(TileId tileId) => GetOccludedFlag(tileId);

        private bool IsChildIrrelevantOrOccluder(TileId tileId, int childIdx)
        {
            var tileId1 = new TileId(tileId.LevelOfDetail + 1, (tileId.X << 1) + childIdx % 2, (tileId.Y << 1) + childIdx / 2);
            GetTileBoundsAtLod(tileId1.LevelOfDetail, out var lodX0, out var lodY0, out var lodX1, out var lodY1);
            if (tileId1.X < lodX0 || tileId1.X >= lodX1 || (tileId1.Y < lodY0 || tileId1.Y >= lodY1))
                return true;
            var occluderFlag = GetOccluderFlag(tileId1);
            if (occluderFlag.HasValue)
                return occluderFlag.Value;
            return true;
        }

        private bool? GetOccluderFlag(TileId tileId) => occluderFlags[tileId.LevelOfDetail][GetIndexInLodArray(tileId)];

        private void SetOccluderFlag(TileId tileId, bool? occluderFlag) => occluderFlags[tileId.LevelOfDetail][GetIndexInLodArray(tileId)] = occluderFlag;

        private bool GetOccludedFlag(TileId tileId) => occludedFlags[tileId.LevelOfDetail][GetIndexInLodArray(tileId)];

        private void SetOccludedFlag(TileId tileId, bool occludedFlag) => occludedFlags[tileId.LevelOfDetail][GetIndexInLodArray(tileId)] = occludedFlag;

        private int GetIndexInLodArray(TileId tileId)
        {
            GetTileBoundsAtLod(tileId.LevelOfDetail, out var lodX0, out var lodY0, out var lodX1, out var lodY1);
            return (int)((tileId.Y - lodY0) * (lodX1 - lodX0) + (tileId.X - lodX0));
        }

        private void GetTileBoundsAtLod(
          int lod,
          out long lodX0,
          out long lodY0,
          out long lodX1,
          out long lodY1)
        {
            var power = levelOfDetail - lod;
            lodX0 = x0 >> power;
            lodY0 = y0 >> power;
            lodX1 = VectorMath.DivPow2RoundUp(x1, power);
            lodY1 = VectorMath.DivPow2RoundUp(y1, power);
        }
    }
}
