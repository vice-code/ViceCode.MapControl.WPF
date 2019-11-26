using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    public class MapConfigurationSection
    {
        private readonly Dictionary<string, string> values;

        internal MapConfigurationSection(Dictionary<string, string> values) => this.values = values;

        public string this[string key]
        {
            get
            {
                key = key.ToUpper(CultureInfo.InvariantCulture);
                if (values.ContainsKey(key))
                    return values[key];
                return string.Empty;
            }
        }

        public bool Contains(string key)
        {
            key = key.ToUpper(CultureInfo.InvariantCulture);
            return values.ContainsKey(key);
        }
    }
}
