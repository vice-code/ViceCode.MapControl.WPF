using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Design
{
    public class ApplicationIdCredentialsProviderConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string applicationId)
                return new ApplicationIdCredentialsProvider(applicationId);
            throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidApplicationIdCredentialsProvider);
        }
    }
}
