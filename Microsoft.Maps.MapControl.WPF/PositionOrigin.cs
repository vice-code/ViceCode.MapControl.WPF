using System.ComponentModel;
using Microsoft.Maps.MapControl.WPF.Design;

namespace Microsoft.Maps.MapControl.WPF
{
    [TypeConverter(typeof(PositionOriginConverter))]
    public struct PositionOrigin
    {
        public static readonly PositionOrigin TopLeft = new PositionOrigin(0.0, 0.0);
        public static readonly PositionOrigin TopCenter = new PositionOrigin(0.5, 0.0);
        public static readonly PositionOrigin TopRight = new PositionOrigin(1.0, 0.0);
        public static readonly PositionOrigin CenterLeft = new PositionOrigin(0.0, 0.5);
        public static readonly PositionOrigin Center = new PositionOrigin(0.5, 0.5);
        public static readonly PositionOrigin CenterRight = new PositionOrigin(1.0, 0.5);
        public static readonly PositionOrigin BottomLeft = new PositionOrigin(0.0, 1.0);
        public static readonly PositionOrigin BottomCenter = new PositionOrigin(0.5, 1.0);
        public static readonly PositionOrigin BottomRight = new PositionOrigin(1.0, 1.0);

        public PositionOrigin(double horizontalOrigin, double verticalOrigin)
        {
            X = horizontalOrigin;
            Y = verticalOrigin;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is PositionOrigin)
                return Equals((PositionOrigin)obj);
            return false;
        }

        public bool Equals(PositionOrigin origin)
        {
            if (X == origin.X)
                return Y == origin.Y;
            return false;
        }

        public static bool operator ==(PositionOrigin origin1, PositionOrigin origin2) => origin1.Equals(origin2);

        public static bool operator !=(PositionOrigin origin1, PositionOrigin origin2) => !origin1.Equals(origin2);
    }
}
