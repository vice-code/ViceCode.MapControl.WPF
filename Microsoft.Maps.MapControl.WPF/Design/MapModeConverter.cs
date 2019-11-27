using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Design
{
    public class MapModeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
        {
            if (value is string b)
            {
                foreach (var property in typeof(MapModes).GetProperties())
                {
                    if (string.Equals(property.Name, b, StringComparison.OrdinalIgnoreCase) || string.Equals(property.PropertyType.FullName, b, StringComparison.OrdinalIgnoreCase))
                        return property.GetValue(null, null);
                }
                throw new FormatException(ExceptionStrings.TypeConverter_InvalidMapMode);
            }
            throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidMapMode);
        }
    }
}
