using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Design
{
    public class LocationCollectionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
        {
            if (!(value is string str1))
                throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidLocationCollection);
            var locationCollection = new LocationCollection();
            var locationConverter = new LocationConverter();
            var num = -1;
            for (var index = 0; index < str1.Length + 1; ++index)
            {
                if (index >= str1.Length || char.IsWhiteSpace(str1[index]))
                {
                    var startIndex = num + 1;
                    var length = index - startIndex;
                    if (length >= 1)
                    {
                        var str2 = str1.Substring(startIndex, length);
                        locationCollection.Add((Location)locationConverter.ConvertFrom(str2));
                    }
                    num = index;
                }
            }
            return locationCollection;
        }
    }
}
