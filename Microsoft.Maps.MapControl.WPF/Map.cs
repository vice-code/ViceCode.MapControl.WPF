using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.Manipulations;
using System.Windows.Threading;
using Microsoft.Maps.MapControl.WPF.Core;
using Microsoft.Maps.MapControl.WPF.Overlays;

namespace Microsoft.Maps.MapControl.WPF
{
    public class Map : MapCore
    {
        private Point? _LeftButtonDownNormalizedMercatorPoint = new Point?();
        private Point _LeftButtonDownViewportPoint = new Point();
        private readonly StoredManipulationDelta2D storedManipulation = new StoredManipulationDelta2D();
        private readonly int _doubleTapDelay = 500;
        private readonly int _doubleTapThreshold = 20;
        private Point? _touchTapPointPrevious = new Point?();
        private DateTime? _touchTapTimePrevious = new DateTime?();
        private Point? _touchTapPointCurrent = new Point?();
        private DateTime? _touchTapTimeCurrent = new DateTime?();
        private DateTime _lastMouseMoveTime = DateTime.Now;
        private Manipulations2D _SupportedManipulations = Manipulations2D.All;
        private static readonly string version = GetVersion();
        private WeakEventListener<Map, object, PropertyChangedEventArgs> _weakMapCredentials;
        private bool firstFrameDone;
        private bool getConfigurationDone;
        private readonly bool settingDefaultCredentials;
        private bool startedSession;
        private LoadingErrorMessage loadingErrorMessage;
        private Exception LoadingException;
        private string logServiceUriFormat;
        private readonly MapForeground _MapForeground;
        private readonly ManipulationProcessor2D _ManipulationProcessor;
        private readonly InertiaProcessor2D _InertiaProcessor;
        private readonly DispatcherTimer _InertiaTimer;
        private long _lastTouchTick;
        private int _touchCount;
        private Point _velocity;
        private Point _previousMousePoint;
        private Timer _logTimer;

        private event EventHandler<LoadingErrorEventArgs> loadingErrorEvent;

        private static string GetVersion()
        {
            var strArray = Assembly.GetExecutingAssembly().FullName.Split(',');
            return strArray.Length > 1 ? strArray[1].Replace("Version=", string.Empty, StringComparison.Ordinal).Trim() : string.Empty;
        }

        static Map() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Map), new FrameworkPropertyMetadata(typeof(Map)));

        public Map()
        {
            settingDefaultCredentials = true;
            CredentialsProvider = new ApplicationIdCredentialsProvider();
            settingDefaultCredentials = false;
            MapConfiguration.GetSection("v1", "Services", Culture, null, new MapConfigurationCallback(AsynchronousConfigurationLoaded), true);
            _MapForeground = new MapForeground(this);
            MapForegroundContainer.Children.Add(_MapForeground);
            _ManipulationProcessor = new ManipulationProcessor2D(_SupportedManipulations);
            _ManipulationProcessor.Started += new EventHandler<Manipulation2DStartedEventArgs>(ManipulationProcessor_Started);
            _ManipulationProcessor.Delta += new EventHandler<Manipulation2DDeltaEventArgs>(ManipulationProcessor_Delta);
            _ManipulationProcessor.Completed += new EventHandler<Manipulation2DCompletedEventArgs>(ManipulationProcessor_Completed);
            _InertiaProcessor = new InertiaProcessor2D();
            _InertiaProcessor.TranslationBehavior.DesiredDeceleration = 0.00384f;
            _InertiaProcessor.ExpansionBehavior.DesiredDeceleration = 9.6E-05f;
            _InertiaProcessor.RotationBehavior.DesiredDeceleration = 0.00072f;
            _InertiaProcessor.Delta += new EventHandler<Manipulation2DDeltaEventArgs>(ManipulationProcessor_Delta);
            _InertiaProcessor.Completed += (sender, e) => StopInertia();
            _InertiaTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            _InertiaTimer.Tick += (sender, e) => _InertiaProcessor.Process(DateTime.UtcNow.Ticks);
            IsTabStop = true;
        }

        ~Map()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_logTimer is object)
            {
                _logTimer.Dispose();
                _logTimer = null;
            }
            if (_weakMapCredentials is null)
                return;
            _weakMapCredentials.Detach();
            _weakMapCredentials = null;
        }

        public static bool UseHttps { get; set; } = false;

        public static string UriScheme => !UseHttps ? Uri.UriSchemeHttp : Uri.UriSchemeHttps;

        public static int LoggingDelay { get; set; }

        public new event MouseButtonEventHandler MouseLeftButtonDown;

        public new event MouseButtonEventHandler MouseLeftButtonUp;

        public new event MouseButtonEventHandler MouseDoubleClick;

        public new event MouseEventHandler MouseMove;

        public new event MouseWheelEventHandler MouseWheel;

        public new event KeyEventHandler KeyDown;

        public new event EventHandler<TouchEventArgs> TouchUp;

        public new event EventHandler<TouchEventArgs> TouchDown;

        public new event EventHandler<TouchEventArgs> TouchMove;

        public Style MapForegroundStyle
        {
            get => _MapForeground.Style;
            set
            {
                _MapForeground.Style = value;
                _MapForeground.Visibility = value is null ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public event EventHandler<LoadingErrorEventArgs> LoadingError
        {
            add
            {
                if (LoadingException is object && value is object)
                    value(this, new LoadingErrorEventArgs(LoadingException));
                loadingErrorEvent += value;
            }
            remove
            {
                loadingErrorEvent -= value;
            }
        }

        public Manipulations2D SupportedManipulations
        {
            get => _SupportedManipulations;
            set
            {
                StopInertia();
                _SupportedManipulations = value;
                _ManipulationProcessor.SupportedManipulations = _SupportedManipulations;
            }
        }

        public bool UseInertia { get; set; }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var keyDown = KeyDown;
            if (keyDown is object)
                keyDown(this, e);
            if (e.Handled)
                return;
            if (e.Key == Key.Up || e.Key == Key.Down || (e.Key == Key.Left || e.Key == Key.Right))
            {
                var point = new Point();
                point.X += e.Key == Key.Right ? 100 : 0;
                point.X += e.Key == Key.Left ? -100 : 0;
                point.Y += e.Key == Key.Up ? -100 : 0;
                point.Y += e.Key == Key.Down ? 100 : 0;
                var normalizedMercatorTarget = TransformViewportToNormalizedMercator_Target(new Point(ActualWidth / 2 + point.X, ActualHeight / 2 + point.Y));
                ArrestZoomAndRotation();
                ViewBeingSetByUserInput = true;
                SetView(normalizedMercatorTarget, TargetZoomLevel, TargetHeading);
                ViewBeingSetByUserInput = false;
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                ZoomAndRotateOrigin = new Point?(new Point(ActualWidth / 2, ActualHeight / 2));
                ViewBeingSetByUserInput = true;
                SetView(TargetZoomLevel + 0.5, TargetHeading);
                ViewBeingSetByUserInput = false;
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                ZoomAndRotateOrigin = new Point?(new Point(ActualWidth / 2, ActualHeight / 2));
                ViewBeingSetByUserInput = true;
                SetView(TargetZoomLevel - 0.5, TargetHeading);
                ViewBeingSetByUserInput = false;
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice is object && e.StylusDevice.TabletDevice is object && e.StylusDevice.TabletDevice.Type == TabletDeviceType.Touch)
                return;
            var mouseLeftButtonDown = MouseLeftButtonDown;
            if (mouseLeftButtonDown is object)
                mouseLeftButtonDown(this, e);
            if (e.Handled)
                return;
            StopInertia();
            if (CaptureMouse())
            {
                _LeftButtonDownViewportPoint = e.GetPosition(this);
                _LeftButtonDownNormalizedMercatorPoint = new Point?(TransformViewportToNormalizedMercator_Current(_LeftButtonDownViewportPoint));
                _previousMousePoint = _LeftButtonDownViewportPoint;
                _lastMouseMoveTime = DateTime.Now;
            }
            else
                _LeftButtonDownNormalizedMercatorPoint = new Point?();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            var mouseDoubleClick = MouseDoubleClick;
            if (mouseDoubleClick is object)
                mouseDoubleClick(this, e);
            if (e.Handled)
                return;
            _LeftButtonDownNormalizedMercatorPoint = new Point?();
            ZoomAboutViewportPoint(1, e.GetPosition(this));
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            var mouseWheel = MouseWheel;
            if (mouseWheel is object)
                mouseWheel(this, e);
            if (e.Handled)
                return;
            ZoomAboutViewportPoint(e.Delta / 100, e.GetPosition(this));
            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.StylusDevice is object && e.StylusDevice.TabletDevice is object && e.StylusDevice.TabletDevice.Type == TabletDeviceType.Touch)
                return;
            var mouseMove = MouseMove;
            if (mouseMove is object)
                mouseMove(this, e);
            if (e.Handled)
                return;
            if (IsMouseCaptured)
            {
                var position = e.GetPosition(this);
                if (UseInertia)
                    UpdateVelocity(position);
                _previousMousePoint = position;
                if (_LeftButtonDownNormalizedMercatorPoint.HasValue && _LeftButtonDownViewportPoint != position)
                {
                    _LeftButtonDownViewportPoint = new Point(double.NaN, double.NaN);
                    var normalizedMercatorTarget1 = TransformViewportToNormalizedMercator_Target(position);
                    var normalizedMercatorTarget2 = TransformViewportToNormalizedMercator_Target(new Point(ActualWidth / 2, ActualHeight / 2));
                    var centerNormalizedMercator = new Point(_LeftButtonDownNormalizedMercatorPoint.Value.X - normalizedMercatorTarget1.X + normalizedMercatorTarget2.X, _LeftButtonDownNormalizedMercatorPoint.Value.Y - normalizedMercatorTarget1.Y + normalizedMercatorTarget2.Y);
                    var animationLevel = AnimationLevel;
                    AnimationLevel = AnimationLevel.None;
                    ArrestZoomAndRotation();
                    ViewBeingSetByUserInput = true;
                    SetView(centerNormalizedMercator, TargetZoomLevel, TargetHeading);
                    ViewBeingSetByUserInput = false;
                    ZoomAndRotateOrigin = new Point?(position);
                    AnimationLevel = animationLevel;
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.StylusDevice is object && e.StylusDevice.TabletDevice is object && e.StylusDevice.TabletDevice.Type == TabletDeviceType.Touch)
                return;
            var mouseLeftButtonUp = MouseLeftButtonUp;
            mouseLeftButtonUp?.Invoke(this, e);
            if (e.Handled)
                return;
            ReleaseMouseCapture();
            if (!IsMouseCaptured && !AreAnyTouchesCaptured)
                _LeftButtonDownNormalizedMercatorPoint = new Point?();
            if (UseInertia && !_InertiaTimer.IsEnabled)
            {
                var now = DateTime.Now;
                var milliseconds = (now - _lastMouseMoveTime).Milliseconds;
                _lastMouseMoveTime = now;
                if (milliseconds < 200)
                {
                    _InertiaProcessor.InitialOriginX = (float)_previousMousePoint.X;
                    _InertiaProcessor.InitialOriginY = (float)_previousMousePoint.Y;
                    _InertiaProcessor.TranslationBehavior.InitialVelocityX = (float)_velocity.X;
                    _InertiaProcessor.TranslationBehavior.InitialVelocityY = (float)_velocity.Y;
                    _InertiaProcessor.Process(DateTime.UtcNow.Ticks);
                    _InertiaTimer.Start();
                }
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            var touchDown = TouchDown;
            if (touchDown is object)
                touchDown(this, e);
            _lastTouchTick = 0L;
            if (e.Handled)
                return;
            e.Handled = true;
            if (e.TouchDevice.Capture(this))
            {
                _LeftButtonDownViewportPoint = e.GetTouchPoint(this).Position;
                _LeftButtonDownNormalizedMercatorPoint = new Point?(TransformViewportToNormalizedMercator_Current(_LeftButtonDownViewportPoint));
                ProcessManipulators();
            }
            else
                _LeftButtonDownNormalizedMercatorPoint = new Point?();
            base.OnTouchDown(e);
        }

        protected override void OnTouchMove(TouchEventArgs e)
        {
            var touchMove = TouchMove;
            touchMove?.Invoke(this, e);
            if (e.Handled)
                return;
            if (e.TouchDevice.Captured == this)
            {
                e.Handled = true;
                ProcessManipulators();
            }
            base.OnTouchMove(e);
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            var touchUp = TouchUp;
            touchUp?.Invoke(this, e);
            _lastTouchTick = 0L;
            if (e.Handled)
                return;
            e.Handled = true;
            e.TouchDevice.Capture(null);
            if (!IsMouseCaptured && !AreAnyTouchesCaptured)
                _LeftButtonDownNormalizedMercatorPoint = new Point?();
            ProcessManipulators();
            base.OnTouchUp(e);
        }

        private void UpdateVelocity(Point position)
        {
            var now = DateTime.Now;
            var milliseconds = (now - _lastMouseMoveTime).Milliseconds;
            _lastMouseMoveTime = now;
            if (milliseconds <= 0)
                return;
            _velocity.X = 0.3 * _velocity.X + 0.7 * (position.X - _previousMousePoint.X) / milliseconds;
            _velocity.Y = 0.3 * _velocity.Y + 0.7 * (position.Y - _previousMousePoint.Y) / milliseconds;
        }

        private void ManipulationProcessor_Started(object sender, Manipulation2DStartedEventArgs e)
        {
            StopInertia();
            _touchTapPointCurrent = new Point?(new Point(e.OriginX, e.OriginY));
            _touchTapTimeCurrent = new DateTime?(DateTime.Now);
        }

        private void ManipulationProcessor_Delta(object sender, Manipulation2DDeltaEventArgs e)
        {
            if (!UseInertia && _InertiaProcessor.IsRunning || storedManipulation.Accumulate(e))
                return;
            ProcessStoredManipulation();
        }

        private void ProcessStoredManipulation()
        {
            if (!storedManipulation.HasValuesStored())
                return;
            var heading = TargetHeading + storedManipulation.Rotation * (180 / Math.PI);
            var animationLevel = AnimationLevel;
            AnimationLevel = AnimationLevel.None;
            if (_touchCount > 1)
            {
                ZoomAndRotateOrigin = new Point?(new Point(storedManipulation.OriginX, storedManipulation.OriginY));
                if (storedManipulation.ScaleX != 1 || storedManipulation.ScaleY != 1)
                {
                    _cancelDoubleTap();
                    ZoomAboutViewportPoint(Math.Log(Math.Max(storedManipulation.ScaleX, storedManipulation.ScaleY)) / Math.Log(2), ZoomAndRotateOrigin.Value);
                }
                if (storedManipulation.Rotation != 0)
                {
                    _cancelDoubleTap();
                    ViewBeingSetByUserInput = true;
                    SetView(TargetZoomLevel, heading);
                    ViewBeingSetByUserInput = false;
                }
            }
            if (storedManipulation.TranslationX != 0 || storedManipulation.TranslationY != 0)
            {
                if (Math.Abs(storedManipulation.TranslationX) > (double)_doubleTapThreshold || Math.Abs(storedManipulation.TranslationY) > (double)_doubleTapThreshold)
                    _cancelDoubleTap();
                var normalizedMercatorTarget = TransformViewportToNormalizedMercator_Target(new Point(ActualWidth / 2 - storedManipulation.TranslationX, ActualHeight / 2 - storedManipulation.TranslationY));
                ZoomAndRotateOrigin = new Point?();
                ViewBeingSetByUserInput = true;
                SetView(normalizedMercatorTarget, TargetZoomLevel, heading);
                ViewBeingSetByUserInput = false;
            }
            AnimationLevel = animationLevel;
            storedManipulation.Reset();
        }

        private void ManipulationProcessor_Completed(object sender, Manipulation2DCompletedEventArgs e)
        {
            ProcessStoredManipulation();
            var flag = false;
            if (_touchTapPointCurrent.HasValue)
            {
                if (_touchTapPointPrevious.HasValue && (DateTime.Now - _touchTapTimePrevious.Value).Milliseconds < _doubleTapDelay && (Math.Abs(_touchTapPointCurrent.Value.X - _touchTapPointPrevious.Value.X) < _doubleTapThreshold && Math.Abs(_touchTapPointCurrent.Value.Y - _touchTapPointPrevious.Value.Y) < _doubleTapThreshold))
                {
                    ZoomAboutViewportPoint(1, _touchTapPointCurrent.Value);
                    _cancelDoubleTap();
                    flag = true;
                }
                else
                {
                    _touchTapPointPrevious = _touchTapPointCurrent;
                    _touchTapTimePrevious = _touchTapTimeCurrent;
                }
            }
            if (!UseInertia || flag || _InertiaTimer.IsEnabled)
                return;
            _InertiaProcessor.InitialOriginX = e.OriginX;
            _InertiaProcessor.InitialOriginY = e.OriginY;
            _InertiaProcessor.TranslationBehavior.InitialVelocityX = e.Velocities.LinearVelocityX;
            _InertiaProcessor.TranslationBehavior.InitialVelocityY = e.Velocities.LinearVelocityY;
            _InertiaProcessor.Process(DateTime.UtcNow.Ticks);
            _InertiaTimer.Start();
        }

        private void _cancelDoubleTap()
        {
            _touchTapPointCurrent = new Point?();
            _touchTapPointPrevious = new Point?();
        }

        private void ProcessManipulators()
        {
            var ticks = DateTime.UtcNow.Ticks;
            if (ticks - _lastTouchTick <= 50000L)
                return;
            var manipulator2DList = new List<Manipulator2D>();
            _touchCount = 0;
            foreach (var touchDevice in TouchesCaptured)
            {
                var position = touchDevice.GetTouchPoint(this).Position;
                manipulator2DList.Add(new Manipulator2D(touchDevice.Id, (float)position.X, (float)position.Y));
                ++_touchCount;
            }
            _ManipulationProcessor.ProcessManipulators(DateTime.UtcNow.Ticks, manipulator2DList);
            _lastTouchTick = ticks;
        }

        private void StopInertia()
        {
            if (_InertiaProcessor.IsRunning)
                _InertiaProcessor.Complete(DateTime.UtcNow.Ticks);
            _velocity = new Point();
            _InertiaTimer.Stop();
        }

        private void ZoomAboutViewportPoint(double zoomLevelIncrement, Point zoomTargetInViewport)
        {
            ZoomAndRotateOrigin = new Point?(zoomTargetInViewport);
            ViewBeingSetByUserInput = true;
            SetView(TargetZoomLevel + zoomLevelIncrement, TargetHeading);
            ViewBeingSetByUserInput = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (firstFrameDone)
                return;
            firstFrameDone = true;
            OnFirstFrame();
        }

        protected void OnFirstFrame() => Log(false);

        protected override void OnCredentialsProviderChanged(
      DependencyPropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.OldValue is INotifyPropertyChanged && _weakMapCredentials is object)
            {
                _weakMapCredentials.Detach();
                _weakMapCredentials = null;
            }
            if (eventArgs.NewValue is INotifyPropertyChanged newCredentials)
            {
                _weakMapCredentials = new WeakEventListener<Map, object, PropertyChangedEventArgs>(this)
                {
                    OnEventAction = (instance, source, leventArgs) => instance.Credentials_PropertyChanged(source, leventArgs),
                    OnDetachAction = weakEventListener => newCredentials.PropertyChanged -= new PropertyChangedEventHandler(weakEventListener.OnEvent)
                };
                newCredentials.PropertyChanged += new PropertyChangedEventHandler(_weakMapCredentials.OnEvent);
            }
            EndSession(eventArgs.OldValue as CredentialsProvider);
            Log(true);
        }

        private void AsynchronousConfigurationLoaded(MapConfigurationSection config, object userState)
        {
            getConfigurationDone = true;
            if (config is null)
            {
                ShowLoadingError(new ConfigurationNotLoadedException());
            }
            else
            {
                logServiceUriFormat = IsInDesignMode ? null : config["WPFLOGGING"];
                if (!string.IsNullOrEmpty(logServiceUriFormat))
                {
                    logServiceUriFormat = logServiceUriFormat.Replace("{UriScheme}", UriScheme, StringComparison.Ordinal);
                    logServiceUriFormat = logServiceUriFormat.Replace("{entryPoint}", "{0}", StringComparison.Ordinal);
                    logServiceUriFormat = logServiceUriFormat.Replace("{authKey}", "{1}", StringComparison.Ordinal);
                    logServiceUriFormat = logServiceUriFormat.Replace("{productBuildVersion}", "{2}", StringComparison.Ordinal);
                    logServiceUriFormat = logServiceUriFormat.Replace("{session}", "{3}", StringComparison.Ordinal);
                    logServiceUriFormat = logServiceUriFormat.Replace("{culture}", "{4}", StringComparison.Ordinal);
                }
            }
            Log(false);
        }

        private void Log(bool hasCredentialsProviderChanged)
        {
            if (settingDefaultCredentials)
                return;
            if (getConfigurationDone && string.IsNullOrEmpty(logServiceUriFormat))
            {
                EndSession(CredentialsProvider);
            }
            else
            {
                if (hasCredentialsProviderChanged)
                    StartSession(CredentialsProvider as ApplicationIdCredentialsProvider);
                if (startedSession)
                {
                    if (!hasCredentialsProviderChanged)
                        return;
                    Log("2");
                }
                else
                {
                    if (!getConfigurationDone || !firstFrameDone)
                        return;
                    startedSession = true;
                    Log("0");
                }
            }
        }

        private void Log(string entry)
        {
            if (CredentialsProvider is object)
            {
                if (CredentialsProvider is ApplicationIdCredentialsProvider credentialsProvider)
                    Log(entry, CredentialsProvider, new Credentials()
                    {
                        ApplicationId = credentialsProvider.ApplicationId
                    });
                else
                    CredentialsProvider.GetCredentials(credentials => Log(entry, CredentialsProvider, credentials));
            }
            else
                Dispatcher.BeginInvoke(new Action(() => OnCredentialsError()));
        }

        private void Log(string entry, CredentialsProvider credentialsProvider, Credentials credentials)
        {
            var logRequestString = string.Format(CultureInfo.InvariantCulture, logServiceUriFormat, entry, credentials.ToString(), version, Guid.Empty, Culture);
            if (_logTimer is object)
                _logTimer.Dispose();
            _logTimer = new Timer(LoggingDelay > 0 ? LoggingDelay : 1)
            {
                AutoReset = false
            };
            _logTimer.Elapsed += (sender, e) =>
            {
                _logTimer.Dispose();
                _logTimer = null;
                try
                {
                    using var webClient = new WebClient();
                    webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(LogResponse);
                    webClient.DownloadStringAsync(new Uri(logRequestString, UriKind.Absolute), credentialsProvider);
                }
                catch (WebException)
                {
                    EndSession(credentialsProvider);
                    OnCredentialsError();
                }
                catch (NotSupportedException)
                {
                    EndSession(credentialsProvider);
                }
            };
            _logTimer.Start();
        }

        private void LogResponse(object sender, DownloadStringCompletedEventArgs e)
        {
            var credentialsValid = e.Error is null;
            var appId = e.UserState as ApplicationIdCredentialsProvider;
            var serverSessionId = (string)null;
            if (appId is object && credentialsValid)
            {
                try
                {
                    using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(e.Result));
                    if (new DataContractJsonSerializer(typeof(Session)).ReadObject(memoryStream) is Session session)
                    {
                        if (!string.IsNullOrEmpty(session.SessionId))
                            serverSessionId = session.SessionId;
                    }
                }
                catch (SerializationException)
                {
                }
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (appId is object)
                {
                    if (credentialsValid && serverSessionId is object)
                        appId.SetSessionId(serverSessionId);
                    else
                        appId.EndSession();
                }
                if (credentialsValid)
                    OnCredentialsValid();
                else
                    OnCredentialsError();
            }));
        }

        private void OnCredentialsError() => ShowLoadingError(new CredentialsInvalidException());

        private void OnCredentialsValid()
        {
            if (!(LoadingException is CredentialsInvalidException) || loadingErrorMessage is null)
                return;
            Children.Remove(loadingErrorMessage);
            loadingErrorMessage = null;
        }

        private void Credentials_PropertyChanged(object sender, PropertyChangedEventArgs e) => Log(true);

        private void ShowLoadingError(Exception e)
        {
            if (LoadingException is null && e is object)
            {
                LoadingException = e;
                if (loadingErrorEvent is object)
                    loadingErrorEvent(this, new LoadingErrorEventArgs(LoadingException));
            }
            if (loadingErrorMessage is null)
            {
                loadingErrorMessage = new LoadingErrorMessage();
                Children.Add(loadingErrorMessage);
            }
            if (e is ConfigurationNotLoadedException)
            {
                loadingErrorMessage.SetConfigurationError(Culture);
            }
            else
            {
                if (!(e is CredentialsInvalidException))
                    return;
                loadingErrorMessage.SetCredentialsError(Culture);
            }
        }

        private static void StartSession(CredentialsProvider credentialsProvider) => (credentialsProvider as ApplicationIdCredentialsProvider)?.StartSession();

        private static void EndSession(CredentialsProvider credentialsProvider) => (credentialsProvider as ApplicationIdCredentialsProvider)?.EndSession();

        private static bool IsInDesignMode => (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

        private static class LogEntry
        {
            public const string StartSession = "0";
            public const string ChangeCredentials = "2";
        }

        private class StoredManipulationDelta2D
        {
            public int OriginX { get; set; }

            public int OriginY { get; set; }

            public float Rotation { get; set; }

            public float ScaleX { get; set; }

            public float ScaleY { get; set; }

            public float TranslationX { get; set; }

            public float TranslationY { get; set; }

            public StoredManipulationDelta2D() => Reset();

            public bool Accumulate(Manipulation2DDeltaEventArgs additional)
            {
                var flag = true;
                if (OriginY == int.MinValue)
                {
                    OriginX = (int)Math.Round(additional.OriginX);
                    OriginY = (int)Math.Round(additional.OriginY);
                }
                ScaleX *= additional.Delta.ScaleX;
                ScaleY *= additional.Delta.ScaleX;
                if (Math.Abs(OriginX - (int)Math.Round(additional.OriginX)) > 2 || Math.Abs(OriginY - (int)Math.Round(additional.OriginY)) > 2)
                    flag = false;
                Rotation += additional.Delta.Rotation;
                TranslationX += additional.Delta.TranslationX;
                TranslationY += additional.Delta.TranslationY;
                if (Math.Abs(Rotation) < 0.1 && Math.Abs(1f - ScaleX) < 0.1 && (Math.Abs(1f - ScaleY) < 0.1 && Math.Abs(TranslationX) < 2) && Math.Abs(TranslationY) < 2)
                    return false;
                return flag;
            }

            public void Reset()
            {
                Rotation = 0f;
                TranslationX = 0f;
                TranslationY = 0f;
                ScaleX = 1f;
                ScaleY = 1f;
                OriginX = int.MinValue;
                OriginY = int.MinValue;
            }

            public bool HasValuesStored() => OriginX != int.MinValue;
        }
    }
}
