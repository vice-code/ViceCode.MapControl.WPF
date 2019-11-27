using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Maps.MapControl.WPF.Core;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF
{
    public class TileSource : INotifyPropertyChanged
    {
        public const string QuadKeyUriFragment = "{quadkey}";
        public const string UriSchemeUriFragment = "{UriScheme}";
        public const string UriCulture = "{UriCulture}";
        public const string UriRegion = "{UriRegion}";
        public const string UriVersion = "{UriVersion}";
        public const string UriKey = "{UriKey}";
        public const string SubdomainUriFragment = "{subdomain}";
        public const string UriRegionLoc = "{RegionLocation}";
        private const string InternalQuadKeyUriFragment = "{QUADKEY}";
        private const string InternalSubdomainUriFragment = "{SUBDOMAIN}";
        private string uriFormat;
        private string convertedUriFormat;
        private Visibility visibility;
        private string[][] subdomainsList;
        private int maxX;
        private int maxY;

        public TileSource()
        {
            subdomainsList = new string[2][]
            {
        new string[4]{ "0", "2", "4", "6" },
        new string[4]{ "1", "3", "5", "7" }
            };
            maxX = 2;
            maxY = 4;
        }

        public TileSource(string uriFormat)
          : this() => UriFormat = uriFormat;

        public virtual Uri GetUri(int x, int y, int zoomLevel)
        {
            var uri = (Uri)null;
            var quadKey = new QuadKey(x, y, zoomLevel);
            if (!string.IsNullOrEmpty(convertedUriFormat) && !string.IsNullOrEmpty(quadKey.Key) && Visibility == Visibility.Visible)
                uri = new Uri(convertedUriFormat.Replace("{QUADKEY}", quadKey.Key).Replace("{SUBDOMAIN}", GetSubdomain(quadKey)));
            return uri;
        }

        public virtual string GetSubdomain(QuadKey quadKey)
        {
            if (subdomainsList is null)
                return string.Empty;
            return subdomainsList[quadKey.X % maxX][quadKey.Y % maxY];
        }

        public void SetSubdomains(string[][] subdomains)
        {
            if (subdomains is object)
            {
                if (subdomains.Length == 0 || subdomains[0].Length == 0)
                    throw new ArgumentException(ExceptionStrings.TileSource_InvalidSubdomains_LengthMoreThan0);
                var length = subdomains[0].Length;
                foreach (var subdomain in subdomains)
                {
                    if (subdomain.Length != length)
                        throw new ArgumentException(ExceptionStrings.TileSource_InvalidSubdomains_DifferentLength);
                    foreach (var str in subdomain)
                    {
                        if (str is null)
                            throw new ArgumentException(ExceptionStrings.TileSource_InvalidSubdomain_stringNull);
                    }
                }
                subdomainsList = subdomains;
                maxX = subdomains.Length;
                maxY = length;
            }
            else
                subdomainsList = null;
        }

        public virtual BitmapImage GetImage(long x, long y, int zoomLevel) => null;

        public string UriFormat
        {
            get => uriFormat;
            set
            {
                if (!(uriFormat != value))
                    return;
                uriFormat = value;
                convertedUriFormat = ReplaceString(uriFormat, "{UriScheme}", Map.UriScheme);
                convertedUriFormat = ReplaceString(convertedUriFormat, "{quadkey}", "{QUADKEY}");
                convertedUriFormat = ReplaceString(convertedUriFormat, "{subdomain}", "{SUBDOMAIN}");
                OnPropertyChanged(nameof(UriFormat));
            }
        }

        public Visibility Visibility
        {
            get => visibility;
            set
            {
                visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        public virtual bool SuppliesImagesDirectly => DirectImage is object;

        public ImageCallback DirectImage { get; set; }

        private static string ReplaceString(string input, string pattern, string replacement) => Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged is null)
                return;
            var e = new PropertyChangedEventArgs(propertyName);
            propertyChanged(this, e);
        }
    }
}
