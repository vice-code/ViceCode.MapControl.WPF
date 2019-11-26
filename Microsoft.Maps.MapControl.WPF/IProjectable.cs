using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapControl.WPF
{
    internal interface IProjectable
    {
        void SetView(
          Size viewportSize,
          Matrix3D normalizedMercatorToViewport,
          Matrix3D viewportToNormalizedMercator);
    }
}
