using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Maps.MapExtras
{
    internal struct TileId : IComparable<TileId>, IEquatable<TileId>
    {
        public TileId(int levelOfDetail, long x, long y)
        {
            LevelOfDetail = levelOfDetail;
            X = x;
            Y = y;
        }

        public int LevelOfDetail { get; }

        public long X { get; }

        public long Y { get; }

        public TileId GetParent()
        {
            if (!HasParent)
                throw new InvalidOperationException("Level 0 tile has no parent");
            return new TileId(LevelOfDetail - 1, X >> 1, Y >> 1);
        }

        public bool HasParent => LevelOfDetail > 0;

        public static TileId operator -(TileId left, TileId right)
        {
            if (left.LevelOfDetail > right.LevelOfDetail)
                throw new InvalidOperationException("Cannot subtract these tiles. Left must be higher level of detail than right.");
            var levelOfDetail = right.LevelOfDetail - left.LevelOfDetail;
            return new TileId(levelOfDetail, right.X - (left.X << levelOfDetail), right.Y - (left.Y << levelOfDetail));
        }

        public bool IsChildOf(TileId other)
        {
            if (LevelOfDetail < other.LevelOfDetail)
                return false;
            var num = LevelOfDetail - other.LevelOfDetail;
            if (X >> num == other.X)
                return Y >> num == other.Y;
            return false;
        }

        public int GetChildIndex(TileId child)
        {
            if (child.GetParent() != this)
                throw new InvalidOperationException("Must be parent of child.");
            return 2 * (int)(child.Y - Y * 2L) + (int)(child.X - X * 2L);
        }

        public IEnumerable<TileId> GetChildren()
        {
            var x = X << 1;
            var y = Y << 1;
            return new TileId[4]
            {
        new TileId(LevelOfDetail + 1, x, y),
        new TileId(LevelOfDetail + 1, x + 1L, y),
        new TileId(LevelOfDetail + 1, x, y + 1L),
        new TileId(LevelOfDetail + 1, x + 1L, y + 1L)
            };
        }

        public string GetRequestCode()
        {
            if (LevelOfDetail < 0)
                return string.Empty;
            var stringBuilder = new StringBuilder(LevelOfDetail)
            {
                Length = LevelOfDetail
            };
            var x = X;
            var y = Y;
            for (var index = 0; index < LevelOfDetail; ++index)
            {
                stringBuilder[LevelOfDetail - 1 - index] = "0123"[(int)((x & 1L) + 2L * (y & 1L))];
                x >>= 1;
                y >>= 1;
            }
            return stringBuilder.ToString();
        }

        public override int GetHashCode() => LevelOfDetail << 26 + (int)X << 13 + (int)Y;

        public override bool Equals(object obj) => obj is TileId tileId && Equals(tileId);

        public static bool operator ==(TileId left, TileId right)
        {
            if (left.LevelOfDetail == right.LevelOfDetail && left.X == right.X)
                return left.Y == right.Y;
            return false;
        }

        public static bool operator !=(TileId left, TileId right) => !(left == right);

        public static bool operator <(TileId left, TileId right) => left.CompareTo(right) < 0;

        public static bool operator >(TileId left, TileId right) => left.CompareTo(right) > 0;

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "LOD: {0} Tile ({1},{2}) {3}", LevelOfDetail, X, Y, GetRequestCode());

        public int CompareTo(TileId other)
        {
            var num = LevelOfDetail.CompareTo(other.LevelOfDetail);
            if (num == 0)
            {
                num = X.CompareTo(other.X);
                if (num == 0)
                    num = Y.CompareTo(other.Y);
            }
            return num;
        }

        public bool Equals(TileId other) => this == other;
    }
}
