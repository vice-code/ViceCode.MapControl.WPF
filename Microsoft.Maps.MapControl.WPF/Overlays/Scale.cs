using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF.Overlays
{
    public partial class Scale : UserControl
    {
        public static readonly DependencyProperty DistanceUnitProperty = DependencyProperty.Register(nameof(DistanceUnit), typeof(DistanceUnit), typeof(Scale), new PropertyMetadata(new PropertyChangedCallback(OnUnitChanged)));
        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(nameof(Culture), typeof(string), typeof(Scale), new PropertyMetadata(new PropertyChangedCallback(OnCultureChanged)));
        private static readonly int[] singleDigitValues = new int[2]
        {
      5,
      2
        };
        private static readonly double[] multiDigitValues = new double[3]
        {
      5.0,
      2.5,
      2.0
        };
        private const int MetersPerKm = 1000;
        private const double YardsPerMeter = 1.0936133;
        private const int YardsPerMile = 1760;
        private const int FeetPerYard = 3;
        private const double FeetPerMeter = 3.2808399;
        private const int FeetPerMile = 5280;
        private double _ScaleInMetersPerPixel;
        private RegionInfo regionInfo;
        private CultureInfo cultureInfo;
        private double _CurrentMetersPerPixel;
        private double _PreviousMaxWidth;
        private OverlayResources overlayResources;

        public Scale()
        {
            InitializeComponent();
            LayoutUpdated += new EventHandler(Scale_LayoutUpdated);
        }

        public double MetersPerPixel
        {
            get => _ScaleInMetersPerPixel;
            internal set
            {
                _ScaleInMetersPerPixel = value;
                OnPerPixelChanged();
            }
        }

        public DistanceUnit DistanceUnit
        {
            get => (DistanceUnit)GetValue(DistanceUnitProperty);
            set => SetValue(DistanceUnitProperty, value);
        }

        public string Culture
        {
            get => (string)GetValue(CultureProperty);
            set => SetValue(CultureProperty, value);
        }

        private OverlayResources OverlayResources
        {
            get
            {
                if (overlayResources is null)
                    overlayResources = ResourceUtility.GetResource<OverlayResources, OverlayResourcesHelper>(!string.IsNullOrEmpty(Culture) ? Culture : CultureInfo.CurrentUICulture.Name);
                return overlayResources;
            }
        }

        private void SetScaling(double metersPerPixel)
        {
            if (Visibility != Visibility.Visible || metersPerPixel <= 0.0)
                return;
            var cultureInfo = this.cultureInfo is object ? this.cultureInfo : CultureInfo.CurrentUICulture;
            var distanceUnit = DistanceUnit;
            if (distanceUnit == DistanceUnit.Default)
                distanceUnit = (regionInfo is object ? regionInfo : RegionInfo.CurrentRegion).IsMetric ? DistanceUnit.KilometersMeters : DistanceUnit.MilesFeet;
            var maxWidth = MaxWidth;
            _PreviousMaxWidth = maxWidth;
            if (DistanceUnit.KilometersMeters == distanceUnit)
            {
                var dIn = metersPerPixel * maxWidth;
                if (dIn > 1000.0)
                {
                    var num = LargestNiceNumber(dIn / 1000.0);
                    var pixels = (int)(num * 1000 / metersPerPixel);
                    var format = num == 1 ? OverlayResources.KilometersSingular : OverlayResources.KilometersPlural;
                    SetScaling(pixels, string.Format(cultureInfo, format, num));
                }
                else
                {
                    var num = LargestNiceNumber(dIn);
                    var pixels = (int)(num / metersPerPixel);
                    var format = num == 1 ? OverlayResources.MetersSingular : OverlayResources.MetersPlural;
                    SetScaling(pixels, string.Format(cultureInfo, format, num));
                }
            }
            else
            {
                var num1 = metersPerPixel * 3.2808399;
                var dIn = num1 * maxWidth;
                if (dIn > 5280.0)
                {
                    var num2 = LargestNiceNumber(dIn / 5280.0);
                    var pixels = (int)(num2 * 5280 / num1);
                    var format = num2 == 1 ? OverlayResources.MilesSingular : OverlayResources.MilesPlural;
                    SetScaling(pixels, string.Format(cultureInfo, format, num2));
                }
                else if (DistanceUnit.MilesFeet == distanceUnit)
                {
                    var num2 = LargestNiceNumber(dIn);
                    var pixels = (int)(num2 / num1);
                    var format = num2 == 1 ? OverlayResources.FeetSingular : OverlayResources.FeetPlural;
                    SetScaling(pixels, string.Format(cultureInfo, format, num2));
                }
                else
                {
                    var num2 = LargestNiceNumber(dIn / 3.0);
                    var pixels = (int)(num2 * 3 / num1);
                    var format = num2 == 1 ? OverlayResources.YardsSingular : OverlayResources.YardsPlural;
                    SetScaling(pixels, string.Format(cultureInfo, format, num2));
                }
            }
            _CurrentMetersPerPixel = metersPerPixel;
        }

        private void SetScaling(int pixels, string text)
        {
            Width = pixels + ScaleRectangle.Margin.Left + ScaleRectangle.Margin.Right;
            ScaleString.Text = text;
            ScaleRectangle.Width = pixels;
        }

        private void Refresh()
        {
            if (_CurrentMetersPerPixel <= 0.0)
                return;
            SetScaling(_CurrentMetersPerPixel);
        }

        private void Scale_LayoutUpdated(object sender, EventArgs e)
        {
            if (_PreviousMaxWidth == MaxWidth)
                return;
            Refresh();
        }

        protected virtual void OnPerPixelChanged() => SetScaling(MetersPerPixel);

        private static void OnUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((Scale)d).OnUnitChanged();

        protected virtual void OnUnitChanged() => Refresh();

        private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((Scale)d).OnCultureChanged();

        protected virtual void OnCultureChanged()
        {
            if (string.IsNullOrEmpty(Culture))
            {
                regionInfo = null;
                cultureInfo = null;
                overlayResources = ResourceUtility.GetResource<OverlayResources, OverlayResourcesHelper>(CultureInfo.CurrentUICulture.Name);
            }
            else
            {
                regionInfo = ResourceUtility.GetRegionInfo(Culture);
                cultureInfo = ResourceUtility.GetCultureInfo(Culture);
                overlayResources = ResourceUtility.GetResource<OverlayResources, OverlayResourcesHelper>(Culture);
            }
            Refresh();
        }

        private static int GetSingleDigitValue(double value)
        {
            var num = (int)Math.Floor(value);
            foreach (var singleDigitValue in singleDigitValues)
            {
                if (num > singleDigitValue)
                    return singleDigitValue;
            }
            return 1;
        }

        private static int GetMultiDigitValue(double value, double exponentOf10)
        {
            foreach (var multiDigitValue in multiDigitValues)
            {
                if (value > multiDigitValue)
                    return (int)(multiDigitValue * exponentOf10);
            }
            return (int)exponentOf10;
        }

        private static int LargestNiceNumber(double dIn)
        {
            var exponentOf10 = Math.Pow(10.0, Math.Floor(Math.Log(dIn) / Math.Log(10.0)));
            var num = dIn / exponentOf10;
            if (1.0 == exponentOf10)
                return GetSingleDigitValue(num);
            return GetMultiDigitValue(num, exponentOf10);
        }
    }
}
