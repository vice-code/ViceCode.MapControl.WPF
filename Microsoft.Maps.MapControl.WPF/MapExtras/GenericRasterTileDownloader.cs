using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Microsoft.Maps.MapExtras
{
    internal class GenericRasterTileDownloader : RasterTileDownloader
    {
        private readonly Dictionary<TileId, TileRequest> tileRequests = new Dictionary<TileId, TileRequest>();
        private readonly Dictionary<BitmapImage, TileRequest> tileRequestsByBitmapImage = new Dictionary<BitmapImage, TileRequest>();
        private const string HeaderMetadataPrefix = "X-VE-";
        private readonly TileUriDelegate tileUriDelegate;
        private readonly TileImageDelegate tileImageDelegate;
        private readonly MapControl.WPF.TileSource tileSource;
        private readonly Dispatcher uiThreadDispatcher;

        public GenericRasterTileDownloader(
          MapControl.WPF.TileSource tileSource,
          OverlapBorderPresence overlapBordersPresence,
          Dispatcher uiThreadDispatcher)
        {
            tileUriDelegate = tileId => VectorMath.TileSourceGetUriWrapper(tileSource, tileId);
            tileImageDelegate = tileSource.DirectImage is null ? (tileId => tileSource.GetImage(tileId.X, tileId.Y, tileId.LevelOfDetail - 8)) : (TileImageDelegate)(tileId => tileSource.DirectImage(tileId.X, tileId.Y, tileId.LevelOfDetail - 8));
            this.tileSource = tileSource;
            OverlapBorderPresence = overlapBordersPresence;
            this.uiThreadDispatcher = uiThreadDispatcher;
        }

        public OverlapBorderPresence OverlapBorderPresence { get; protected set; }

        public Func<TileId, BitmapSource> ProvideMissingTileImage { get; set; }

        public override void DownloadTile(
          TileId tileId,
          TileEdgeFlags tileEdgeFlags,
          object token,
          RasterTileAvailableDelegate tileAvailableDelegate,
          int priority)
        {
            var uri = tileUriDelegate(tileId);
            if (uri is object)
            {
                if (tileRequests.TryGetValue(tileId, out var tileRequest1))
                    throw new InvalidOperationException("Multiple concurrent downloads of the same tile is not supported.");
                TileRequest tileRequest2;
                tileRequests[tileId] = tileRequest2 = new TileRequest()
                {
                    TileId = tileId,
                    Token = token,
                    TileEdgeFlags = tileEdgeFlags,
                    TileAvailableDelegate = tileAvailableDelegate
                };
                tileRequest2.WebRequest = BitmapImageRequestQueue.Instance.CreateRequest(uri, (NetworkPriority)priority, tileRequest2, new BitmapImageRequestCompletedHandler(TileDownloadCompleted));
            }
            else if (tileSource.SuppliesImagesDirectly)
            {
                if (tileRequests.TryGetValue(tileId, out var tileRequest1))
                    throw new InvalidOperationException("Multiple concurrent downloads of the same tile is not supported.");
                TileRequest tileRequest2;
                tileRequests[tileId] = tileRequest2 = new TileRequest()
                {
                    TileId = tileId,
                    Token = token,
                    TileEdgeFlags = tileEdgeFlags,
                    TileAvailableDelegate = tileAvailableDelegate
                };
                tileRequest2.WebRequest = BitmapImageRequestQueue.Instance.CreateRequest(tileImageDelegate, (NetworkPriority)priority, tileRequest2, new BitmapImageRequestCompletedHandler(TileDownloadCompleted));
            }
            else
                tileAvailableDelegate(null, new Rect(), null, token);
        }

        public override void UpdateTileDownloadPriority(TileId tileId, int priority)
        {
            if (!tileRequests.TryGetValue(tileId, out var tileRequest))
                throw new InvalidOperationException("Tile download must be in progress to update its priority.");
            tileRequest.WebRequest.NetworkPriority = (NetworkPriority)priority;
        }

        public override void CancelTileDownload(TileId tileId)
        {
            if (!tileRequests.TryGetValue(tileId, out var tileRequest))
                throw new InvalidOperationException("Tile download must be in progress to be cancelled.");
            tileRequest.Cancelled = true;
            tileRequest.WebRequest.AbortIfInQueue();
            tileRequests.Remove(tileId);
        }

        private void TileDownloadCompleted(object userToken, BitmapImage bitmapImage, Exception error)
        {
            if (bitmapImage is object && error is object)
                throw new ArgumentException("only one of bitmapImage and error may be null");
            var tileRequest = (TileRequest)userToken;
            if (tileRequest.Cancelled)
                return;
            var bitmapSourceResult = (BitmapSource)null;
            var tileSubregionResult = new Rect();
            if (tileRequest.Cancelled)
                return;
            if (error is object)
            {
                if (ProvideMissingTileImage is object)
                {
                    bitmapSourceResult = ProvideMissingTileImage(tileRequest.TileId);
                    tileSubregionResult = new Rect(1.0, 1.0, bitmapSourceResult.PixelWidth - 2, bitmapSourceResult.PixelHeight - 2);
                }
            }
            else if (OverlapBorderPresence == OverlapBorderPresence.OnAllEdges)
            {
                bitmapSourceResult = bitmapImage;
                tileSubregionResult = new Rect(1.0, 1.0, bitmapImage.PixelWidth - 2, bitmapImage.PixelHeight - 2);
            }
            else
            {
                try
                {
                    var num1 = (bitmapImage.Format.BitsPerPixel + 7) / 8;
                    var stride1 = bitmapImage.PixelWidth * num1;
                    var numArray1 = new byte[stride1 * bitmapImage.PixelHeight];
                    bitmapImage.CopyPixels(numArray1, stride1, 0);
                    var pixelWidth1 = bitmapImage.PixelWidth;
                    var pixelHeight1 = bitmapImage.PixelHeight;
                    var pixelWidth2 = bitmapImage.PixelWidth;
                    var pixelHeight2 = bitmapImage.PixelHeight;
                    int pixelWidth3;
                    int pixelHeight3;
                    int num2;
                    int num3;
                    if (OverlapBorderPresence == OverlapBorderPresence.None)
                    {
                        pixelWidth3 = pixelWidth2 + 2;
                        pixelHeight3 = pixelHeight2 + 2;
                        num2 = 1;
                        num3 = 1;
                    }
                    else
                    {
                        pixelWidth3 = pixelWidth2 + ((tileRequest.TileEdgeFlags.IsLeftEdge ? 1 : 0) + (tileRequest.TileEdgeFlags.IsRightEdge ? 1 : 0));
                        pixelHeight3 = pixelHeight2 + ((tileRequest.TileEdgeFlags.IsTopEdge ? 1 : 0) + (tileRequest.TileEdgeFlags.IsBottomEdge ? 1 : 0));
                        num2 = tileRequest.TileEdgeFlags.IsLeftEdge ? 1 : 0;
                        num3 = tileRequest.TileEdgeFlags.IsTopEdge ? 1 : 0;
                    }
                    var num4 = num1;
                    var stride2 = pixelWidth3 * num4;
                    var numArray2 = new byte[pixelHeight3 * stride2];
                    for (var index1 = 0; index1 < pixelHeight3; ++index1)
                    {
                        for (var index2 = 0; index2 < pixelWidth3; ++index2)
                        {
                            var num5 = Math.Max(0, Math.Min(pixelHeight1 - 1, index1 - num3));
                            var num6 = Math.Max(0, Math.Min(pixelWidth1 - 1, index2 - num2));
                            var num7 = num5 * stride1 + num6 * num1;
                            var num8 = index1 * stride2 + index2 * num4;
                            for (var index3 = 0; index3 < num1; ++index3)
                                numArray2[num8++] = numArray1[num7++];
                        }
                    }
                    bitmapSourceResult = BitmapSource.Create(pixelWidth3, pixelHeight3, 96.0, 96.0, bitmapImage.Format, bitmapImage.Palette, numArray2, stride2);
                    bitmapSourceResult.Freeze();
                    tileSubregionResult = new Rect(1.0, 1.0, pixelWidth3 - 2, pixelHeight3 - 2);
                }
                catch (Exception)
                {
                }
            }
            uiThreadDispatcher.BeginInvoke(new Action(() =>
           {
               if (tileRequest.Cancelled)
                   return;
               tileRequest.TileAvailableDelegate(bitmapSourceResult, tileSubregionResult, null, tileRequest.Token);
               tileRequests.Remove(tileRequest.TileId);
           }));
        }

        internal class TileRequest
        {
            public TileId TileId { get; set; }

            public TileEdgeFlags TileEdgeFlags { get; set; }

            public object Token { get; set; }

            public RasterTileAvailableDelegate TileAvailableDelegate { get; set; }

            public bool Cancelled { get; set; }

            public BitmapImageRequest WebRequest { get; set; }
        }
    }
}
