using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Design
{
    public class LocationConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
        {
            if (value is string str)
            {
                var strArray = str.Split(',');
                switch (strArray.Length)
                {
                    case 2:
                        double result1;
                        double result2;
                        if (double.TryParse(strArray[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result1) && double.TryParse(strArray[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
                            return new Location(result1, result2);
                        break;
                    case 3:
                        double result3;
                        double result4;
                        double result5;
                        if (double.TryParse(strArray[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result3) && double.TryParse(strArray[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result4) && double.TryParse(strArray[2], NumberStyles.Float, CultureInfo.InvariantCulture, out result5))
                            return new Location(result3, result4, result5);
                        break;
                    case 4:
                        double result6;
                        double result7;
                        if (double.TryParse(strArray[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result6) && double.TryParse(strArray[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result7))
                        {
                            if (double.TryParse(strArray[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var result8))
                            {
                                try
                                {
                                    var altitudeReference = (AltitudeReference)Enum.Parse(typeof(AltitudeReference), strArray[3], true);
                                    if (!string.IsNullOrEmpty(Enum.GetName(typeof(AltitudeReference), altitudeReference)))
                                        return new Location(result6, result7, result8, altitudeReference);
                                    break;
                                }
                                catch (ArgumentException)
                                {
                                    break;
                                }
                            }
                            else
                                break;
                        }
                        else
                            break;
                }
                throw new FormatException(ExceptionStrings.TypeConverter_InvalidLocationFormat);
            }
            throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidLocationFormat);
        }
    }
}
