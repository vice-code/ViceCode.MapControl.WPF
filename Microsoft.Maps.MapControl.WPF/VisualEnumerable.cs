using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Maps.MapControl.WPF
{
    internal static class VisualEnumerable
    {
        internal static IEnumerable<T> GetVisualOfType<T>(this DependencyObject element) => element.GetVisualTree().Where(t => t.GetType() == typeof(T)).Cast<T>();

        internal static IEnumerable<DependencyObject> GetVisualTree(
      this DependencyObject element)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (var i = 0; i < childrenCount; ++i)
            {
                var visualChild = VisualTreeHelper.GetChild(element, i);
                yield return visualChild;
                foreach (var dependencyObject in visualChild.GetVisualTree())
                    yield return dependencyObject;
            }
        }
    }
}
