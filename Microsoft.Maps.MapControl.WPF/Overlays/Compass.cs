using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Maps.MapControl.WPF.Overlays
{
    public partial class Compass : UserControl
    {
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(nameof(Heading), typeof(double), typeof(Compass), new PropertyMetadata(0.0));

        public Compass() => InitializeComponent();

        public double Heading
        {
            get => (double)GetValue(HeadingProperty);
            set => SetValue(HeadingProperty, value);
        }
    }
}
