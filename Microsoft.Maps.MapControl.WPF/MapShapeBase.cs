using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Microsoft.Maps.MapExtras;

namespace Microsoft.Maps.MapControl.WPF
{
    public abstract class MapShapeBase : Panel, IProjectable
    {
        private static readonly DependencyProperty LocationsProperty = DependencyProperty.Register(nameof(Locations), typeof(LocationCollection), typeof(MapShapeBase), new PropertyMetadata(new PropertyChangedCallback(Locations_Changed)));
        private Point topLeftViewportPoint = new Point();
        private Size _ViewportSize;
        private Matrix3D _NormalizedMercatorToViewport;

        protected MapShapeBase(Shape shape)
        {
            EncapsulatedShape = shape;
            Children.Add(shape);
            UseLayoutRounding = false;
            EncapsulatedShape.UseLayoutRounding = false;
        }

        public Brush Fill
        {
            get => (Brush)EncapsulatedShape.GetValue(Shape.FillProperty);
            set => EncapsulatedShape.SetValue(Shape.FillProperty, value);
        }

        public Brush Stroke
        {
            get => (Brush)EncapsulatedShape.GetValue(Shape.StrokeProperty);
            set => EncapsulatedShape.SetValue(Shape.StrokeProperty, value);
        }

        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)EncapsulatedShape.GetValue(Shape.StrokeDashArrayProperty);
            set => EncapsulatedShape.SetValue(Shape.StrokeDashArrayProperty, value);
        }

        public PenLineCap StrokeDashCap
        {
            get => (PenLineCap)EncapsulatedShape.GetValue(Shape.StrokeDashCapProperty);
            set => EncapsulatedShape.SetValue(Shape.StrokeDashCapProperty, value);
        }

        public double StrokeDashOffset
        {
            get => (double)EncapsulatedShape.GetValue(Shape.StrokeDashOffsetProperty);
            set => EncapsulatedShape.SetValue(Shape.StrokeDashOffsetProperty, value);
        }

        public PenLineCap StrokeEndLineCap
        {
            get => (PenLineCap)EncapsulatedShape.GetValue(Shape.StrokeEndLineCapProperty);
            set => EncapsulatedShape.SetValue(Shape.StrokeEndLineCapProperty, value);
        }

        public PenLineJoin StrokeLineJoin
        {
            get => (PenLineJoin)EncapsulatedShape.GetValue(Shape.StrokeLineJoinProperty);
            set => EncapsulatedShape.SetValue(Shape.StrokeLineJoinProperty, value);
        }

        public double StrokeMiterLimit
        {
            get => (double)EncapsulatedShape.GetValue(Shape.StrokeMiterLimitProperty);
            set => EncapsulatedShape.SetValue(Shape.StrokeMiterLimitProperty, value);
        }

        public PenLineCap StrokeStartLineCap
        {
            get => (PenLineCap)EncapsulatedShape.GetValue(Shape.StrokeStartLineCapProperty);
            set => EncapsulatedShape.SetValue(Shape.StrokeStartLineCapProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)EncapsulatedShape.GetValue(Shape.StrokeThicknessProperty);
            set => EncapsulatedShape.SetValue(Shape.StrokeThicknessProperty, value);
        }

        public new double Opacity
        {
            get => base.Opacity;
            set => base.Opacity = value;
        }

        public new Brush OpacityMask
        {
            get => base.OpacityMask;
            set => base.OpacityMask = value;
        }

        public LocationCollection Locations
        {
            get => (LocationCollection)GetValue(LocationsProperty);
            set => SetValue(LocationsProperty, value);
        }

        private static void Locations_Changed(DependencyObject o, DependencyPropertyChangedEventArgs ea)
        {
            if (!(o is MapShapeBase mapShapeBase))
                return;
            if (ea.OldValue is LocationCollection oldValue)
                oldValue.CollectionChanged -= new NotifyCollectionChangedEventHandler(mapShapeBase.Locations_CollectionChanged);
            if (!(ea.NewValue is LocationCollection newValue))
                return;
            newValue.CollectionChanged += new NotifyCollectionChangedEventHandler(mapShapeBase.Locations_CollectionChanged);
        }

        private void Locations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => InvalidateMeasure();

        protected abstract PointCollection ProjectedPoints { get; set; }

        protected Shape EncapsulatedShape { get; }

        void IProjectable.SetView(
      Size viewportSize,
      Matrix3D normalizedMercatorToViewport,
      Matrix3D viewportToNormalizedMercator)
        {
            _ViewportSize = viewportSize;
            _NormalizedMercatorToViewport = normalizedMercatorToViewport;
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Locations is object)
            {
                var pointCollection = new PointCollection();
                var point1 = new Point(double.MaxValue, double.MaxValue);
                var pointList = (IList<Point>)new List<Point>(Locations.Count);
                foreach (var location in Locations)
                {
                    MapMath.TryLocationToViewportPoint(ref _NormalizedMercatorToViewport, location, out var viewportPosition);
                    pointList.Add(viewportPosition);
                }
                foreach (var point2 in pointList)
                {
                    point1.X = Math.Min(point1.X, point2.X);
                    point1.Y = Math.Min(point1.Y, point2.Y);
                    pointCollection.Add(point2);
                }
                for (var index = 0; index < pointCollection.Count; ++index)
                    pointCollection[index] = new Point(pointCollection[index].X - point1.X, pointCollection[index].Y - point1.Y);
                ProjectedPoints = pointCollection;
                if (ProjectedPoints.Count > 0 && Locations.Count > 0 && MapMath.TryLocationToViewportPoint(ref _NormalizedMercatorToViewport, Locations[0], out var viewportPosition1))
                    topLeftViewportPoint = new Point(viewportPosition1.X - ProjectedPoints[0].X, viewportPosition1.Y - ProjectedPoints[0].Y);
            }
            else
                ProjectedPoints.Clear();
            Children[0].Measure(new Size(double.MaxValue, double.MaxValue));
            return _ViewportSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Children[0].Arrange(new Rect(topLeftViewportPoint.X, topLeftViewportPoint.Y, Children[0].DesiredSize.Width + 1.0, Children[0].DesiredSize.Height + 1.0));
            return _ViewportSize;
        }
    }
}
