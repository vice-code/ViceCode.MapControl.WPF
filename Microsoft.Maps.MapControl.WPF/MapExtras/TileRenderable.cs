using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapExtras
{
    internal class TileRenderable
    {
        private Canvas layerCanvas;
        private FrameworkElement element;
        private Rect clip;
        private readonly TimeSpan? fadeInDuration;
        private bool configurationDirty;
        private bool allowHardwareAcceleration;
        private bool useHardwareAcceleration;
        private int zIndex;
        private double targetOpacity;
        private DateTime lastFrameTime;
        private bool registeredForCompositionTargetRendering;
        private Canvas elementCanvas;

        public TileRenderable(TileId tileId, FrameworkElement element, TimeSpan? fadeInDuration)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));
            if (element.Parent is object)
                throw new ArgumentException("element.Parent not null");
            TileId = tileId;
            Element = element;
            Canvas.SetLeft(element, 0.0);
            Canvas.SetTop(element, 0.0);
            this.fadeInDuration = fadeInDuration;
            Opacity = TargetOpacity = 0.0;
            configurationDirty = true;
        }

        public TileId TileId { get; protected set; }

        public bool AllowHardwareAcceleration
        {
            get => allowHardwareAcceleration;
            set
            {
                if (allowHardwareAcceleration == value)
                    return;
                allowHardwareAcceleration = value;
                useHardwareAcceleration = allowHardwareAcceleration;
                configurationDirty = true;
            }
        }

        public bool IsVisible => layerCanvas is object;

        public Canvas LayerCanvas
        {
            get => layerCanvas;
            internal set
            {
                if (layerCanvas == value)
                    return;
                var frameworkElement = elementCanvas is object ? elementCanvas : element;
                if (layerCanvas is null && value is object)
                    EnsureRegisteredForCompositionTargetRendering(true);
                else if (layerCanvas is object && value is null)
                    EnsureRegisteredForCompositionTargetRendering(false);
                if (layerCanvas is object)
                    layerCanvas.Children.Remove(frameworkElement);
                layerCanvas = value;
                if (layerCanvas is object)
                    layerCanvas.Children.Add(frameworkElement);
                if (elementCanvas is null)
                    return;
                if (layerCanvas is null)
                {
                    elementCanvas.Children.Remove(element);
                }
                else
                {
                    if (element.Parent is object)
                        return;
                    elementCanvas.Children.Add(element);
                }
            }
        }

        public double TargetOpacity
        {
            get => targetOpacity;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (targetOpacity == value)
                    return;
                targetOpacity = value;
                if (!fadeInDuration.HasValue)
                    Opacity = targetOpacity;
                if (Opacity == targetOpacity || LayerCanvas is null)
                    return;
                EnsureRegisteredForCompositionTargetRendering(true);
            }
        }

        public double Opacity
        {
            get => element.Opacity;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (element.Opacity != targetOpacity && !fadeInDuration.HasValue)
                    element.Opacity = targetOpacity;
                else
                    element.Opacity = value;
                if (element.Opacity != 1.0 || BecameFullyOpaque is null)
                    return;
                BecameFullyOpaque(this, EventArgs.Empty);
            }
        }

        public event EventHandler BecameFullyOpaque;

        public int ZIndex
        {
            get => zIndex;
            set
            {
                if (zIndex == value)
                    return;
                zIndex = value;
                configurationDirty = true;
            }
        }

        public Rect Clip
        {
            get => clip;
            set
            {
                clip = value;
                configurationDirty = true;
            }
        }

        public FrameworkElement Element
        {
            get => element;
            protected set
            {
                element = value;
                AutomationProperties.SetAutomationId(element, string.Format(CultureInfo.InvariantCulture, "{0}_{1}x{2}", TileId.LevelOfDetail, TileId.X, TileId.Y));
            }
        }

        public void DetachFromElement()
        {
            EnsureRegisteredForCompositionTargetRendering(false);
            element = null;
        }

        public virtual void NoLongerRendering()
        {
        }

        public virtual void Render(
          Point2D viewportSize,
          ref Matrix3D tileToViewport,
          double preciseRenderLod)
        {
            EnsureConfiguration();
            var num1 = 0.01;
            var num2 = Math.Max(viewportSize.X, viewportSize.Y);
            var num3 = (1.0 - num2 / (num2 + num1)) / Math.Max(element.ActualWidth, element.ActualHeight);
            if (Math.Abs(tileToViewport.M14 / tileToViewport.M44) + Math.Abs(tileToViewport.M24 / tileToViewport.M44) >= num3)
                throw new InvalidOperationException("WPF implementation doesn't support 3D.");
            var matrixTransform = (MatrixTransform)element.RenderTransform;
            if (matrixTransform is null || matrixTransform.Matrix.IsIdentity)
            {
                matrixTransform = new MatrixTransform();
                element.RenderTransform = matrixTransform;
            }
            var matrix = new Matrix(tileToViewport.M11 / tileToViewport.M44, tileToViewport.M12 / tileToViewport.M44, tileToViewport.M21 / tileToViewport.M44, tileToViewport.M22 / tileToViewport.M44, tileToViewport.OffsetX / tileToViewport.M44, tileToViewport.OffsetY / tileToViewport.M44);
            if (Math.Abs(Math.Abs(matrix.M11) - 1.0) < 0.0001 && Math.Abs(matrix.M21) < 0.0001 && (Math.Abs(Math.Abs(matrix.M22) - 1.0) < 0.0001 && Math.Abs(matrix.M12) < 0.0001) || Math.Abs(Math.Abs(matrix.M21) - 1.0) < 0.0001 && Math.Abs(matrix.M11) < 0.0001 && (Math.Abs(Math.Abs(matrix.M12) - 1.0) < 0.0001 && Math.Abs(matrix.M22) < 0.0001))
            {
                matrix.M11 = Math.Round(matrix.M11);
                matrix.M12 = Math.Round(matrix.M12);
                matrix.M21 = Math.Round(matrix.M21);
                matrix.M22 = Math.Round(matrix.M22);
                matrix.OffsetX = Math.Round(matrix.OffsetX);
                matrix.OffsetY = Math.Round(matrix.OffsetY);
            }
            matrixTransform.Matrix = matrix;
            if (useHardwareAcceleration)
                return;
            SetPostProjectionClip(viewportSize, ref tileToViewport);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - lastFrameTime;
            lastFrameTime = utcNow;
            if (element is null)
                return;
            if (Opacity != targetOpacity && timeSpan.Ticks > 0L)
            {
                var num1 = targetOpacity - Opacity;
                var num2 = timeSpan.Ticks / (double)fadeInDuration.Value.Ticks;
                if (Math.Abs(num1) < num2)
                    Opacity = targetOpacity;
                else
                    Opacity += num1 > 0.0 ? num2 : -num2;
            }
            if (Opacity != targetOpacity)
                return;
            EnsureRegisteredForCompositionTargetRendering(false);
        }

        private void EnsureRegisteredForCompositionTargetRendering(bool registered)
        {
            if (registered)
            {
                if (registeredForCompositionTargetRendering)
                    return;
                lastFrameTime = DateTime.UtcNow;
                CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
                registeredForCompositionTargetRendering = true;
            }
            else
            {
                if (!registeredForCompositionTargetRendering)
                    return;
                CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
                registeredForCompositionTargetRendering = false;
            }
        }

        private void EnsureConfiguration()
        {
            if (!configurationDirty)
                return;
            if (useHardwareAcceleration)
            {
                if (elementCanvas is object)
                {
                    var layerCanvas = this.layerCanvas;
                    LayerCanvas = null;
                    elementCanvas = null;
                    LayerCanvas = layerCanvas;
                }
            }
            else if (elementCanvas is null)
            {
                var layerCanvas = this.layerCanvas;
                LayerCanvas = null;
                var canvas = new Canvas
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    IsHitTestVisible = false,
                    Tag = "ElementCanvas"
                };
                elementCanvas = canvas;
                Canvas.SetLeft(elementCanvas, 0.0);
                Canvas.SetTop(elementCanvas, 0.0);
                LayerCanvas = layerCanvas;
            }
            if (useHardwareAcceleration)
            {
                if (element.Width <= 0.0 || double.IsNaN(element.Width) || (element.Height <= 0.0 || double.IsNaN(element.Height)))
                    throw new ArgumentException("Element size must be set and positive.");
                var clip = element.Clip as RectangleGeometry;
                if (Math.Abs(this.clip.Left) > 1E-06 || Math.Abs(this.clip.Top) > 1E-06 || (Math.Abs(this.clip.Width - element.Width) > 1E-06 || Math.Abs(this.clip.Height - element.Height) > 1E-06))
                {
                    if (clip is null || clip.Rect != this.clip)
                        element.Clip = new RectangleGeometry()
                        {
                            Rect = this.clip
                        };
                }
                else if (element.Clip is object)
                    element.Clip = null;
            }
            else if (element.Clip is object)
                element.Clip = null;
            if (useHardwareAcceleration)
            {
                if (element.CacheMode is null)
                    element.CacheMode = new BitmapCache();
            }
            else if (element.CacheMode is object)
                element.CacheMode = null;
            if (useHardwareAcceleration)
            {
                if (Panel.GetZIndex(element) != zIndex)
                    Panel.SetZIndex(element, zIndex);
            }
            else if (Panel.GetZIndex(elementCanvas) != zIndex)
                Panel.SetZIndex(elementCanvas, zIndex);
            configurationDirty = false;
        }

        private void SetPostProjectionClip(Point2D viewportSize, ref Matrix3D tileToViewportTransform)
        {
            var poly = new Point4D[4]
            {
        VectorMath.Transform(tileToViewportTransform, new Point4D(clip.Left, clip.Top, 0.0, 1.0)),
        VectorMath.Transform(tileToViewportTransform, new Point4D(clip.Right, clip.Top, 0.0, 1.0)),
        VectorMath.Transform(tileToViewportTransform, new Point4D(clip.Right, clip.Bottom, 0.0, 1.0)),
        VectorMath.Transform(tileToViewportTransform, new Point4D(clip.Left, clip.Bottom, 0.0, 1.0))
            };
            var clipBounds = new RectangularSolid(0.0, 0.0, 0.0, viewportSize.X, viewportSize.Y, 1.0);
            var clippedPoly = new Point4D[poly.Length + 6];
            var tempVertexBuffer = new Point4D[poly.Length + 6];
            VectorMath.ClipConvexPolygon(clipBounds, poly, null, 4, clippedPoly, null, out var clippedPolyVertexCount, tempVertexBuffer, null);
            if (clippedPolyVertexCount <= 0)
                return;
            for (var index = 0; index < clippedPolyVertexCount; ++index)
            {
                clippedPoly[index].X /= clippedPoly[index].W;
                clippedPoly[index].Y /= clippedPoly[index].W;
            }
            var pathGeometry = new PathGeometry();
            var pathFigure = new PathFigure()
            {
                StartPoint = new Point(clippedPoly[0].X, clippedPoly[0].Y),
                Segments = new PathSegmentCollection()
            };
            var polyLineSegment = new PolyLineSegment();
            for (var index = 1; index < clippedPolyVertexCount; ++index)
                polyLineSegment.Points.Add(new Point(clippedPoly[index].X, clippedPoly[index].Y));
            polyLineSegment.Points.Add(new Point(clippedPoly[0].X, clippedPoly[0].Y));
            pathFigure.Segments.Add(polyLineSegment);
            pathGeometry.Figures.Add(pathFigure);
            elementCanvas.Clip = pathGeometry;
        }
    }
}
