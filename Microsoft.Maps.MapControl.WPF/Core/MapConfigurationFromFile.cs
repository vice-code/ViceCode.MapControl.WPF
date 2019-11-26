using System;
using System.Xml;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class MapConfigurationFromFile : MapConfigurationProvider
    {
        private readonly XmlReader config;

        public MapConfigurationFromFile(XmlReader config, string culture, string version)
        {
            _culture = culture;
            _version = version;
            this.config = config;
        }

        public override event EventHandler<MapConfigurationLoadedEventArgs> Loaded;

        public override void LoadConfiguration()
        {
            try
            {
                Sections = ParseConfiguration(config);
                if (Loaded is null)
                    return;
                Loaded(this, new MapConfigurationLoadedEventArgs(null));
            }
            catch (Exception ex)
            {
                if (Loaded is null)
                    return;
                Loaded(this, new MapConfigurationLoadedEventArgs(ex));
            }
        }

        public override void Cancel()
        {
        }

        public override void GetConfigurationSection(string version, string sectionName, string culture, MapConfigurationCallback callback, bool reExecuteCallback, object userState)
        {
            _version = version;
            _culture = culture;
            if (callback is null)
                return;
            callback(GetSection(version, sectionName, culture), userState);
        }
    }
}
