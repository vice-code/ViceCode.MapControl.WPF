using System.Globalization;
using System.Windows;

namespace Microsoft.Maps.MapExtras
{
    internal struct Point3D
    {
        public static readonly Point3D Empty = new Point3D(0.0, 0.0, 0.0);
        public double X;
        public double Y;
        public double Z;

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D(Point point, double z)
        {
            X = point.X;
            Y = point.Y;
            Z = z;
        }

        public Point ToPoint() => new Point(X, Y);

        public static bool operator ==(Point3D point1, Point3D point2)
        {
            if (point1.X == point2.X && point1.Y == point2.Y)
                return point1.Z == point2.Z;
            return false;
        }

        public static Point3D operator /(Point3D val, double div) => new Point3D(val.X / div, val.Y / div, val.Z / div);

        public static bool operator !=(Point3D point1, Point3D point2) => !(point1 == point2);

        public bool Equals(Point3D other)
        {
            if (X == other.X && Y == other.Y)
                return Z == other.Z;
            return false;
        }

        public static Point3D Add(ref Point3D a, ref Point3D b) => new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public void Add(Point3D val)
        {
            X += val.X;
            Y += val.Y;
            Z += val.Z;
        }

        public static Point3D Subtract(ref Point3D a, ref Point3D b) => new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Point3D))
                return false;
            return Equals((Point3D)obj);
        }

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", X, Y, Z);
    }
}
