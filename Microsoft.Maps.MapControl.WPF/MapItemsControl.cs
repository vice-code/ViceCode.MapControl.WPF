using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapControl.WPF
{
    public class MapItemsControl : ItemsControl, IProjectable
    {
        private Size _ViewportSize = new Size();
        private Matrix3D _NormalizedMercatorToViewport = Matrix3D.Identity;
        private Matrix3D _ViewportToNormalizedMercator = Matrix3D.Identity;
        private MapLayer _MapLayer;

        public MapItemsControl()
        {
            DefaultStyleKey = typeof(MapItemsControl);
            Loaded += new RoutedEventHandler(MapItemsControl_Loaded);
        }

        private void MapItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return;
            _MapLayer = (MapLayer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this, 0), 0);
            ((IProjectable)_MapLayer).SetView(_ViewportSize, _NormalizedMercatorToViewport, _ViewportToNormalizedMercator);
        }

        void IProjectable.SetView(
          Size viewportSize,
          Matrix3D normalizedMercatorToViewport,
          Matrix3D viewportToNormalizedMercator)
        {
            _ViewportSize = viewportSize;
            _NormalizedMercatorToViewport = normalizedMercatorToViewport;
            _ViewportToNormalizedMercator = viewportToNormalizedMercator;
            if (_MapLayer is object)
                ((IProjectable)_MapLayer).SetView(viewportSize, normalizedMercatorToViewport, viewportToNormalizedMercator);
            InvalidateMeasure();
        }
    }
}
