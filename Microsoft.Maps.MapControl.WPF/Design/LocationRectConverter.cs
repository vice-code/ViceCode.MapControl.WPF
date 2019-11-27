using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Design
{
    public class LocationRectConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
        {
            if (!(value is string str))
                throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidLocationRectFormat);
            var strArray = str.Split(',');
            if (strArray.Length == 4)
                return new LocationRect(double.Parse(strArray[0], CultureInfo.InvariantCulture), double.Parse(strArray[1], CultureInfo.InvariantCulture), double.Parse(strArray[2], CultureInfo.InvariantCulture), double.Parse(strArray[3], CultureInfo.InvariantCulture));
            throw new FormatException(ExceptionStrings.TypeConverter_InvalidLocationRectFormat);
        }
    }
}
