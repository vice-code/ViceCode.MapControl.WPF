using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Design
{
    public class PositionOriginConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
        {
            if (!(value is string name))
                throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidPositionOriginFormat);
            var strArray = name.Split(',');
            if (strArray.Length == 2)
                return new PositionOrigin(double.Parse(strArray[0], CultureInfo.InvariantCulture), double.Parse(strArray[1], CultureInfo.InvariantCulture));
            var field = typeof(PositionOrigin).GetField(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
            if (field is object)
                return field.GetValue(null);
            throw new FormatException(ExceptionStrings.TypeConverter_InvalidPositionOriginFormat);
        }
    }
}
