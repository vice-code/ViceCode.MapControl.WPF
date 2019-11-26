using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Microsoft.Maps.MapExtras
{
    internal delegate void RasterTileAvailableDelegate(BitmapSource image, Rect tileSubregion, Dictionary<string, string> metadata, object token);
}
