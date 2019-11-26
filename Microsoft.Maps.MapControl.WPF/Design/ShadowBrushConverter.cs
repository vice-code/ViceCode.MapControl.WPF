using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF.Design
{
    internal class ShadowBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var modeBackground = (ModeBackground)value;
                var flag = parameter.ToString().ToUpper(CultureInfo.InvariantCulture) == "TOP";
                return modeBackground != ModeBackground.Dark ? (flag ? Brushes.Black : Brushes.White) : (flag ? Brushes.White : Brushes.Black);
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
