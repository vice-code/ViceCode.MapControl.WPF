using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Microsoft.Maps.MapControl.WPF.Core;
using Microsoft.Maps.MapControl.WPF.Design;
using Microsoft.Maps.MapControl.WPF.Resources;
using Microsoft.Maps.MapExtras;

namespace Microsoft.Maps.MapControl.WPF
{
    [ContentProperty("Children")]
    public class MapCore : Control, IDisposable
    {
        public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register(nameof(ZoomLevel), typeof(double), typeof(MapCore), new PropertyMetadata(1d, new PropertyChangedCallback(OnZoomLevelChanged)));
        public static readonly DependencyProperty ScaleVisibilityProperty = DependencyProperty.Register(nameof(ScaleVisibility), typeof(Visibility), typeof(MapCore), new PropertyMetadata(new PropertyChangedCallback(OnScaleVisibilityChanged)));
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(nameof(Heading), typeof(double), typeof(MapCore), new PropertyMetadata(0d, new PropertyChangedCallback(OnHeadingChanged)));
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(nameof(Center), typeof(Location), typeof(MapCore), new PropertyMetadata(new Location(), new PropertyChangedCallback(OnCenterChanged)));
        public static readonly DependencyProperty CredentialsProviderProperty = DependencyProperty.Register(nameof(CredentialsProvider), typeof(CredentialsProvider), typeof(MapCore), new PropertyMetadata(new PropertyChangedCallback(OnCredentialsProviderChangedCallback)));
        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(nameof(Culture), typeof(string), typeof(MapCore), new PropertyMetadata((sender, e) => (sender as MapCore).UpdateCulture()));
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(MapMode), typeof(MapCore), new PropertyMetadata(null, (sender, e) => (sender as MapCore).UpdateMapMode()));
        private Grid _MapContainer;
        private readonly Grid _MapModeContainer;
        private readonly MapLayer _MapUserLayerContainer;
        private readonly List<MapMode> _MapModes;
        private MapMode _CurrentMapMode;
        private readonly Timer _CurrentMapModeTransitionTimeout;
        private MapMode _PendingMapMode;
        private readonly AnimationDriver _ZoomAndPanAnimationDriver;
        private readonly ZoomAndPanAnimator _ZoomAndPanAnimator;
        private Rect _ZoomAndPan_FromRect;
        private double _ZoomAndPan_FromZoomLevel;
        private bool _ViewUpdatingInternally;
        private readonly CriticallyDampedSpring _HeadingSpring;
        private Point? _ZoomAndRotateOrigin;
        private readonly CriticallyDampedSpring _ZoomLevelSpring;
        private readonly CriticallyDampedSpring _CenterNormalizedMercatorSpringX;
        private readonly CriticallyDampedSpring _CenterNormalizedMercatorSpringY;
        private bool _ViewIsAnimating;
        private bool _Disposed;
        private readonly AnimationDriver _ModeSwitchAnationDriver;
        private static Matrix3D _NormalizedMercatorToViewport_TranslatePre;
        private static Matrix3D _NormalizedMercatorToViewport_Scale;
        private static Matrix3D _NormalizedMercatorToViewport_Rotate;
        private static Matrix3D _NormalizedMercatorToViewport_TranslatePost;
        private readonly DispatcherTimer _UserInputTimeout;

        public MapCore()
        {
            MapForegroundContainer = new Grid();
            _MapModeContainer = new Grid();
            _MapUserLayerContainer = new MapLayer();
            _MapModes = new List<MapMode>();
            AnimationLevel = AnimationLevel.UserInput;
            _ZoomAndPanAnimator = new ZoomAndPanAnimator();
            _ZoomAndPanAnimationDriver = new AnimationDriver();
            _ZoomAndPanAnimationDriver.AnimationProgressChanged += new EventHandler(_ZoomAndPanAnimationDriver_AnimationProgressChanged);
            _ZoomAndPanAnimationDriver.AnimationStopped += new EventHandler(_ZoomAndPanAnimationDriver_AnimationStopped);
            _ZoomAndPanAnimationDriver.AnimationCompleted += new EventHandler(_ZoomAndPanAnimationDriver_AnimationCompleted);
            _ZoomAndRotateOrigin = new Point?(new Point());
            _HeadingSpring = new CriticallyDampedSpring();
            _HeadingSpring.SnapToValue(Heading);
            _ZoomLevelSpring = new CriticallyDampedSpring();
            _ZoomLevelSpring.SnapToValue(ZoomLevel);
            var normalizedMercator = ConvertLocationToNormalizedMercator(Center);
            _CenterNormalizedMercatorSpringX = new CriticallyDampedSpring();
            _CenterNormalizedMercatorSpringX.SnapToValue(normalizedMercator.X);
            _CenterNormalizedMercatorSpringY = new CriticallyDampedSpring();
            _CenterNormalizedMercatorSpringY.SnapToValue(normalizedMercator.Y);
            _NormalizedMercatorToViewport_TranslatePre = Matrix3D.Identity;
            _NormalizedMercatorToViewport_Scale = Matrix3D.Identity;
            _NormalizedMercatorToViewport_Rotate = Matrix3D.Identity;
            _NormalizedMercatorToViewport_TranslatePost = Matrix3D.Identity;
            ModeCrossFadeDuration = new Duration(TimeSpan.FromMilliseconds(500));
            _CurrentMapModeTransitionTimeout = new Timer(4000)
            {
                AutoReset = false
            };
            _CurrentMapModeTransitionTimeout.Elapsed += new ElapsedEventHandler(_CurrentMapModeTransitionTimeout_Elapsed);
            _ModeSwitchAnationDriver = new AnimationDriver();
            _ModeSwitchAnationDriver.AnimationProgressChanged += new EventHandler(_ModeSwitchAnationDriver_AnimationProgressChanged);
            _ModeSwitchAnationDriver.AnimationCompleted += new EventHandler(_ModeSwitchAnationDriver_AnimationCompleted);
            Loaded += new RoutedEventHandler(MapCore_Loaded);
            _UserInputTimeout = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 100)
            };
            UpdateView();
        }

        public override void OnApplyTemplate()
        {
            if (_MapContainer is object)
            {
                _MapContainer.SizeChanged -= new SizeChangedEventHandler(_MapContainer_SizeChanged);
                _MapContainer.Children.Clear();
            }
            _MapContainer = (Grid)GetTemplateChild("MapContainer");
            _MapContainer.Clip = new RectangleGeometry();
            _MapContainer.SizeChanged += new SizeChangedEventHandler(_MapContainer_SizeChanged);
            _MapContainer.Children.Add(_MapModeContainer);
            _MapContainer.Children.Add(_MapUserLayerContainer);
            _MapContainer.Children.Add(MapForegroundContainer);
        }

        public void Dispose()
        {
            if (Parent is object)
                throw new InvalidOperationException("Cannot be disposed while still in the visual tree.");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed)
                return;
            if (disposing)
            {
                _CurrentMapModeTransitionTimeout.Dispose();
                foreach (var mapMode in _MapModes)
                    mapMode.Detach();
                _MapModes.Clear();
                if (_PendingMapMode is object)
                {
                    _PendingMapMode.Detach();
                    _PendingMapMode = null;
                }
                ViewIsAnimating = false;
                _ZoomAndPanAnimationDriver.Stop();
                _ModeSwitchAnationDriver.Stop();
                _MapContainer.SizeChanged -= new SizeChangedEventHandler(_MapContainer_SizeChanged);
            }
            _Disposed = true;
        }

        public double ZoomLevel
        {
            get => (double)GetValue(ZoomLevelProperty);
            set => SetValue(ZoomLevelProperty, value);
        }

        private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MapCore)d).OnZoomLevelChanged(e);

        private void OnZoomLevelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_ViewUpdatingInternally)
                return;
            var animationLevel = AnimationLevel;
            AnimationLevel = AnimationLevel.None;
            SetView((double)e.NewValue, _HeadingSpring.TargetValue);
            AnimationLevel = animationLevel;
        }

        public Visibility ScaleVisibility
        {
            get => (Visibility)GetValue(ScaleVisibilityProperty);
            set => SetValue(ScaleVisibilityProperty, value);
        }

        private static void OnScaleVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MapCore)d).OnScaleVisibilityChanged(e);

        protected virtual void OnScaleVisibilityChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
        }

        public double Heading
        {
            get => (double)GetValue(HeadingProperty);
            set => SetValue(HeadingProperty, value);
        }

        private static void OnHeadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MapCore)d).OnHeadingChanged(e);

        private void OnHeadingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_ViewUpdatingInternally)
                return;
            var animationLevel = AnimationLevel;
            AnimationLevel = AnimationLevel.None;
            SetView(_ZoomLevelSpring.CurrentValue, (double)e.NewValue);
            AnimationLevel = animationLevel;
        }

        public Location Center
        {
            get => (Location)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        private static void OnCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MapCore)d).OnCenterChanged(e);

        private void OnCenterChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_ViewUpdatingInternally)
                return;
            var animationLevel = AnimationLevel;
            AnimationLevel = AnimationLevel.None;
            SetView((Location)e.NewValue, _ZoomLevelSpring.TargetValue);
            AnimationLevel = animationLevel;
        }

        [TypeConverter(typeof(ApplicationIdCredentialsProviderConverter))]
        public CredentialsProvider CredentialsProvider
        {
            get => (CredentialsProvider)GetValue(CredentialsProviderProperty);
            set => SetValue(CredentialsProviderProperty, value);
        }

        private static void OnCredentialsProviderChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs) => ((MapCore)d).OnCredentialsProviderChanged(eventArgs);

        protected virtual void OnCredentialsProviderChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
        }

        public string Culture
        {
            get
            {
                var str = (string)GetValue(CultureProperty);
                return !string.IsNullOrEmpty(str) ? str : CultureInfo.CurrentUICulture.Name;
            }
            set
            {
                var flag = true;
                try
                {
                    var cultureInfo = new CultureInfo(value);
                }
                catch (ArgumentException)
                {
                    flag = Regex.IsMatch(value, "^[A-Za-z]{2}-[A-Za-z]{2}$");
                }
                if (!flag)
                    return;
                SetValue(CultureProperty, value);
            }
        }

        private void UpdateCulture() => MapConfiguration.GetSection("v1", "Services", Culture, null, new MapConfigurationCallback(AsynchronousConfigurationLoaded), true);

        private void AsynchronousConfigurationLoaded(MapConfigurationSection config, object userState)
        {
            if (Mode is null)
                return;
            Mode.Culture = Culture;
            Mode.SessionId = Mode.SessionId;
            UpdateView();
        }

        public UIElementCollection Children => _MapUserLayerContainer.Children;

        [TypeConverter(typeof(MapModeConverter))]
        public MapMode Mode
        {
            get => (MapMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        private void UpdateMapMode()
        {
            if (Mode is null)
                throw new InvalidOperationException(ExceptionStrings.MapCore_UpdateMapMode_NonNull);
            if (_ModeSwitchAnationDriver.IsAnimating)
                _PendingMapMode = Mode;
            else
            {
                foreach (var mapMode in _MapModes)
                    mapMode.ChooseLevelOfDetailSettings = TilePyramidRenderable.ChooseLevelOfDetailSettingsDownloadNothing;
                if (_MapModes.Count > 1)
                {
                    _MapModes[1].Detach();
                    _MapModeContainer.Children.Remove(_MapModes[1]);
                    _MapModes.RemoveAt(1);
                }
                _CurrentMapMode = Mode;
                _CurrentMapMode.TileWrap = TileWrap.Horizontal;
                _CurrentMapMode.Culture = Culture;
                if (CredentialsProvider is ApplicationIdCredentialsProvider credentialsProvider)
                    _CurrentMapMode.SessionId = credentialsProvider.SessionId;
                _MapModes.Add(_CurrentMapMode);
                _MapModeContainer.Children.Insert(0, _CurrentMapMode);
                if (_MapModes.Count > 1)
                {
                    _CurrentMapMode.Opacity = 0;
                    _CurrentMapMode.Rendered += new EventHandler(_CurrentMapMode_Rendered);
                    _CurrentMapModeTransitionTimeout.Enabled = true;
                    _CurrentMapModeTransitionTimeout.Start();
                }
                _PendingMapMode = null;
                ModeChanged?.Invoke(this, new MapEventArgs());
            }
            UpdateView();
        }

        public Duration ModeCrossFadeDuration { get; set; }

        public AnimationLevel AnimationLevel { get; set; }

        public void SetView(Location center, double zoomLevel)
        {
            ZoomAndRotateOrigin = new Point?();
            if (ZoomAndRotateOrigin.HasValue)
                throw new InvalidOperationException("Cannot set map center if ZoomAndRotateOrigin is set.");
            SetView(center, zoomLevel, TargetHeading);
        }

        public void SetView(Location center, double zoomLevel, double heading)
        {
            ZoomAndRotateOrigin = new Point?();
            if (ZoomAndRotateOrigin.HasValue)
                throw new InvalidOperationException("Cannot set map center if ZoomAndRotateOrigin is set.");
            SetViewInternal(ConvertLocationToNormalizedMercator(center), zoomLevel, heading);
        }

        public void SetView(double zoomLevel, double heading) => SetViewInternal(TargetNormalizedMercatorCenter, zoomLevel, heading);

        public void SetView(LocationRect boundingRectangle)
        {
            var viewportSize = ViewportSize;
            if (viewportSize.Width <= 0 || double.IsInfinity(viewportSize.Width) || double.IsNaN(viewportSize.Width) || viewportSize.Height <= 0 || double.IsInfinity(viewportSize.Height) || double.IsNaN(viewportSize.Height))
                throw new InvalidOperationException("The actual size of the control must be positive and finite in order to set the view using a bounding rectangle.");
            ZoomAndRotateOrigin = new Point?();
            SetViewInternal(ConvertLocationToNormalizedMercator(MapMath.GetMercatorCenter(boundingRectangle)), MapMath.LocationRectToMercatorZoomLevel(viewportSize, boundingRectangle), 0);
        }

        public void SetView(IEnumerable<Location> locations, Thickness margin, double heading)
        {
            if (margin.Left < 0 || margin.Right < 0 || (margin.Top < 0 || margin.Bottom < 0))
                throw new ArgumentOutOfRangeException(nameof(margin));
            var viewportSize = ViewportSize;
            if (viewportSize.Width <= 0 || double.IsInfinity(viewportSize.Width) || double.IsNaN(viewportSize.Width) || viewportSize.Height <= 0 || double.IsInfinity(viewportSize.Height) || double.IsNaN(viewportSize.Height))
                throw new InvalidOperationException("The actual size of the control must be positive and finite in order to set the view using a bounding rectangle.");
            ZoomAndRotateOrigin = new Point?();
            MapMath.CalculateViewFromLocations(locations, viewportSize, TargetHeading, margin, out var centerNormalizedMercator, out var zoomLevel);
            SetViewInternal(centerNormalizedMercator, zoomLevel, heading);
        }

        public void SetView(IEnumerable<Location> locations, Thickness margin, double heading, double maxZoomLevel)
        {
            if (margin.Left < 0 || margin.Right < 0 || (margin.Top < 0 || margin.Bottom < 0))
                throw new ArgumentOutOfRangeException(nameof(margin));
            var viewportSize = ViewportSize;
            if (viewportSize.Width <= 0 || double.IsInfinity(viewportSize.Width) || (double.IsNaN(viewportSize.Width) || viewportSize.Height <= 0) || (double.IsInfinity(viewportSize.Height) || double.IsNaN(viewportSize.Height)))
                throw new InvalidOperationException("The actual size of the control must be positive and finite in order to set the view using a bounding rectangle.");
            ZoomAndRotateOrigin = new Point?();
            MapMath.CalculateViewFromLocations(locations, viewportSize, TargetHeading, margin, out var centerNormalizedMercator, out var zoomLevel);
            zoomLevel = Math.Min(zoomLevel, maxZoomLevel);
            SetViewInternal(centerNormalizedMercator, zoomLevel, heading);
        }

        internal void SetView(Point centerNormalizedMercator, double zoomLevel, double heading)
        {
            ZoomAndRotateOrigin = new Point?();
            SetViewInternal(centerNormalizedMercator, zoomLevel, heading);
        }

        public Location TargetCenter
        {
            get
            {
                var point = ApplyWorldWrap(new Point(_CenterNormalizedMercatorSpringX.TargetValue, _CenterNormalizedMercatorSpringY.TargetValue));
                return MapMath.NormalizeLocation(MercatorCube.Instance.ToLocation(new MapExtras.Point3D(point.X, point.Y, 0)));
            }
        }

        private Point TargetNormalizedMercatorCenter => new Point(_CenterNormalizedMercatorSpringX.TargetValue, _CenterNormalizedMercatorSpringY.TargetValue);

        public double TargetZoomLevel => _ZoomLevelSpring.TargetValue;

        public double TargetHeading => _HeadingSpring.TargetValue;

        public Size ViewportSize => new Size(ActualWidth, ActualHeight);

        public LocationRect BoundingRectangle
        {
            get
            {
                var viewportSize = ViewportSize;
                return new LocationRect(new Location[4]
                {
                    ViewportPointToLocation(new Point(0, 0)),
                    ViewportPointToLocation(new Point(0, viewportSize.Height)),
                    ViewportPointToLocation(new Point(viewportSize.Width, viewportSize.Height)),
                    ViewportPointToLocation(new Point(viewportSize.Width, 0))
                });
            }
        }

        public bool TryViewportPointToLocation(Point viewportPoint, out Location location)
        {
            location = ViewportPointToLocation(viewportPoint);
            return true;
        }

        public Location ViewportPointToLocation(Point viewportPoint) => ConvertNormalizedMercatorToLocation(TransformViewportToNormalizedMercator_Current(viewportPoint));

        public bool TryLocationToViewportPoint(Location location, out Point viewportPoint)
        {
            var identity1 = Matrix3D.Identity;
            var identity2 = Matrix3D.Identity;
            CalculateNormalizedMercatorToViewportMapping(ViewportSize, new Point(_CenterNormalizedMercatorSpringX.CurrentValue, _CenterNormalizedMercatorSpringY.CurrentValue), _HeadingSpring.CurrentValue, _ZoomLevelSpring.CurrentValue, true, ref identity1, ref identity2);
            return MapMath.TryLocationToViewportPoint(ref identity1, location, out viewportPoint);
        }

        public Point LocationToViewportPoint(Location location)
        {
            TryLocationToViewportPoint(location, out var viewportPoint);
            return viewportPoint;
        }

        public event EventHandler<MapEventArgs> ViewChangeOnFrame;

        public event EventHandler<MapEventArgs> TargetViewChanged;

        public event EventHandler<MapEventArgs> ViewChangeStart;

        public event EventHandler<MapEventArgs> ViewChangeEnd;

        public event EventHandler<MapEventArgs> ModeChanged;

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            if (_Disposed)
                throw new InvalidOperationException("The control cannot be added to the visual tree if it has been diposed.");
            base.OnVisualParentChanged(oldParent);
        }

        internal bool ViewBeingSetByUserInput { get; set; }

        internal Point? ZoomAndRotateOrigin
        {
            get => _ZoomAndRotateOrigin;
            set => _ZoomAndRotateOrigin = value;
        }

        internal Grid MapForegroundContainer { get; }

        internal Point TransformViewportToNormalizedMercator_Current(Point viewportPoint)
        {
            var identity1 = Matrix3D.Identity;
            var identity2 = Matrix3D.Identity;
            CalculateNormalizedMercatorToViewportMapping(ViewportSize, new Point(_CenterNormalizedMercatorSpringX.CurrentValue, _CenterNormalizedMercatorSpringY.CurrentValue), _HeadingSpring.CurrentValue, _ZoomLevelSpring.CurrentValue, false, ref identity1, ref identity2);
            return Transform(identity2, viewportPoint);
        }

        internal Point TransformViewportToNormalizedMercator_Target(Point viewportPoint)
        {
            var identity1 = Matrix3D.Identity;
            var identity2 = Matrix3D.Identity;
            CalculateNormalizedMercatorToViewportMapping(ViewportSize, new Point(_CenterNormalizedMercatorSpringX.TargetValue, _CenterNormalizedMercatorSpringY.TargetValue), _HeadingSpring.TargetValue, _ZoomLevelSpring.TargetValue, false, ref identity1, ref identity2);
            return Transform(identity2, viewportPoint);
        }

        private bool ViewIsAnimating
        {
            get => _ViewIsAnimating;
            set
            {
                if (_ViewIsAnimating == value)
                    return;
                if (!_ViewIsAnimating && value)
                    CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
                else
                    CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
                _ViewIsAnimating = value;
            }
        }

        private void AnimateViewUsingZoomAndPan(double zoomLevel, Point centerNormalizedMercator)
        {
            _ZoomAndPan_FromZoomLevel = _ZoomLevelSpring.CurrentValue;
            _ZoomAndPan_FromRect = new Rect(TransformViewportToNormalizedMercator_Current(new Point(0, 0)), TransformViewportToNormalizedMercator_Current(new Point(ActualWidth, ActualHeight)));
            var num = Math.Pow(2, _ZoomAndPan_FromZoomLevel - zoomLevel);
            var toRect = new Rect(0, 0, _ZoomAndPan_FromRect.Width * num, _ZoomAndPan_FromRect.Height * num);
            toRect.X = centerNormalizedMercator.X - toRect.Width / 2;
            toRect.Y = centerNormalizedMercator.Y - toRect.Height / 2;
            toRect.X += Math.Floor(_CenterNormalizedMercatorSpringX.CurrentValue);
            toRect.Y += Math.Floor(_CenterNormalizedMercatorSpringY.CurrentValue);
            _ZoomAndPanAnimator.Begin(_ZoomAndPan_FromRect, toRect, out var duration);
            _ZoomAndPanAnimationDriver.Start(new Duration(TimeSpan.FromSeconds(duration)));
            _CurrentMapMode.ChooseLevelOfDetailSettings = TilePyramidRenderable.ChooseLevelOfDetailSettingsDownloadInMotion;
        }

        private void _ZoomAndPanAnimationDriver_AnimationProgressChanged(object sender, EventArgs e)
        {
            _ZoomAndPanAnimator.Tick(_ZoomAndPanAnimationDriver.AnimationProgress, out var width, out var center);
            _ZoomLevelSpring.SnapToValue(Math.Log(_ZoomAndPan_FromRect.Width / width) / Math.Log(2) + _ZoomAndPan_FromZoomLevel);
            _CenterNormalizedMercatorSpringX.SnapToValue(center.X);
            _CenterNormalizedMercatorSpringY.SnapToValue(center.Y);
            UpdateView();
            if (ViewChangeOnFrame is null)
                return;
            ViewChangeOnFrame(this, new MapEventArgs());
        }

        private void _ZoomAndPanAnimationDriver_AnimationStopped(object sender, EventArgs e) => _CurrentMapMode.ChooseLevelOfDetailSettings = TilePyramidRenderable.ChooseLevelOfDetailSettingsDownloadNormal;

        private void _ZoomAndPanAnimationDriver_AnimationCompleted(object sender, EventArgs e)
        {
            ViewIsAnimating = false;
            if (ViewChangeEnd is object && !_UserInputTimeout.IsEnabled)
                ViewChangeEnd(this, new MapEventArgs());
            _CurrentMapMode.ChooseLevelOfDetailSettings = TilePyramidRenderable.ChooseLevelOfDetailSettingsDownloadNormal;
        }

        private void CalculateNormalizedMercatorToViewportMapping(Size viewportSize, Point centerNormalizedMercator, double heading, double zoomLevel, bool applyWorldWrap, ref Matrix3D normalizedMercatorToViewport, ref Matrix3D viewportToNormalizedMercator)
        {
            var point = applyWorldWrap ? ApplyWorldWrap(centerNormalizedMercator) : centerNormalizedMercator;
            _NormalizedMercatorToViewport_TranslatePre.OffsetX = -point.X;
            _NormalizedMercatorToViewport_TranslatePre.OffsetY = -point.Y;
            var num = 256 * Math.Pow(2, zoomLevel);
            _NormalizedMercatorToViewport_Scale.M11 = num;
            _NormalizedMercatorToViewport_Scale.M22 = num;
            _NormalizedMercatorToViewport_Rotate = VectorMath.RotationMatrix3DZ(Math.PI * heading / 180);
            _NormalizedMercatorToViewport_TranslatePost.OffsetX = viewportSize.Width / 2;
            _NormalizedMercatorToViewport_TranslatePost.OffsetY = viewportSize.Height / 2;
            normalizedMercatorToViewport = _NormalizedMercatorToViewport_TranslatePre * _NormalizedMercatorToViewport_Scale * _NormalizedMercatorToViewport_Rotate * _NormalizedMercatorToViewport_TranslatePost;
            viewportToNormalizedMercator = normalizedMercatorToViewport;
            viewportToNormalizedMercator.Invert();
        }

        private static Point ApplyWorldWrap(Point normalizedMercator) => new Point(normalizedMercator.X - Math.Floor(normalizedMercator.X), normalizedMercator.Y);

        private void UpdateView()
        {
            var viewportSize = ViewportSize;
            var identity1 = Matrix3D.Identity;
            var identity2 = Matrix3D.Identity;
            if (_CenterNormalizedMercatorSpringY.CurrentValue < 0)
                _CenterNormalizedMercatorSpringY.SnapToValue(0);
            else if (_CenterNormalizedMercatorSpringY.CurrentValue > 1)
                _CenterNormalizedMercatorSpringY.SnapToValue(1);
            CalculateNormalizedMercatorToViewportMapping(viewportSize, new Point(_CenterNormalizedMercatorSpringX.CurrentValue, _CenterNormalizedMercatorSpringY.CurrentValue), _HeadingSpring.CurrentValue, _ZoomLevelSpring.CurrentValue, true, ref identity1, ref identity2);
            foreach (var mapMode in _MapModes)
            {
                mapMode.CurrentMapCopyInstance = new Point(Math.Floor(_CenterNormalizedMercatorSpringX.CurrentValue), Math.Floor(_CenterNormalizedMercatorSpringY.CurrentValue));
                ((IProjectable)mapMode).SetView(viewportSize, identity1, identity2);
            }
            ((IProjectable)_MapUserLayerContainer).SetView(viewportSize, identity1, identity2);
            foreach (var child in Children)
                (child as IProjectable)?.SetView(viewportSize, identity1, identity2);
            UpdateViewDependencyProperties();
        }

        private void UpdateViewDependencyProperties()
        {
            _ViewUpdatingInternally = true;
            ZoomLevel = _ZoomLevelSpring.CurrentValue;
            Heading = _HeadingSpring.CurrentValue;
            Center = ConvertNormalizedMercatorToLocation(new Point(_CenterNormalizedMercatorSpringX.CurrentValue, _CenterNormalizedMercatorSpringY.CurrentValue));
            _ViewUpdatingInternally = false;
        }

        private void _MapContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((RectangleGeometry)_MapContainer.Clip).Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
            UpdateView();
        }

        private static Location ConvertNormalizedMercatorToLocation(Point normalizedMercatorPoint)
        {
            var point = ApplyWorldWrap(normalizedMercatorPoint);
            return MapMath.NormalizeLocation(MercatorCube.Instance.ToLocation(new MapExtras.Point3D(point.X, point.Y, 0)));
        }

        private static Point ConvertLocationToNormalizedMercator(Location location) => MercatorCube.Instance.FromLocation(location).ToPoint();

        private void MapCore_Loaded(object sender, RoutedEventArgs e)
        {
            if (_MapModes.Count != 0)
                return;
            Mode = new RoadMode
            {
                SessionId = CredentialsProvider.SessionId
            };
        }

        private void SetViewInternal(Point centerNormalizedMercator, double zoomLevel, double heading)
        {
            zoomLevel = Math.Min(21, Math.Max(0.75, zoomLevel));
            if (!ViewBeingSetByUserInput)
                ZoomAndRotateOrigin = new Point?();
            var viewIsAnimating = ViewIsAnimating;
            ViewIsAnimating = false;
            var flag = _CenterNormalizedMercatorSpringX.TargetValue != centerNormalizedMercator.X || _CenterNormalizedMercatorSpringY.TargetValue != centerNormalizedMercator.Y || _ZoomLevelSpring.TargetValue != zoomLevel || _HeadingSpring.TargetValue != heading;
            var setViewImmediately = AnimationLevel == AnimationLevel.None || AnimationLevel == AnimationLevel.UserInput && !ViewBeingSetByUserInput;
            if (flag)
            {
                if (_ZoomAndPanAnimationDriver.IsAnimating)
                    _ZoomAndPanAnimationDriver.Stop();
                if (!setViewImmediately && !ViewBeingSetByUserInput)
                {
                    _ZoomLevelSpring.SnapToValue(_ZoomLevelSpring.CurrentValue);
                    _CenterNormalizedMercatorSpringX.SnapToValue(_CenterNormalizedMercatorSpringX.CurrentValue);
                    _CenterNormalizedMercatorSpringY.SnapToValue(_CenterNormalizedMercatorSpringY.CurrentValue);
                    _HeadingSpring.TargetValue = heading;
                    AnimateViewUsingZoomAndPan(zoomLevel, centerNormalizedMercator);
                    ViewIsAnimating = true;
                }
                else
                {
                    AnimateViewUsingSprings(centerNormalizedMercator, zoomLevel, heading, setViewImmediately);
                    ViewIsAnimating = !setViewImmediately;
                }
            }
            else if (viewIsAnimating && setViewImmediately)
                AnimateViewUsingSprings(centerNormalizedMercator, zoomLevel, heading, true);
            if (!flag)
                return;
            if (TargetViewChanged is object)
                TargetViewChanged(this, new MapEventArgs());
            if (!viewIsAnimating && ViewChangeStart is object)
            {
                if (ViewBeingSetByUserInput)
                {
                    if (_UserInputTimeout.IsEnabled)
                    {
                        _UserInputTimeout.Stop();
                    }
                    else
                    {
                        ViewChangeStart(this, new MapEventArgs());
                        _UserInputTimeout.Tick += new EventHandler(UserInputCompleted);
                    }
                    _UserInputTimeout.Start();
                }
                else
                    ViewChangeStart(this, new MapEventArgs());
            }
            if (ViewIsAnimating)
                return;
            if (ViewChangeOnFrame is object)
                ViewChangeOnFrame(this, new MapEventArgs());
            if (ViewChangeEnd is null || _UserInputTimeout.IsEnabled)
                return;
            ViewChangeEnd(this, new MapEventArgs());
        }

        private void UserInputCompleted(object sender, EventArgs e)
        {
            if (!ViewIsAnimating && ViewChangeEnd is object)
                ViewChangeEnd(this, new MapEventArgs());
            _UserInputTimeout.Tick -= new EventHandler(UserInputCompleted);
            _UserInputTimeout.Stop();
        }

        private void AnimateViewUsingSprings(Point centerNormalizedMercator, double zoomLevel, double heading, bool setViewImmediately)
        {
            var nullable = _ZoomAndRotateOrigin.HasValue ? new Point?(TransformViewportToNormalizedMercator_Current(_ZoomAndRotateOrigin.Value)) : new Point?();
            var currentValue1 = _HeadingSpring.CurrentValue;
            var currentValue2 = _ZoomLevelSpring.CurrentValue;
            if (setViewImmediately)
            {
                _ZoomLevelSpring.SnapToValue(zoomLevel);
                _HeadingSpring.SnapToValue(heading);
            }
            else
            {
                _HeadingSpring.TargetValue = heading;
                _ZoomLevelSpring.TargetValue = zoomLevel;
            }
            if ((currentValue2 != _ZoomLevelSpring.CurrentValue || currentValue1 != _HeadingSpring.CurrentValue) && nullable.HasValue)
                AdjustCenterAfterTransform(nullable.Value);
            else if (setViewImmediately)
            {
                _CenterNormalizedMercatorSpringX.SnapToValue(centerNormalizedMercator.X);
                _CenterNormalizedMercatorSpringY.SnapToValue(centerNormalizedMercator.Y);
            }
            else
            {
                _CenterNormalizedMercatorSpringX.TargetValue = centerNormalizedMercator.X;
                _CenterNormalizedMercatorSpringY.TargetValue = centerNormalizedMercator.Y;
            }
            UpdateView();
        }

        private static Point Transform(Matrix3D matrix, Point point)
        {
            var point3D = matrix.Transform(new System.Windows.Media.Media3D.Point3D(point.X, point.Y, 0));
            return new Point(point3D.X, point3D.Y);
        }

        private void AdjustCenterAfterTransform(Point zoomAndRotateOriginNormalizedMercatorPreUpdate)
        {
            var normalizedMercatorCurrent = TransformViewportToNormalizedMercator_Current(_ZoomAndRotateOrigin.Value);
            _CenterNormalizedMercatorSpringX.SnapToValue(_CenterNormalizedMercatorSpringX.CurrentValue + zoomAndRotateOriginNormalizedMercatorPreUpdate.X - normalizedMercatorCurrent.X);
            _CenterNormalizedMercatorSpringY.SnapToValue(_CenterNormalizedMercatorSpringY.CurrentValue + zoomAndRotateOriginNormalizedMercatorPreUpdate.Y - normalizedMercatorCurrent.Y);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            var flag1 = false;
            var nullable = _ZoomAndRotateOrigin.HasValue ? new Point?(TransformViewportToNormalizedMercator_Current(_ZoomAndRotateOrigin.Value)) : new Point?();
            bool flag2;
            if (_ZoomLevelSpring.Update() || _HeadingSpring.Update())
            {
                if (nullable.HasValue)
                    AdjustCenterAfterTransform(nullable.Value);
                flag2 = true;
            }
            else
                flag2 = flag1 | _CenterNormalizedMercatorSpringX.Update() | _CenterNormalizedMercatorSpringY.Update();
            if (flag2)
            {
                UpdateView();
                if (ViewChangeOnFrame is null)
                    return;
                ViewChangeOnFrame(this, new MapEventArgs());
            }
            else
            {
                if (!ViewIsAnimating || _ZoomAndPanAnimationDriver.IsAnimating)
                    return;
                ViewIsAnimating = false;
                if (ViewChangeEnd is null || _UserInputTimeout.IsEnabled)
                    return;
                ViewChangeEnd(this, new MapEventArgs());
            }
        }

        internal void ArrestZoomAndRotation()
        {
            _ZoomLevelSpring.SnapToValue(_ZoomLevelSpring.CurrentValue);
            _HeadingSpring.SnapToValue(_HeadingSpring.CurrentValue);
            _ZoomAndRotateOrigin = new Point?();
        }

        private void _CurrentMapModeTransitionTimeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
           {
               if (_MapModes.Count <= 1 || _ModeSwitchAnationDriver.IsAnimating)
                   return;
               _CurrentMapMode.Opacity = 1;
               _ModeSwitchAnationDriver.Start(ModeCrossFadeDuration);
           }));
        }

        private void _CurrentMapMode_Rendered(object sender, EventArgs e)
        {
            if (_MapModes.Count <= 1 || _ModeSwitchAnationDriver.IsAnimating || !_CurrentMapMode.HasSomeTiles)
                return;
            _CurrentMapMode.Opacity = 1;
            _ModeSwitchAnationDriver.Start(ModeCrossFadeDuration);
            _CurrentMapModeTransitionTimeout.Enabled = false;
        }

        private void _ModeSwitchAnationDriver_AnimationCompleted(object sender, EventArgs e)
        {
            _MapModes[0].Detach();
            _MapModeContainer.Children.Remove(_MapModes[0]);
            _MapModes.RemoveAt(0);
            _CurrentMapMode.Rendered -= new EventHandler(_CurrentMapMode_Rendered);
            if (_PendingMapMode is null)
                return;
            Dispatcher.BeginInvoke(new Action(() => Mode = _PendingMapMode));
        }

        private void _ModeSwitchAnationDriver_AnimationProgressChanged(object sender, EventArgs e) => _MapModes[0].Opacity = 1 - _ModeSwitchAnationDriver.AnimationProgress;
    }
}
