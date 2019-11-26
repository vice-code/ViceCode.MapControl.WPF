using System;
using Microsoft.Maps.MapControl.WPF;

namespace Microsoft.Maps.MapExtras
{
    internal sealed class MercatorCube : LocationConverter
    {
        public const double MercatorLatitudeLimit = 85.051128;
        public const double OneMeterAsVectorDistanceAtEquator = 2.49532023366534E-08;

        private MercatorCube()
        {
        }

        public static MercatorCube Instance { get; } = new MercatorCube();

        public override Location ToLocation(Point3D vector)
        {
            var latitude = YToLatitude(vector.Y);
            return new Location(latitude, (vector.X - 0.5) * 360.0, VectorDistanceToMetersAtLatitude(latitude, vector.Z), AltitudeReference.Ellipsoid);
        }

        public override Location ToLocation(Point3D vector, Point3D wrappingCenter)
        {
            vector.X = WrapX(vector.X, wrappingCenter.X);
            return ToLocation(vector);
        }

        public override Point3D FromLocation(Location location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));
            return new Point3D(location.Longitude / 360.0 + 0.5, LatitudeToY(location.Latitude), MetersToVectorDistanceAtLatitude(location.Latitude, location.Altitude));
        }

        public override Point3D FromLocation(Location location, Point3D wrappingCenter)
        {
            var point3D = FromLocation(location);
            point3D.X = WrapX(point3D.X, wrappingCenter.X);
            return point3D;
        }

        public override Point3D GetUpVector(Point3D vector) => new Point3D(0.0, 0.0, 1.0);

        public override Point3D GetNorthVector(Point3D vector) => new Point3D(0.0, -1.0, 0.0);

        public override Point3D ChangeAltitude(Point3D vector, double altitude) => new Point3D(vector.X, vector.Y, MetersToVectorDistance(vector, altitude));

        public override double VectorDistanceToMeters(Point3D vector, double distance) => VectorDistanceToMetersAtLatitude(YToLatitude(vector.Y), distance);

        public override double MetersToVectorDistance(Point3D vector, double meters) => MetersToVectorDistanceAtLatitude(YToLatitude(vector.Y), meters);

        public override CoordinateSystemDirection Direction => CoordinateSystemDirection.LeftHanded;

        public double VectorDistanceToMetersAtLatitude(double latitude, double distance)
        {
            if (Math.Abs(distance) <= double.Epsilon)
                return 0.0;
            return distance * Math.Cos(latitude * (Math.PI / 180.0)) / OneMeterAsVectorDistanceAtEquator;
        }

        public double MetersToVectorDistanceAtLatitude(double latitude, double meters)
        {
            if (Math.Abs(meters) <= double.Epsilon)
                return 0.0;
            return meters * OneMeterAsVectorDistanceAtEquator / Math.Cos(latitude * (Math.PI / 180.0));
        }

        public double YToLatitude(double y) => 90.0 - 2.0 * Math.Atan(Math.Exp((y * 2.0 - 1.0) * Math.PI)) * (180.0 / Math.PI);

        public double LatitudeToY(double latitude)
        {
            if (latitude >= MercatorLatitudeLimit)
                return 0.0;
            if (latitude <= -MercatorLatitudeLimit)
                return 1.0;
            var num = Math.Sin(latitude * (Math.PI / 180.0));
            return 0.5 - Math.Log((1.0 + num) / (1.0 - num)) / (4.0 * Math.PI);
        }

        public static double WrapX(double x, double wrappingCenterX)
        {
            if (x < wrappingCenterX - 0.5 || x > wrappingCenterX + 0.5)
                return x - Math.Floor(x - wrappingCenterX + 0.5);
            return x;
        }
    }
}
