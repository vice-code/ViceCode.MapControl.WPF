using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Maps.MapControl.WPF;

namespace Microsoft.Maps.MapExtras
{
    internal static class MapMath
    {
        public static readonly double EnhancedBirdseyePitch = -180.0 / Math.PI * Math.Asin(45.0 / 64.0);
        public const int DefaultTileSize = 256;
        public const int NorthUpHeading = 0;
        public const int EastUpHeading = 90;
        public const int SouthUpHeading = 180;
        public const int WestUpHeading = 270;

        public static double LocationRectToMercatorZoomLevel(Size viewportSize, LocationRect boundingRectangle)
        {
            var rect = new Rect(MercatorCube.Instance.FromLocation(boundingRectangle.Northwest).ToPoint(), MercatorCube.Instance.FromLocation(boundingRectangle.Southeast).ToPoint());
            return Math.Log(Math.Min(viewportSize.Width / (DefaultTileSize * rect.Width), viewportSize.Height / (DefaultTileSize * rect.Height)), 2.0);
        }

        public static void CalculateViewFromLocations(IEnumerable<Location> locations, Size viewportSize, double heading, Thickness margin, out Point centerNormalizedMercator, out double zoomLevel)
        {
            var point1 = new Point(double.MaxValue, double.MaxValue);
            var point2 = new Point(double.MinValue, double.MinValue);
            var rotateTransform = new RotateTransform(heading)
            {
                CenterX = 0.5,
                CenterY = 0.5
            };
            foreach (var location in locations)
            {
                var point3 = rotateTransform.Transform(MercatorCube.Instance.FromLocation(location).ToPoint());
                point1.X = Math.Min(point1.X, point3.X);
                point1.Y = Math.Min(point1.Y, point3.Y);
                point2.X = Math.Max(point2.X, point3.X);
                point2.Y = Math.Max(point2.Y, point3.Y);
            }
            if (point2.X > point1.X && point2.Y > point1.Y)
            {
                var a = Math.Min((viewportSize.Width - margin.Left - margin.Right) / (DefaultTileSize * (point2.X - point1.X)), (viewportSize.Height - margin.Top - margin.Bottom) / (DefaultTileSize * (point2.Y - point1.Y)));
                zoomLevel = Math.Log(a, 2.0);
                var num = 1.0 / (DefaultTileSize * Math.Pow(2.0, zoomLevel));
                centerNormalizedMercator = rotateTransform.Inverse.Transform(new Point((point2.X + num * margin.Right + point1.X - num * margin.Left) / 2.0, (point2.Y + num * margin.Bottom + point1.Y - num * margin.Top) / 2.0));
            }
            else
            {
                if (point1.X != point2.X)
                    throw new InvalidOperationException("Must provide at least one location.");
                zoomLevel = 1.0;
                centerNormalizedMercator = point1;
            }
        }

        public static Location GetMercatorCenter(LocationRect boundingRectangle)
        {
            var location = MercatorCube.Instance.ToLocation(new Point3D(VectorMath.Multiply(VectorMath.Add(MercatorCube.Instance.FromLocation(boundingRectangle.Northwest).ToPoint(), MercatorCube.Instance.FromLocation(boundingRectangle.Southeast).ToPoint()), 0.5), 0.0));
            return new Location(location.Latitude, location.Longitude, boundingRectangle.Center.Altitude, boundingRectangle.Center.AltitudeReference);
        }

        public static double MercatorZoomLevelToScale(double zoomLevel, double latitude)
        {
            var num = 40075016.6855785 / (Math.Pow(2.0, zoomLevel) * DefaultTileSize);
            return Math.Cos(latitude * (SouthUpHeading / Math.PI)) * num;
        }

        public static double ScaleToMercatorZoomLevel(double scale, double latitude) => Math.Log(40075016.6855785 / (scale / Math.Cos(latitude * (SouthUpHeading / Math.PI)) * DefaultTileSize), 2.0);

        public static Location NormalizeLocation(Location location) => new Location(Math.Min(Math.Max(location.Latitude, -EastUpHeading), EastUpHeading), Location.NormalizeLongitude(location.Longitude), location.Altitude, location.AltitudeReference);

        public static LocationRect NormalizeLocationRect(LocationRect locaitonRect)
        {
            var center = NormalizeLocation(locaitonRect.Center);
            var width = locaitonRect.Width;
            var height = locaitonRect.Height;
            if (width >= 360.0)
            {
                width = 360.0;
                center.Longitude = 0;
            }
            if (height >= SouthUpHeading)
            {
                height = SouthUpHeading;
                center.Latitude = 0;
            }
            return new LocationRect(center, width, height);
        }

        public static int SnapToCardinalHeading(double heading) => ((int)Math.Round(heading / EastUpHeading) % 4 + 4) % 4 * EastUpHeading;

        internal static Rect LocationToViewportPoint(ref Matrix3D normalizedMercatorToViewport, LocationRect boundingRectangle)
        {
            var point3D1 = MercatorCube.Instance.FromLocation(boundingRectangle.Northwest);
            var point3D2 = normalizedMercatorToViewport.Transform(new System.Windows.Media.Media3D.Point3D(point3D1.X, point3D1.Y, NorthUpHeading));
            var point3D3 = MercatorCube.Instance.FromLocation(boundingRectangle.Southeast);
            var point3D4 = normalizedMercatorToViewport.Transform(new System.Windows.Media.Media3D.Point3D(point3D3.X, point3D3.Y, NorthUpHeading));
            return new Rect(new Point(point3D2.X, point3D2.Y), new Point(point3D4.X, point3D4.Y));
        }

        internal static bool LocationToViewportPoint(ref Matrix3D normalizedMercatorToViewport, Location location, out Point viewportPosition)
        {
            var point3D1 = MercatorCube.Instance.FromLocation(location);
            var point3D2 = normalizedMercatorToViewport.Transform(new System.Windows.Media.Media3D.Point3D(point3D1.X, point3D1.Y, NorthUpHeading));
            viewportPosition = new Point(point3D2.X, point3D2.Y);
            return true;
        }

        internal static bool TryLocationToViewportPoint(ref Matrix3D normalizedMercatorToViewport, Location location, out Point viewportPosition)
        {
            var point3D1 = MercatorCube.Instance.FromLocation(location);
            var point3D2 = normalizedMercatorToViewport.Transform(new System.Windows.Media.Media3D.Point3D(point3D1.X, point3D1.Y, NorthUpHeading));
            viewportPosition = new Point(point3D2.X, point3D2.Y);
            return true;
        }
    }
}
