using System;

namespace Microsoft.Maps.MapExtras
{
    internal struct Point2D
    {
        public double X;
        public double Y;

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Point2D operator +(Point2D point0, Point2D point1) => new Point2D(point0.X + point1.X, point0.Y + point1.Y);

        public static Point2D operator -(Point2D point0, Point2D point1) => new Point2D(point0.X - point1.X, point0.Y - point1.Y);

        public static Point2D operator *(double scalar, Point2D point) => new Point2D(point.X * scalar, point.Y * scalar);

        public static double Dot(Point2D left, Point2D right) => left.X * right.X + left.Y * right.Y;

        public static Point2D Normalize(Point2D point)
        {
            var num1 = point.Length();
            if (num1 == 0.0)
                return point;
            var num2 = 1.0 / num1;
            return new Point2D(point.X * num2, point.Y * num2);
        }

        public double Length() => Math.Sqrt(X * X + Y * Y);

        public double LengthSquared() => X * X + Y * Y;

        public static double DistanceSquared(Point2D pointA, Point2D pointB)
        {
            var num1 = pointA.X - pointB.X;
            var num2 = pointA.Y - pointB.Y;
            return num1 * num1 + num2 * num2;
        }

        public static double Cross(Point2D left, Point2D right) => left.X * right.Y - left.Y * right.X;

        public static Point2D Lerp(Point2D point0, Point2D point1, double alpha) => point0 + alpha * (point1 - point0);

        public static bool operator ==(Point2D point0, Point2D point1)
        {
            if (point0.X == point1.X)
                return point0.Y == point1.Y;
            return false;
        }

        public static bool operator !=(Point2D point0, Point2D point1) => !(point0 == point1);

        public override bool Equals(object obj)
        {
            if (GetType() == obj.GetType())
                return this == (Point2D)obj;
            return false;
        }

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
    }
}
