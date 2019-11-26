using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Maps.MapControl.WPF.Design;

namespace Microsoft.Maps.MapControl.WPF
{
    [TypeConverter(typeof(LocationRectConverter))]
    public class LocationRect : IFormattable
    {
        private double halfWidth;
        private double halfHeight;

        public LocationRect() => Center = new Location(0.0, 0.0);

        public LocationRect(Location center, double width, double height)
        {
            Center = center;
            halfWidth = width / 2.0;
            halfHeight = height / 2.0;
        }

        public LocationRect(double north, double west, double south, double east)
          : this() => Init(north, west, south, east);

        public LocationRect(Location corner1, Location corner2)
      : this(new Location[2]
      {
        corner1,
        corner2
      })
        {
        }

        public LocationRect(IList<Location> locations)
          : this()
        {
            var num1 = -90.0;
            var num2 = 90.0;
            var num3 = 180.0;
            var num4 = -180.0;
            foreach (var location in locations)
            {
                num1 = Math.Max(num1, location.Latitude);
                num2 = Math.Min(num2, location.Latitude);
                num3 = Math.Min(num3, location.Longitude);
                num4 = Math.Max(num4, location.Longitude);
            }
            Init(num1, num3, num2, num4);
        }

        public LocationRect(LocationRect rect)
        {
            Center = new Location(rect.Center);
            halfHeight = rect.halfHeight;
            halfWidth = rect.halfWidth;
        }

        private void Init(double north, double west, double south, double east)
        {
            if (west > east)
                east += 360.0;
            Center = new Location((south + north) / 2.0, (west + east) / 2.0);
            halfHeight = (north - south) / 2.0;
            halfWidth = Math.Abs(east - west) / 2.0;
        }

        public double North
        {
            get => Center.Latitude + halfHeight;
            set => Init(value, West, South, East);
        }

        public double West
        {
            get
            {
                if (halfWidth != 180.0)
                    return Location.NormalizeLongitude(Center.Longitude - halfWidth);
                return -180.0;
            }
            set => Init(North, value, South, East);
        }

        public double South
        {
            get => Center.Latitude - halfHeight;
            set => Init(North, West, value, East);
        }

        public double East
        {
            get
            {
                if (halfWidth != 180.0)
                    return Location.NormalizeLongitude(Center.Longitude + halfWidth);
                return 180.0;
            }
            set => Init(North, West, South, value);
        }

        public Location Center { get; private set; }

        public double Width => halfWidth * 2.0;

        public double Height => halfHeight * 2.0;

        public Location Northwest => new Location(North, West);

        public Location Northeast
        {
            get => new Location(North, East);
            set
            {
                if (Center is null)
                    Init(value.Latitude, value.Longitude, value.Latitude, value.Longitude);
                else
                    Init(value.Latitude, West, South, value.Longitude);
            }
        }

        public Location Southeast => new Location(South, East);

        public Location Southwest
        {
            get => new Location(South, West);
            set
            {
                if (Center is null)
                    Init(value.Latitude, value.Longitude, value.Latitude, value.Longitude);
                else
                    Init(North, value.Longitude, value.Latitude, East);
            }
        }

        public static bool operator ==(LocationRect rect1, LocationRect rect2)
        {
            if (ReferenceEquals(rect1, rect2))
                return true;
            if (rect1 is null || rect2 is null || (!(rect1.Center == rect2.Center) || rect1.halfWidth != rect2.halfWidth))
                return false;
            return rect1.halfHeight == rect2.halfHeight;
        }

        public static bool operator !=(LocationRect rect1, LocationRect rect2) => !(rect1 == rect2);

        public override int GetHashCode() => Center.GetHashCode() ^ halfWidth.GetHashCode() ^ halfHeight.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is null || obj as LocationRect is null)
                return false;
            return this == (LocationRect)obj;
        }

        public bool Intersects(LocationRect rect)
        {
            var num1 = Math.Abs(Center.Latitude - rect.Center.Latitude);
            var num2 = Math.Abs(Center.Longitude - rect.Center.Longitude);
            if (num2 > 180.0)
                num2 = 360.0 - num2;
            if (num1 <= halfHeight + rect.halfHeight)
                return num2 <= halfWidth + rect.halfWidth;
            return false;
        }

        public LocationRect Intersection(LocationRect rect)
        {
            var locationRect = new LocationRect();
            if (Intersects(rect))
            {
                var val1_1 = Center.Longitude - halfWidth;
                var val2_1 = rect.Center.Longitude - rect.halfWidth;
                var val1_2 = Center.Longitude + halfWidth;
                var val2_2 = rect.Center.Longitude + rect.halfWidth;
                if (Math.Abs(Center.Longitude - rect.Center.Longitude) > 180.0)
                {
                    if (Center.Longitude < rect.Center.Longitude)
                    {
                        val1_1 += 360.0;
                        val1_2 += 360.0;
                    }
                    else
                    {
                        val2_1 += 360.0;
                        val2_2 += 360.0;
                    }
                }
                var num1 = Math.Max(val1_1, val2_1);
                var num2 = Math.Min(val1_2, val2_2);
                var num3 = Math.Min(North, rect.North);
                var num4 = Math.Max(South, rect.South);
                locationRect = new LocationRect(new Location((num3 + num4) / 2.0, Location.NormalizeLongitude((num1 + num2) / 2.0)), num2 - num1, num3 - num4);
            }
            return locationRect;
        }

        public override string ToString() => ((IFormattable)this).ToString(null, null);

        public string ToString(IFormatProvider provider) => ((IFormattable)this).ToString(null, provider);

        string IFormattable.ToString(string format, IFormatProvider provider) => string.Format(provider, "{0:" + format + "} {1:" + format + "}", Northwest, Southeast);
    }
}
