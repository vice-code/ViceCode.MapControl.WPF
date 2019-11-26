using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Windows.Media.Imaging;

namespace Microsoft.Maps.MapExtras
{
    internal class BitmapImageRequestQueue : IDisposable
    {
        private static readonly int MaxSimultaneousRequests = 6;
        private static BitmapImageRequestQueue instance;
        private readonly List<BitmapImageRequest> pendingRequests;
        private readonly Dictionary<BitmapImageRequest, WebClient> executingRequests;
        private readonly Thread downloadThread;
        private readonly ManualResetEvent thereMayBeWorkToDo;

        public static BitmapImageRequestQueue Instance
        {
            get
            {
                if (instance is null)
                    instance = new BitmapImageRequestQueue();
                return instance;
            }
        }

        private BitmapImageRequestQueue()
        {
            pendingRequests = new List<BitmapImageRequest>();
            executingRequests = new Dictionary<BitmapImageRequest, WebClient>();
            thereMayBeWorkToDo = new ManualResetEvent(true);
            downloadThread = new Thread(new ThreadStart(DownloadThreadStart))
            {
                IsBackground = true
            };
            downloadThread.Start();
        }

        public BitmapImageRequest CreateRequest(Uri uri, NetworkPriority networkPriority, object userToken, BitmapImageRequestCompletedHandler callback)
        {
            var bitmapImageRequest = new BitmapImageRequest(uri, userToken, callback)
            {
                NetworkPriority = networkPriority
            };
            lock (pendingRequests)
                pendingRequests.Add(bitmapImageRequest);
            thereMayBeWorkToDo.Set();
            return bitmapImageRequest;
        }

        public BitmapImageRequest CreateRequest(TileImageDelegate getImage, NetworkPriority networkPriority, object userToken, BitmapImageRequestCompletedHandler callback)
        {
            var bitmapImageRequest = new BitmapImageRequest(getImage, userToken, callback)
            {
                NetworkPriority = networkPriority
            };
            lock (pendingRequests)
                pendingRequests.Add(bitmapImageRequest);
            thereMayBeWorkToDo.Set();
            return bitmapImageRequest;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            thereMayBeWorkToDo.Dispose();
        }

        private void DownloadThreadStart()
        {
            while (true)
            {
                WaitHandle.WaitAll(new WaitHandle[1]
                {
           thereMayBeWorkToDo
                });
                var key = (BitmapImageRequest)null;
                lock (executingRequests)
                {
                    lock (pendingRequests)
                    {
                        pendingRequests.Where(wr => wr.Aborted).ToList().ForEach(wr => pendingRequests.Remove(wr));
                        foreach (var pendingRequest in pendingRequests)
                            pendingRequest.NetworkPrioritySnapshot = pendingRequest.NetworkPriority;
                        pendingRequests.Sort((left, right) => Comparer<int>.Default.Compare((int)left.NetworkPrioritySnapshot, (int)right.NetworkPrioritySnapshot));
                        if (executingRequests.Count < MaxSimultaneousRequests && pendingRequests.Count > 0)
                        {
                            key = pendingRequests[pendingRequests.Count - 1];
                            pendingRequests.RemoveAt(pendingRequests.Count - 1);
                        }
                        else
                            thereMayBeWorkToDo.Reset();
                    }
                    if (key is object)
                    {
                        if (key.Uri is object)
                        {
                            var webClient = new WebClient
                            {
                                CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable)
                            };
                            webClient.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCompleted);
                            executingRequests.Add(key, webClient);
                            webClient.DownloadDataAsync(key.Uri, null);
                        }
                        else if (key.GetImage is object)
                        {
                            var error = (Exception)null;
                            var result = (BitmapImage)null;
                            try
                            {
                                result = key.GetImage(((GenericRasterTileDownloader.TileRequest)key.UserToken).TileId);
                            }
                            catch (Exception ex)
                            {
                                error = ex;
                            }
                            key.Callback(key.UserToken, result, error);
                        }
                    }
                }
            }
        }

        private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            var key = (BitmapImageRequest)null;
            lock (executingRequests)
            {
                key = executingRequests.First(item => item.Value == sender).Key;
                executingRequests.Remove(key);
                thereMayBeWorkToDo.Set();
            }
            var result = (BitmapImage)null;
            var error = e.Error;
            if (error is null)
            {
                try
                {
                    if (e.Result.Length > 0)
                    {
                        result = new BitmapImage();
                        result.BeginInit();
                        result.StreamSource = new MemoryStream(e.Result);
                        result.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                        result.CacheOption = BitmapCacheOption.None;
                        result.EndInit();
                        result.Freeze();
                    }
                    else
                        error = new Exception("empty result");
                }
                catch (Exception ex)
                {
                    error = ex;
                    result = null;
                }
            }
            key.Callback(key.UserToken, result, error);
            ((Component)sender).Dispose();
        }
    }
}
