using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Maps.MapExtras;

namespace Microsoft.Maps.MapControl.WPF
{
    public class MapLayer : Panel, IProjectable
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(Location), typeof(MapLayer), new PropertyMetadata(new PropertyChangedCallback(OnPositionChanged)));
        public static readonly DependencyProperty PositionRectangleProperty = DependencyProperty.RegisterAttached("PositionRectangle", typeof(LocationRect), typeof(MapLayer), new PropertyMetadata(new PropertyChangedCallback(OnPositionRectangleChanged)));
        public static readonly DependencyProperty PositionOriginProperty = DependencyProperty.RegisterAttached("PositionOrigin", typeof(PositionOrigin), typeof(MapLayer), new PropertyMetadata(new PropertyChangedCallback(OnPositionOriginChanged)));
        public static readonly DependencyProperty PositionOffsetProperty = DependencyProperty.RegisterAttached("PositionOffset", typeof(Point), typeof(MapLayer), new PropertyMetadata(new PropertyChangedCallback(OnPositionOffsetChanged)));
        private static readonly DependencyProperty ProjectionUpdatedTag = DependencyProperty.RegisterAttached("ProjectionUpdatedTagProperty", typeof(Guid), typeof(MapLayer), null);
        private Size _ViewportSize;
        private Matrix3D _NormalizedMercatorToViewport;
        private Matrix3D _ViewportToNormalizedMercator;

        public void AddChild(UIElement element, Location location)
        {
            Children.Add(element);
            SetPosition(element, location);
        }

        public void AddChild(UIElement element, Location location, Point offset)
        {
            Children.Add(element);
            SetPosition(element, location);
            SetPositionOffset(element, offset);
        }

        public void AddChild(UIElement element, Location location, PositionOrigin origin)
        {
            Children.Add(element);
            SetPosition(element, location);
            SetPositionOrigin(element, origin);
        }

        public void AddChild(UIElement element, LocationRect locationRect)
        {
            Children.Add(element);
            SetPositionRectangle(element, locationRect);
        }

        public static Location GetPosition(DependencyObject dependencyObject)
        {
            var position = (Location)dependencyObject.GetValue(PositionProperty);
            if (position is null && dependencyObject is ContentPresenter && VisualTreeHelper.GetChildrenCount(dependencyObject) > 0)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, 0);
                if (child is object)
                    position = GetPosition(child);
            }
            return position;
        }

        public static void SetPosition(DependencyObject dependencyObject, Location location) => dependencyObject.SetValue(PositionProperty, location);

        public static void OnPositionChanged(
          DependencyObject dependencyObject,
          DependencyPropertyChangedEventArgs ea) => InvalidateParentLayout(dependencyObject);

        public static LocationRect GetPositionRectangle(DependencyObject dependencyObject)
        {
            var positionRectangle = (LocationRect)dependencyObject.GetValue(PositionRectangleProperty);
            if (positionRectangle is null && dependencyObject is ContentPresenter && VisualTreeHelper.GetChildrenCount(dependencyObject) > 0)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, 0);
                if (child is object)
                    positionRectangle = GetPositionRectangle(child);
            }
            return positionRectangle;
        }

        public static void SetPositionRectangle(DependencyObject dependencyObject, LocationRect rect) => dependencyObject.SetValue(PositionRectangleProperty, rect);

        public static void OnPositionRectangleChanged(
          DependencyObject dependencyObject,
          DependencyPropertyChangedEventArgs ea) => InvalidateParentLayout(dependencyObject);

        public static PositionOrigin GetPositionOrigin(DependencyObject dependencyObject)
        {
            var positionOrigin = (PositionOrigin)dependencyObject.GetValue(PositionOriginProperty);
            if (dependencyObject is ContentPresenter && VisualTreeHelper.GetChildrenCount(dependencyObject) > 0)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, 0);
                if (child is object)
                    positionOrigin = GetPositionOrigin(child);
            }
            return positionOrigin;
        }

        public static void SetPositionOrigin(DependencyObject dependencyObject, PositionOrigin origin) => dependencyObject.SetValue(PositionOriginProperty, origin);

        public static void OnPositionOriginChanged(
          DependencyObject dependencyObject,
          DependencyPropertyChangedEventArgs ea) => InvalidateParentLayout(dependencyObject);

        public static Point GetPositionOffset(DependencyObject dependencyObject)
        {
            var positionOffset = (Point)dependencyObject.GetValue(PositionOffsetProperty);
            if (dependencyObject is ContentPresenter && VisualTreeHelper.GetChildrenCount(dependencyObject) > 0)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, 0);
                if (child is object)
                    positionOffset = GetPositionOffset(child);
            }
            return positionOffset;
        }

        public static void SetPositionOffset(DependencyObject dependencyObject, Point point) => dependencyObject.SetValue(PositionOffsetProperty, point);

        public static void OnPositionOffsetChanged(
          DependencyObject dependencyObject,
          DependencyPropertyChangedEventArgs ea) => InvalidateParentLayout(dependencyObject);

        void IProjectable.SetView(
      Size viewportSize,
      Matrix3D normalizedMercatorToViewport,
      Matrix3D viewportToNormalizedMercator)
        {
            _ViewportSize = viewportSize;
            _NormalizedMercatorToViewport = normalizedMercatorToViewport;
            _ViewportToNormalizedMercator = viewportToNormalizedMercator;
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child1 in Children)
            {
                var positionRectangle = GetPositionRectangle(child1);
                if (positionRectangle is object)
                {
                    var viewportPoint = MapMath.LocationToViewportPoint(ref _NormalizedMercatorToViewport, positionRectangle);
                    child1.Measure(new Size(viewportPoint.Width, viewportPoint.Height));
                }
                else
                {
                    if (child1 is ContentPresenter && VisualTreeHelper.GetChildrenCount(child1) > 0)
                    {
                        if (VisualTreeHelper.GetChild(child1, 0) is IProjectable child2)
                        {
                            child2.SetView(_ViewportSize, _NormalizedMercatorToViewport, _ViewportToNormalizedMercator);
                            (child2 as UIElement)?.InvalidateMeasure();
                        }
                    }
                  (child1 as IProjectable)?.SetView(_ViewportSize, _NormalizedMercatorToViewport, _ViewportToNormalizedMercator);
                    child1.Measure(_ViewportSize);
                }
            }
            return _ViewportSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                var finalRect = new Rect(0.0, 0.0, _ViewportSize.Width, _ViewportSize.Height);
                var positionRectangle = GetPositionRectangle(child);
                if (positionRectangle is object)
                {
                    finalRect = MapMath.LocationToViewportPoint(ref _NormalizedMercatorToViewport, positionRectangle);
                }
                else
                {
                    var position = GetPosition(child);
                    if (position is object && MapMath.TryLocationToViewportPoint(ref _NormalizedMercatorToViewport, position, out var viewportPosition))
                    {
                        var positionOrigin = GetPositionOrigin(child);
                        viewportPosition.X -= positionOrigin.X * child.DesiredSize.Width;
                        viewportPosition.Y -= positionOrigin.Y * child.DesiredSize.Height;
                        finalRect = new Rect(viewportPosition.X, viewportPosition.Y, child.DesiredSize.Width, child.DesiredSize.Height);
                    }
                }
                var positionOffset = GetPositionOffset(child);
                finalRect.X += positionOffset.X;
                finalRect.Y += positionOffset.Y;
                child.Arrange(finalRect);
            }
            return _ViewportSize;
        }

        protected override void OnVisualChildrenChanged(
          DependencyObject childAdded,
          DependencyObject childRemoved)
        {
            if (childAdded is object)
                (childAdded as IAttachable)?.Attach();
            if (childRemoved is object)
                (childRemoved as IAttachable)?.Detach();
            base.OnVisualChildrenChanged(childAdded, childRemoved);
        }

        private static void InvalidateParentLayout(DependencyObject dependencyObject)
        {
            if (!(dependencyObject is FrameworkElement frameworkElement))
                return;
            var parent1 = frameworkElement.Parent as MapLayer;
            if (parent1 is null)
            {
                if (frameworkElement.Parent is ContentPresenter parent2)
                    parent1 = parent2.Parent as MapLayer;
            }
            parent1?.InvalidateMeasure();
        }
    }
}
