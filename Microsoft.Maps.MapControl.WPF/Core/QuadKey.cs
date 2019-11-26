namespace Microsoft.Maps.MapControl.WPF.Core
{
    public struct QuadKey
    {
        public QuadKey(int x, int y, int zoomLevel)
        {
            ZoomLevel = zoomLevel;
            X = x;
            Y = y;
        }

        public QuadKey(string quadKey)
        {
            QuadKeyToQuadPixel(quadKey, out var x, out var y, out var zoomLevel);
            ZoomLevel = zoomLevel;
            X = x;
            Y = y;
        }

        public int ZoomLevel { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string Key => QuadPixelToQuadKey(X, Y, ZoomLevel);

        private static string QuadPixelToQuadKey(int x, int y, int zoomLevel)
        {
            var num = (uint)(1 << zoomLevel);
            var str = string.Empty;
            if (x < 0 || x >= num || (y < 0 || y >= num))
                return null;
            for (var index = 1; index <= zoomLevel; ++index)
            {
                switch (2 * (y % 2) + x % 2)
                {
                    case 0:
                        str = "0" + str;
                        break;
                    case 1:
                        str = "1" + str;
                        break;
                    case 2:
                        str = "2" + str;
                        break;
                    case 3:
                        str = "3" + str;
                        break;
                }
                x /= 2;
                y /= 2;
            }
            return str;
        }

        private static void QuadKeyToQuadPixel(
          string quadKey,
          out int x,
          out int y,
          out int zoomLevel)
        {
            x = 0;
            y = 0;
            zoomLevel = 0;
            if (string.IsNullOrEmpty(quadKey))
                return;
            zoomLevel = quadKey.Length;
            for (var index = 0; index < quadKey.Length; ++index)
            {
                switch (quadKey[index])
                {
                    case '0':
                        x *= 2;
                        y *= 2;
                        break;
                    case '1':
                        x = x * 2 + 1;
                        y *= 2;
                        break;
                    case '2':
                        x *= 2;
                        y = y * 2 + 1;
                        break;
                    case '3':
                        x = x * 2 + 1;
                        y = y * 2 + 1;
                        break;
                }
            }
        }

        public static bool operator ==(QuadKey tile1, QuadKey tile2)
        {
            if (tile1.X == tile2.X && tile1.Y == tile2.Y)
                return tile1.ZoomLevel == tile2.ZoomLevel;
            return false;
        }

        public static bool operator !=(QuadKey tile1, QuadKey tile2) => !(tile1 == tile2);

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is QuadKey))
                return false;
            return this == (QuadKey)obj;
        }

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ ZoomLevel.GetHashCode();
    }
}
