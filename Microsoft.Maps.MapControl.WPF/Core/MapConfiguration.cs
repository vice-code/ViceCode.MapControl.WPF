using System;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    public static class MapConfiguration
    {
        private static readonly string defaultServiceUriFormat = "{UriScheme}://dev.virtualearth.net/REST/{UriVersion}/GeospatialEndpoint/{UriCulture}/{UriRegion}/{RegionLocation}?o=xml&key={UriKey}";
        private static MapConfigurationProvider configuration;

        public static event EventHandler<MapConfigurationLoadedEventArgs> Loaded;

        public static void GetSection(
          string version,
          string sectionName,
          string culture,
          string key,
          MapConfigurationCallback callback,
          bool reExecuteCallback) => GetSection(version, sectionName, culture, key, callback, reExecuteCallback, null);

        public static void GetSection(string version, string sectionName, string culture, string key, MapConfigurationCallback callback, bool reExecuteCallback, object userState)
        {
            if (key is null)
                key = "AnzLvLWGD8KU7wavNObJJdFVQcEGKMuBfBZ08b_Fo2cLb2HANvULeuewDmPDYExL";
            if (configuration is null || configuration.Culture != culture)
            {
                if (!IsInDesignMode)
                {
                    var uriString = defaultServiceUriFormat.Replace("{UriScheme}", Map.UriScheme).Replace("{UriKey}", key).Replace("{UriVersion}", version).Replace("{UriCulture}", culture).Replace("{UriRegion}", culture.Substring(culture.Length - 2)).Replace("{RegionLocation}", "");
                    if (Map.UseHttps)
                        uriString += "&uriScheme=https";
                    LoadConfiguration(new MapConfigurationFromWeb(new Uri(uriString, UriKind.Absolute)));
                }
                else
                {
                    try
                    {
                        typeof(MapConfiguration).Assembly.GetManifestResourceNames();
                        var manifestResourceStream = typeof(MapConfiguration).Assembly.GetManifestResourceStream("Microsoft.Maps.MapControl.WPF.Data.DesignConfig.xml");
                        if (manifestResourceStream is object)
                            LoadConfiguration(new MapConfigurationFromFile(XmlReader.Create(manifestResourceStream), culture, version));
                    }
                    catch (XmlException)
                    {
                    }
                }
            }
            if (string.IsNullOrEmpty(version))
                throw new ArgumentException(ExceptionStrings.MapConfiguration_GetSection_NonNull, nameof(version));
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentException(ExceptionStrings.MapConfiguration_GetSection_NonNull, nameof(sectionName));
            configuration.GetConfigurationSection(version, sectionName, culture, callback, reExecuteCallback, userState);
        }

        internal static void Reset()
        {
            Loaded = null;
            if (configuration is null)
                return;
            configuration.Loaded -= new EventHandler<MapConfigurationLoadedEventArgs>(provider_Loaded);
            configuration = null;
        }

        private static void LoadConfiguration(MapConfigurationProvider provider)
        {
            if (configuration is object)
                configuration.Cancel();
            configuration = provider;
            provider.Loaded += new EventHandler<MapConfigurationLoadedEventArgs>(provider_Loaded);
            provider.LoadConfiguration();
        }

        private static void provider_Loaded(object sender, MapConfigurationLoadedEventArgs e)
        {
            var loaded = Loaded;
            if (loaded is null)
                return;
            loaded(null, e);
        }

        private static bool IsInDesignMode => (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;
    }
}
