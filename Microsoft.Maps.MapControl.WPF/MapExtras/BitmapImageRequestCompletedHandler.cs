using System;
using System.Windows.Media.Imaging;

namespace Microsoft.Maps.MapExtras
{
    internal delegate void BitmapImageRequestCompletedHandler(object userToken, BitmapImage result, Exception error);
}
