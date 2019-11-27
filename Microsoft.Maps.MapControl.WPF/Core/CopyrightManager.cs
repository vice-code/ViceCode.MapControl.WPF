using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Maps.MapControl.WPF.PlatformServices;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class CopyrightManager
    {
        private static readonly Dictionary<string, string> defaultCopyrightCache = new Dictionary<string, string>();
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
            imageryCopyrightUrlString = imageryCopyrightUrlString
                .Replace("{outputType}", "xml", StringComparison.Ordinal)
                .Replace("{heading}", "0", StringComparison.Ordinal);
        }

        public static CopyrightManager GetInstance(string culture, string session) => instance ??= new CopyrightManager(culture, session);

        private static string DefaultCopyright(string culture)
        {
            if (!defaultCopyrightCache.ContainsKey(culture))
            {
                ResourceUtility.GetResource<CoreResources, CoreResourcesHelper>(culture);
                defaultCopyrightCache[culture] = string.Format(ResourceUtility.GetCultureInfo(culture), CoreResources.DefaultCopyright, DateTime.Now.Year);
            }
            return defaultCopyrightCache[culture];
        }

        public void RequestCopyrightString(MapStyle? style, LocationRect boundingRectangle, double zoomLevel, CredentialsProvider credentialsProvider, string culture, Action<CopyrightResult> copyrightCallback)
        {
            if (!style.HasValue)
                return;
            if (credentialsProvider is null)
                RequestCopyrightString(style, boundingRectangle, zoomLevel, (Credentials)null, culture, copyrightCallback);
            else
                credentialsProvider.GetCredentials(credentials => RequestCopyrightString(style, boundingRectangle, zoomLevel, credentials, culture, copyrightCallback));
        }

        private static bool IsInDesignMode => (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

        private void RequestCopyrightString(MapStyle? style, LocationRect boundingRectangle, double zoomLevel, Credentials credentials, string culture, Action<CopyrightResult> copyrightCallback)
        {
            var flag = IsInDesignMode;
            if (!IsInDesignMode && !string.IsNullOrEmpty(imageryCopyrightUrlString))
            {
                if (style.HasValue)
                {
                    try
                    {
                        using var webClient = new WebClient();
                        var copyrightRequestState = new CopyrightRequestState(culture, style.Value, boundingRectangle, zoomLevel, credentials, copyrightCallback);
                        webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(CopyrightRequestCompleted);
                        var uriString = imageryCopyrightUrlString
                            .Replace("{UriScheme}", Map.UriScheme, StringComparison.Ordinal)
                            .Replace("{culture}", culture, StringComparison.Ordinal)
                            .Replace("{imagerySet}", style.ToString(), StringComparison.Ordinal)
                            .Replace("{zoom}", ((int)zoomLevel).ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
                            .Replace("{minLat}", ClipLatitude(boundingRectangle.South).ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
                            .Replace("{minLon}", ClipLongitude(boundingRectangle.West).ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
                            .Replace("{maxLat}", ClipLatitude(boundingRectangle.North).ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
                            .Replace("{maxLon}", ClipLongitude(boundingRectangle.East).ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
                            .Replace("{authKey}", credentials.ApplicationId, StringComparison.Ordinal);
                        webClient.DownloadStringAsync(new Uri(uriString, UriKind.Absolute), copyrightRequestState);
                    }
                    catch (Exception)
                    {
                        flag = true;
                    }
                }
            }
            if (!flag)
                return;
            copyrightCallback(new CopyrightResult(new List<string>() { DefaultCopyright(culture) }, culture, boundingRectangle, zoomLevel));
        }

        private void CopyrightRequestCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var flag = false;
            CopyrightRequestState copyrightRequestState = null;
            if (e.UserState is object)
                copyrightRequestState = e.UserState as CopyrightRequestState;
            if (e.Error is null && e.Result is object && copyrightRequestState is object)
            {
                try
                {
                    var xdocument = XDocument.Parse(e.Result);
                    var xnamespace = (XNamespace)"http://schemas.microsoft.com/search/local/ws/rest/v1";
                    var copyrightResult = new CopyrightResult(new List<string>(), copyrightRequestState.Culture, copyrightRequestState.BoundingRectangle, copyrightRequestState.ZoomLevel);
                    copyrightResult.CopyrightStrings.Add(DefaultCopyright(copyrightRequestState.Culture));
                    var source = xdocument.Descendants($"{xnamespace}ImageryProviders");
                    if (source is object)
                    {
                        foreach (var descendant in source.Descendants($"{xnamespace}string"))
                            copyrightResult.CopyrightStrings.Add(descendant.Value);
                    }
                    copyrightRequestState.CopyrightCallback(copyrightResult);
                }
                catch (Exception)
                {
                    flag = true;
                }
            }
            else
                flag = true;
            if (!flag || copyrightRequestState is null)
                return;
            copyrightRequestState.CopyrightCallback(new CopyrightResult(
                new List<string>() { DefaultCopyright(copyrightRequestState.Culture) },
                copyrightRequestState.Culture,
                copyrightRequestState.BoundingRectangle,
                copyrightRequestState.ZoomLevel));
        }

        private static double ClipLatitude(double latitude)
        {
            latitude = Math.Max(latitude, -85.0);
            latitude = Math.Min(latitude, 85.0);
            return latitude;
        }

        private static double ClipLongitude(double longitude)
        {
            longitude = Math.Max(longitude, -180.0);
            longitude = Math.Min(longitude, 180.0);
            return longitude;
        }
    }
}
