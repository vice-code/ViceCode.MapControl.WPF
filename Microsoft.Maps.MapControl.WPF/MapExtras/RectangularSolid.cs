using System;

namespace Microsoft.Maps.MapExtras
{
    internal struct RectangularSolid
    {
        public RectangularSolid(double x, double y, double z, double sizeX, double sizeY, double sizeZ)
        {
            X = x;
            Y = y;
            Z = z;
            if (sizeX < 0.0 || sizeY < 0.0 || sizeZ < 0.0)
                throw new ArgumentException("It is not valid to set a size dimension to less than 0 in a Rect3D structure");
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public double X { get; }

        public double Y { get; }

        public double Z { get; }

        public double SizeX { get; }

        public double SizeY { get; }

        public double SizeZ { get; }

        public static bool operator ==(RectangularSolid left, RectangularSolid right)
        {
            if (left.X == right.X && left.Y == right.Y && (left.Z == right.Z && left.SizeX == right.SizeX) && left.SizeY == right.SizeY)
                return left.SizeZ == right.SizeZ;
            return false;
        }

        public static bool operator !=(RectangularSolid left, RectangularSolid right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (GetType() == obj.GetType())
                return this == (RectangularSolid)obj;
            return false;
        }

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ SizeX.GetHashCode() ^ SizeY.GetHashCode() ^ SizeZ.GetHashCode();
    }
}
