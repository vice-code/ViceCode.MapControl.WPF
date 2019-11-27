using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal abstract class MapConfigurationProvider
    {
        protected string _version = string.Empty;
        protected string _culture = string.Empty;
        private const string RootNodeName = "Response";
        private const string ResourceSets = "ResourceSets";
        private const string ResourceSet = "ResourceSet";
        private const string Resources = "Resources";
        private const string Resource = "Resource";
        private const string ServiceInfo = "ServiceInfo";
        private const string ServiceName = "ServiceName";
        private const string EndpointName = "Endpoint";

        public abstract event EventHandler<MapConfigurationLoadedEventArgs> Loaded;

        public abstract void LoadConfiguration();

        public abstract void Cancel();

        public abstract void GetConfigurationSection(
          string version,
          string sectionName,
          string culture,
          MapConfigurationCallback callback,
          bool reExecuteCallback,
          object userState);

        protected Dictionary<string, MapConfigurationSection> Sections { get; set; }

        public string Culture => _culture;

        protected static string GetConfigurationKey(string version, string sectionName, string culture)
        {
            if (string.IsNullOrEmpty(culture))
                culture = string.Empty;
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", version, sectionName, culture).ToUpper(CultureInfo.InvariantCulture);
        }

        protected MapConfigurationSection GetSection(
          string version,
          string sectionName,
          string culture)
        {
            var configurationSection = (MapConfigurationSection)null;
            if (Sections is object)
            {
                var configurationKey1 = GetConfigurationKey(version, sectionName, culture);
                if (Sections.ContainsKey(configurationKey1))
                    configurationSection = Sections[configurationKey1];
                else if (!string.IsNullOrEmpty(culture))
                {
                    var configurationKey2 = GetConfigurationKey(version, sectionName, string.Empty);
                    if (Sections.ContainsKey(configurationKey2))
                        configurationSection = Sections[configurationKey2];
                }
            }
            return configurationSection;
        }

        protected bool ContainConfigurationSection(string version, string sectionName, string culture)
        {
            if (Sections is object)
                return Sections.ContainsKey(GetConfigurationKey(version, sectionName, culture));
            return false;
        }

        protected Dictionary<string, MapConfigurationSection> ParseConfiguration(
          XmlReader sectionReader)
        {
            if (sectionReader is null)
                throw new ConfigurationNotLoadedException(ExceptionStrings.ConfigurationException_NullXml);
            var dictionary = new Dictionary<string, MapConfigurationSection>();
            if (sectionReader.Read() && sectionReader.IsStartElement() && sectionReader.LocalName == RootNodeName)
            {
                while (sectionReader.Read() && sectionReader.IsStartElement())
                {
                    if (sectionReader.LocalName != ResourceSets)
                    {
                        var localName = sectionReader.LocalName;
                        var configurationKey = GetConfigurationKey(_version, localName, _culture);
                        sectionReader.Read();
                        var str = sectionReader.Value;
                        dictionary[configurationKey] = new MapConfigurationSection(new Dictionary<string, string>()
            {
              {
                localName,
                str
              }
            });
                        sectionReader.Read();
                    }
                    else if (sectionReader.Read() && sectionReader.IsStartElement() && sectionReader.LocalName == ResourceSet)
                    {
                        while (sectionReader.Read())
                        {
                            if (sectionReader.IsStartElement() && sectionReader.LocalName == Resources && (sectionReader.Read() && sectionReader.IsStartElement()) && sectionReader.LocalName == Resource)
                            {
                                while (sectionReader.Read())
                                {
                                    if (sectionReader.IsStartElement())
                                    {
                                        var localName = sectionReader.LocalName;
                                        if (string.IsNullOrEmpty(localName))
                                            throw new XmlException(ExceptionStrings.MapConfiguration_ParseConfiguration_InvalidSection_NoVersion);
                                        var configurationKey = GetConfigurationKey(_version, localName, _culture);
                                        if (!dictionary.ContainsKey(configurationKey))
                                            dictionary[configurationKey] = ParseConfigurationSection(sectionReader.ReadSubtree());
                                        else
                                            throw new XmlException(string.Format(CultureInfo.InvariantCulture, ExceptionStrings.MapConfiguration_ParseConfiguration_DuplicateSection, localName, _version, _culture));
                                    }
                                }
                            }
                        }
                    }
                }
                return dictionary;
            }
            throw new XmlException(string.Format(CultureInfo.InvariantCulture, ExceptionStrings.MapConfiguration_ParseConfiguration_InvalidRoot, sectionReader.LocalName));
        }

        private MapConfigurationSection ParseConfigurationSection(
          XmlReader sectionReader)
        {
            var values = new Dictionary<string, string>();
            if (sectionReader.Read() && !sectionReader.IsEmptyElement)
            {
                var localName = sectionReader.LocalName;
                while (sectionReader.Read())
                {
                    if (sectionReader.IsStartElement())
                    {
                        if (sectionReader.LocalName == ServiceInfo)
                        {
                            var key = string.Empty;
                            var str = string.Empty;
                            while (sectionReader.Read())
                            {
                                if (sectionReader.IsStartElement() && sectionReader.LocalName == ServiceName)
                                {
                                    sectionReader.Read();
                                    key = sectionReader.Value.ToUpper(CultureInfo.InvariantCulture);
                                }
                                if (sectionReader.IsStartElement() && sectionReader.LocalName == EndpointName)
                                {
                                    sectionReader.Read();
                                    str = "{UriScheme}://" + sectionReader.Value;
                                }
                                if (sectionReader.LocalName == ServiceInfo)
                                    break;
                            }
                            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(str))
                            {
                                if (!values.ContainsKey(key))
                                    values.Add(key, str);
                                else
                                    throw new XmlException(string.Format(CultureInfo.InvariantCulture, ExceptionStrings.MapConfiguration_ParseConfiguration_DuplicateNodeKey, key));
                            }
                        }
                        else
                            throw new XmlException(string.Format(CultureInfo.InvariantCulture, ExceptionStrings.MapConfiguration_ParseConfiguration_InvalidTag, sectionReader.LocalName));
                    }
                    else if (!string.IsNullOrEmpty(sectionReader.Value))
                        values.Add(localName, sectionReader.Value);
                }
            }
            return new MapConfigurationSection(values);
        }
    }
}
