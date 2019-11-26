using System;
using System.Windows;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal static class MercatorUtility
    {
        public const double MercatorLatitudeLimit = 85.051128;
        public const double EarthRadiusInMeters = 6378137.0;
        public const double EarthCircumferenceInMeters = 40075016.6855785;

        public static Point LocationToLogicalPoint(Location location)
        {
            double y;
            if (location.Latitude > MercatorLatitudeLimit)
                y = 0.0;
            else if (location.Latitude < -MercatorLatitudeLimit)
            {
                y = 1.0;
            }
            else
            {
                var num = Math.Sin(location.Latitude * Math.PI / 180.0);
                y = 0.5 - Math.Log((1.0 + num) / (1.0 - num)) / (4.0 * Math.PI);
            }
            return new Point((location.Longitude + 180.0) / 360.0, y);
        }

        public static Location LogicalPointToLocation(Point logicalPoint) => new Location(90.0 - 360.0 * Math.Atan(Math.Exp((logicalPoint.Y * 2.0 - 1.0) * Math.PI)) / Math.PI, logicalPoint.X * 360.0 - 180.0);

        public static double ZoomToScale(Size logicalAreaSizeInScreenSpaceAtLevel1, double zoomLevel, Location location)
        {
            var num = EarthCircumferenceInMeters / (Math.Pow(2.0, zoomLevel - 1.0) * logicalAreaSizeInScreenSpaceAtLevel1.Width);
            return Math.Cos(DegreesToRadians(location.Latitude)) * num;
        }

        public static double ScaleToZoom(Size logicalAreaSizeInScreenSpaceAtLevel1, double scale, Location location) =>
            Math.Log(EarthCircumferenceInMeters / (scale / Math.Cos(DegreesToRadians(location.Latitude)) * logicalAreaSizeInScreenSpaceAtLevel1.Width), 2.0) + 1.0;

        public static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;
    }
}
