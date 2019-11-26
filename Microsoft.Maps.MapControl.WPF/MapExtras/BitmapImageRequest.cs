using System;

namespace Microsoft.Maps.MapExtras
{
    internal class BitmapImageRequest
    {
        public BitmapImageRequest(Uri uri, object userToken, BitmapImageRequestCompletedHandler callback)
        {
            Uri = uri;
            UserToken = userToken;
            Callback = callback;
        }

        public BitmapImageRequest(TileImageDelegate getImage, object userToken, BitmapImageRequestCompletedHandler callback)
        {
            GetImage = getImage;
            UserToken = userToken;
            Callback = callback;
        }

        public Uri Uri { get; }

        public TileImageDelegate GetImage { get; }

        public object UserToken { get; }

        public BitmapImageRequestCompletedHandler Callback { get; }

        public NetworkPriority NetworkPriority { get; set; }

        public NetworkPriority NetworkPrioritySnapshot { get; set; }

        public bool Aborted { get; private set; }

        public void AbortIfInQueue() => Aborted = true;
    }
}
