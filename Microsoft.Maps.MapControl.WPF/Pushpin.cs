using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Maps.MapControl.WPF
{
    public class Pushpin : ContentControl
    {
        public static readonly DependencyProperty LocationDependencyProperty = DependencyProperty.Register(nameof(Location), typeof(Location), typeof(Pushpin), new PropertyMetadata(new PropertyChangedCallback(OnLocationChangedCallback)));
        public static readonly DependencyProperty PositionOriginDependencyProperty = DependencyProperty.Register(nameof(PositionOrigin), typeof(PositionOrigin), typeof(Pushpin), new PropertyMetadata(new PropertyChangedCallback(OnPositionOriginChangedCallback)));
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(nameof(Heading), typeof(double), typeof(Pushpin), new PropertyMetadata(new PropertyChangedCallback(OnHeadingChangedCallback)));

        public Pushpin()
        {
            DefaultStyleKey = typeof(Pushpin);
            SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
        }

        public Location Location
        {
            get => (Location)GetValue(LocationDependencyProperty);
            set => SetValue(LocationDependencyProperty, value);
        }

        public PositionOrigin PositionOrigin
        {
            get => (PositionOrigin)GetValue(PositionOriginDependencyProperty);
            set => SetValue(PositionOriginDependencyProperty, value);
        }

        public double Heading
        {
            get => (double)GetValue(HeadingProperty);
            set => SetValue(HeadingProperty, value);
        }

        protected void UpdateRenderTransform()
        {
            var positionOrigin = PositionOrigin;
            var rotateTransform = RenderTransform as RotateTransform;
            var heading = Heading;
            if (rotateTransform is null && heading != 0.0)
            {
                rotateTransform = new RotateTransform();
                RenderTransform = rotateTransform;
            }
            if (rotateTransform is null)
                return;
            rotateTransform.Angle = Heading;
            rotateTransform.CenterX = positionOrigin.X * ActualWidth;
            rotateTransform.CenterY = positionOrigin.Y * ActualHeight;
        }

        private static void OnLocationChangedCallback(
          DependencyObject d,
          DependencyPropertyChangedEventArgs eventArgs) => MapLayer.SetPosition(d, (Location)eventArgs.NewValue);

        private static void OnPositionOriginChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
        {
            MapLayer.SetPositionOrigin(d, (PositionOrigin)eventArgs.NewValue);
            ((Pushpin)d).UpdateRenderTransform();
        }

        private static void OnHeadingChangedCallback(
          DependencyObject d,
          DependencyPropertyChangedEventArgs eventArgs) => ((Pushpin)d).UpdateRenderTransform();

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) => UpdateRenderTransform();
    }
}
