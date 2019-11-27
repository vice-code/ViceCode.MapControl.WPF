using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Maps.MapControl.WPF.PlatformServices;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class CopyrightManager
    {
        private static readonly Dictionary<string, string> defaultCopyrightCache = new Dictionary<string, string>();
        private readonly Dictionary<CopyrightKey, BadFetchState> retryFailedFetchAt = new Dictionary<CopyrightKey, BadFetchState>(new CopyrightKeyComparer());
        private readonly TimeSpan minimumRetryInterval = new TimeSpan(0, 2, 0);
        private static CopyrightManager instance;
        private string imageryCopyrightUrlString;

        private CopyrightManager(string culture, string session) => MapConfiguration.GetSection("v1", "Services", culture, session, new MapConfigurationCallback(AsynchronousConfigurationLoaded), true);

        private void AsynchronousConfigurationLoaded(MapConfigurationSection config, object userState)
        {
            if (config is null || !config.Contains("COPYRIGHT"))
                return;
            imageryCopyrightUrlString = config["COPYRIGHT"];
            if (string.IsNullOrEmpty(imageryCopyrightUrlString))
                return;
            imageryCopyrightUrlString = imageryCopyrightUrlString.Replace("{outputType}", "xml").Replace("{heading}", "0");
        }

        public static CopyrightManager GetInstance(string culture, string session)
        {
            if (instance is null)
                instance = new CopyrightManager(culture, session);
            return instance;
        }

        internal static CopyrightManager GetCleanInstance(
          string culture,
          string session) => new CopyrightManager(culture, session);

        private static string DefaultCopyright(string culture)
        {
            if (!defaultCopyrightCache.ContainsKey(culture))
            {
                ResourceUtility.GetResource<CoreResources, CoreResourcesHelper>(culture);
                var str = string.Format(ResourceUtility.GetCultureInfo(culture), CoreResources.DefaultCopyright, DateTime.Now.Year);
                defaultCopyrightCache[culture] = str;
            }
            return defaultCopyrightCache[culture];
        }

        public void RequestCopyrightString(
            MapStyle? style,
            LocationRect boundingRectangle,
            double zoomLevel,
            CredentialsProvider credentialsProvider,
            string culture,
            Action<CopyrightResult> copyrightCallback)
        {
            if (!style.HasValue)
                return;
            if (credentialsProvider is object)
            {
                credentialsProvider.GetCredentials(credentials => RequestCopyrightString(style, boundingRectangle, zoomLevel, credentials, culture, copyrightCallback));
            }
            else
            {
                var credentials = (Credentials)null;
                RequestCopyrightString(style, boundingRectangle, zoomLevel, credentials, culture, copyrightCallback);
            }
        }

        private static bool IsInDesignMode => (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

        private void RequestCopyrightString(
            MapStyle? style,
            LocationRect boundingRectangle,
            double zoomLevel,
            Credentials credentials,
            string culture,
            Action<CopyrightResult> copyrightCallback)
        {
            var flag = IsInDesignMode;
            if (!IsInDesignMode && !string.IsNullOrEmpty(imageryCopyrightUrlString))
            {
                if (style.HasValue)
                {
                    try
                    {
                        var uriString = imageryCopyrightUrlString.Replace("{UriScheme}", Map.UriScheme).Replace("{culture}", culture).Replace("{imagerySet}", style.ToString()).Replace("{zoom}", ((int)zoomLevel).ToString()).Replace("{minLat}", ClipLatitude(boundingRectangle.South).ToString()).Replace("{minLon}", ClipLongitude(boundingRectangle.West).ToString()).Replace("{maxLat}", ClipLatitude(boundingRectangle.North).ToString()).Replace("{maxLon}", ClipLongitude(boundingRectangle.East).ToString()).Replace("{authKey}", credentials.ApplicationId);
                        using (var webClient = new WebClient())
                        {
                            var copyrightRequestState = new CopyrightRequestState(culture, style.Value, boundingRectangle, zoomLevel, credentials, copyrightCallback);
                            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(CopyrightRequestCompleted);
                            webClient.DownloadStringAsync(new Uri(uriString, UriKind.Absolute), copyrightRequestState);
                        }
                    }
                    catch (WebException)
                    {
                        flag = true;
                    }
                    catch (NotSupportedException)
                    {
                        flag = true;
                    }
                    catch (Exception)
                    {
                        flag = true;
                    }
                }
            }
            if (!flag)
                return;
            copyrightCallback(new CopyrightResult(new List<string>()
              {
                DefaultCopyright(culture)
              }, culture, boundingRectangle, zoomLevel));
        }

        private void CopyrightRequestCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var flag = false;
            var copyrightRequestState = (CopyrightRequestState)null;
            if (e.UserState is object)
                copyrightRequestState = e.UserState as CopyrightRequestState;
            if (e.Error is null && e.Result is object)
            {
                if (copyrightRequestState is object)
                {
                    try
                    {
                        var xdocument = XDocument.Parse(e.Result);
                        var xnamespace = (XNamespace)"http://schemas.microsoft.com/search/local/ws/rest/v1";
                        var copyrightResult = new CopyrightResult(new List<string>(), copyrightRequestState.Culture, copyrightRequestState.BoundingRectangle, copyrightRequestState.ZoomLevel);
                        copyrightResult.CopyrightStrings.Add(DefaultCopyright(copyrightRequestState.Culture));
                        var source = xdocument.Descendants(xnamespace + "ImageryProviders");
                        if (source is object)
                        {
                            foreach (var descendant in source.Descendants(xnamespace + "string"))
                                copyrightResult.CopyrightStrings.Add(descendant.Value);
                        }
                        copyrightRequestState.CopyrightCallback(copyrightResult);
                        goto label_17;
                    }
                    catch (XmlException)
                    {
                        flag = true;
                        goto label_17;
                    }
                    catch (Exception)
                    {
                        flag = true;
                        goto label_17;
                    }
                }
            }
            flag = true;
        label_17:
            if (!flag || copyrightRequestState is null)
                return;
            copyrightRequestState.CopyrightCallback(new CopyrightResult(new List<string>()
            {
            DefaultCopyright(copyrightRequestState.Culture)
            }, copyrightRequestState.Culture, copyrightRequestState.BoundingRectangle, copyrightRequestState.ZoomLevel));
        }

        private double ClipLatitude(double latitude)
        {
            latitude = Math.Max(latitude, -85.0);
            latitude = Math.Min(latitude, 85.0);
            return latitude;
        }

        private double ClipLongitude(double longitude)
        {
            longitude = Math.Max(longitude, -180.0);
            longitude = Math.Min(longitude, 180.0);
            return longitude;
        }
    }
}
