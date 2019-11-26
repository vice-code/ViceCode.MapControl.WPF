using System;
using System.Globalization;

namespace Microsoft.Maps.MapExtras
{
    internal struct Pixel : IEquatable<Pixel>
    {
        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public static bool operator ==(Pixel pixel1, Pixel pixel2) => pixel1.X == pixel2.X ? pixel1.Y == pixel2.Y : false;

        public static bool operator !=(Pixel pixel1, Pixel pixel2) => !(pixel1 == pixel2);

        public override bool Equals(object obj) => obj is Pixel pixel && Equals(pixel);

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0},{1}", X, Y);

        public bool Equals(Pixel other) => this == other;
    }
}
