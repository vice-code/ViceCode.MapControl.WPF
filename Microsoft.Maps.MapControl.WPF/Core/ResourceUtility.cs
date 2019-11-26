using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal static class ResourceUtility
    {
        private static readonly Dictionary<Type, Dictionary<string, object>> cache = new Dictionary<Type, Dictionary<string, object>>();

        private static bool TryToGetCultureInfo(string cultureName, out CultureInfo cultureInfo)
        {
            try
            {
                cultureInfo = new CultureInfo(cultureName);
            }
            catch (ArgumentException)
            {
                cultureInfo = null;
                return false;
            }
            return true;
        }

        internal static CultureInfo GetCultureInfo(string cultureName)
        {
            if (TryToGetCultureInfo(cultureName, out var cultureInfo))
                return cultureInfo;
            var length = cultureName.IndexOf('-');
            if (length > 0 && TryToGetCultureInfo(cultureName.Substring(0, length), out cultureInfo))
                return cultureInfo;
            return CultureInfo.CurrentUICulture;
        }

        private static bool TryToGetRegionInfo(string regionName, out RegionInfo regionInfo)
        {
            try
            {
                regionInfo = new RegionInfo(regionName);
            }
            catch (ArgumentException)
            {
                regionInfo = null;
                return false;
            }
            return true;
        }

        internal static RegionInfo GetRegionInfo(string regionName)
        {
            if (TryToGetRegionInfo(regionName, out var regionInfo))
                return regionInfo;
            return RegionInfo.CurrentRegion;
        }

        internal static TResourceClass GetResource<TResourceClass, THelper>(string cultureName)
          where TResourceClass : class
          where THelper : class, IResourceHelper<TResourceClass>, new()
        {
            if (!cache.TryGetValue(typeof(TResourceClass), out var dictionary))
            {
                dictionary = new Dictionary<string, object>();
                cache[typeof(TResourceClass)] = dictionary;
            }
            if (!dictionary.TryGetValue(cultureName, out var obj))
            {
                var cultureInfo = GetCultureInfo(cultureName);
                var helper = new THelper();
                var resource = helper.CreateResource();
                helper.SetResourceCulture(resource, cultureInfo);
                dictionary[cultureName] = resource;
                if (string.Compare(cultureName, cultureInfo.Name, StringComparison.OrdinalIgnoreCase) != 0)
                    dictionary[cultureInfo.Name] = resource;
                obj = resource;
            }
            return obj as TResourceClass;
        }
    }
}
