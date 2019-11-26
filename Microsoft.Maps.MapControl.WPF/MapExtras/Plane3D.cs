using System.Globalization;

namespace Microsoft.Maps.MapExtras
{
    internal struct Plane3D
    {
        public Plane3D(double a, double b, double c, double d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Plane3D(Point3D point1, Point3D point2, Point3D point3)
        {
            var point1_1 = VectorMath.Normalize(VectorMath.Cross(VectorMath.Subtract(point3, point2), VectorMath.Subtract(point2, point1)));
            A = point1_1.X;
            B = point1_1.Y;
            C = point1_1.Z;
            D = VectorMath.Dot(point1_1, point1);
        }

        public Plane3D(Point3D point, Point3D normal)
        {
            normal = VectorMath.Normalize(normal);
            A = normal.X;
            B = normal.Y;
            C = normal.Z;
            D = VectorMath.Dot(normal, point);
        }

        public double A { get; }

        public double B { get; }

        public double C { get; }

        public double D { get; }

        public static bool operator ==(Plane3D plane1, Plane3D plane2)
        {
            if (plane1.A == plane2.A && plane1.B == plane2.B && plane1.C == plane2.C)
                return plane1.D == plane2.D;
            return false;
        }

        public static bool operator !=(Plane3D plane1, Plane3D plane2) => !(plane1 == plane2);

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Plane3D))
                return false;
            return this == (Plane3D)obj;
        }

        public override int GetHashCode() => A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode() ^ D.GetHashCode();

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", A, B, C, D);
    }
}
