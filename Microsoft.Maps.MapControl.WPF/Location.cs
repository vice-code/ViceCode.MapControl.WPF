using System;
using System.ComponentModel;
using Microsoft.Maps.MapControl.WPF.Design;

namespace Microsoft.Maps.MapControl.WPF
{
    [TypeConverter(typeof(LocationConverter))]
    public class Location : IFormattable
    {
        public const double MaxLatitude = 90.0;
        public const double MinLatitude = -90.0;
        public const double MaxLongitude = 180.0;
        public const double MinLongitude = -180.0;

        public Location()
          : this(0.0, 0.0, 0.0, AltitudeReference.Ground)
        {
        }

        public Location(double latitude, double longitude)
          : this(latitude, longitude, 0.0, AltitudeReference.Ground)
        {
        }

        public Location(double latitude, double longitude, double altitude)
          : this(latitude, longitude, altitude, AltitudeReference.Ground)
        {
        }

        public Location(
          double latitude,
          double longitude,
          double altitude,
          AltitudeReference altitudeReference)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            AltitudeReference = altitudeReference;
        }

        public Location(Location location)
        {
            Latitude = location.Latitude;
            Longitude = location.Longitude;
            Altitude = location.Altitude;
            AltitudeReference = location.AltitudeReference;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Altitude { get; set; }

        public AltitudeReference AltitudeReference { get; set; }

        public static double NormalizeLongitude(double longitude)
        {
            if (longitude < -180.0 || longitude > 180.0)
                return longitude - Math.Floor((longitude + 180.0) / 360.0) * 360.0;
            return longitude;
        }

        public static bool operator ==(Location location1, Location location2)
        {
            if (ReferenceEquals(location1, location2))
                return true;
            if (location1 is null || location2 is null || (!IsEqual(location1.Latitude, location2.Latitude) || !IsEqual(location1.Longitude, location2.Longitude)) || !IsEqual(location1.Altitude, location2.Altitude))
                return false;
            return location1.AltitudeReference == location2.AltitudeReference;
        }

        public static bool operator !=(Location location1, Location location2) => !(location1 == location2);

        public override bool Equals(object obj)
        {
            if (obj is null || obj as Location is null)
                return false;
            return this == (Location)obj;
        }

        public override int GetHashCode() => Latitude.GetHashCode() ^ Longitude.GetHashCode() ^ Altitude.GetHashCode() ^ AltitudeReference.GetHashCode();

        public override string ToString() => ((IFormattable)this).ToString(null, null);

        public string ToString(IFormatProvider provider) => ((IFormattable)this).ToString(null, provider);

        string IFormattable.ToString(string format, IFormatProvider provider) => string.Format(provider, "{0:" + format + "},{1:" + format + "},{2:" + format + "}", Latitude, Longitude, Altitude);

        private static bool IsEqual(double value1, double value2) => Math.Abs(value1 - value2) <= double.Epsilon;
    }
}
