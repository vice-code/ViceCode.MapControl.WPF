namespace Microsoft.Maps.MapExtras
{
    internal struct Point4D
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Point4D(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static bool operator ==(Point4D point0, Point4D point1)
        {
            if (point0.X == point1.X && point0.Y == point1.Y && point0.Z == point1.Z)
                return point0.W == point1.W;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (GetType() == obj.GetType())
                return this == (Point4D)obj;
            return false;
        }

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();

        public static bool operator !=(Point4D point0, Point4D point1) => !(point0 == point1);

        public static Point4D operator +(Point4D point0, Point4D point1) => new Point4D(point0.X + point1.X, point0.Y + point1.Y, point0.Z + point1.Z, point0.W + point1.W);

        public static Point4D operator -(Point4D point0, Point4D point1) => new Point4D(point0.X - point1.X, point0.Y - point1.Y, point0.Z - point1.Z, point0.W - point1.W);

        public static Point4D operator *(double scalar, Point4D point) => new Point4D(point.X * scalar, point.Y * scalar, point.Z * scalar, point.W * scalar);

        public static Point4D Lerp(Point4D point0, Point4D point1, double alpha) => point0 + alpha * (point1 - point0);
    }
}
