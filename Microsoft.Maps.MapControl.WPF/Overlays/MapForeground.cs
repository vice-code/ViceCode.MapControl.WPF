using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF.Overlays
{
    public class MapForeground : Control
    {
        private static readonly Size MercatorModeLogicalAreaSizeInScreenSpaceAtLevel1 = new Size(512.0, 512.0);
        private const int CopyrightTimeout = 2000;
        private readonly Map _Map;
        private Collection<Logo> _Logos;
        private Collection<Copyright> _Copyrights;
        private Collection<Scale> _Scales;
        private Collection<Compass> _Compasses;
        private bool _TemplateApplied;
        private readonly DispatcherTimer _UpdateTimer;
        private readonly DispatcherTimer _copyrightUpdateTimer;

        static MapForeground() => DefaultStyleKeyProperty.OverrideMetadata(typeof(MapForeground), new FrameworkPropertyMetadata(typeof(MapForeground)));

        public MapForeground(Map map)
        {
            if (map is null)
                throw new ArgumentNullException(nameof(map));
            _Logos = new Collection<Logo>();
            _Copyrights = new Collection<Copyright>();
            _Scales = new Collection<Scale>();
            _Compasses = new Collection<Compass>();
            _Map = map;
            AttachProperty();
            _Map.ModeChanged += new EventHandler<MapEventArgs>(_Map_ModeChanged);
            _Map.ViewChangeStart += new EventHandler<MapEventArgs>(_Map_ViewChangeStart);
            _Map.ViewChangeEnd += new EventHandler<MapEventArgs>(_Map_ViewChangeEnd);
            _UpdateTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(500.0)
            };
            _UpdateTimer.Tick += new EventHandler(_UpdateTimer_Tick);
            _copyrightUpdateTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(2000.0)
            };
            _copyrightUpdateTimer.Tick += new EventHandler(CopyrightUpdateTimerTick);
        }

        internal void AttachProperty()
        {
            if (_Scales is null)
                return;
            foreach (var scale in _Scales)
            {
                var visibilityProperty = VisibilityProperty;
                var binding = new Binding()
                {
                    Mode = BindingMode.TwoWay,
                    Source = _Map,
                    Path = new PropertyPath("ScaleVisibility", new object[0])
                };
                scale.SetBinding(visibilityProperty, binding);
            }
        }

        public override void OnApplyTemplate()
        {
            _Logos = new Collection<Logo>(this.GetVisualOfType<Logo>().ToList());
            _Copyrights = new Collection<Copyright>(this.GetVisualOfType<Copyright>().ToList());
            _Scales = new Collection<Scale>(this.GetVisualOfType<Scale>().ToList());
            foreach (var scale in _Scales)
                scale.Culture = _Map.Culture;
            AttachProperty();
            _Compasses = new Collection<Compass>(this.GetVisualOfType<Compass>().ToList());
            foreach (var compass in _Compasses)
            {
                var headingProperty = Compass.HeadingProperty;
                var binding = new Binding()
                {
                    Source = _Map,
                    Path = new PropertyPath(MapCore.HeadingProperty)
                };
                compass.SetBinding(headingProperty, binding);
            }
            _TemplateApplied = true;
            RefreshMapMode();
        }

        private void _Map_ModeChanged(object sender, MapEventArgs e) => RefreshMapMode();

        private void RefreshMapMode()
        {
            if (!_TemplateApplied || _Map.Mode is null)
                return;
            _copyrightUpdateTimer.Stop();
            _copyrightUpdateTimer.Start();
            UpdateScale();
        }

        private void _UpdateTimer_Tick(object sender, EventArgs e) => UpdateScale();

        private void CopyrightUpdateTimerTick(object sender, EventArgs e)
        {
            _copyrightUpdateTimer.Stop();
            InvokeCopyrightRequest();
        }

        private void InvokeCopyrightRequest()
        {
            if (_Map.Mode is null || !_Map.Mode.MapStyle.HasValue)
                return;
            CopyrightManager.GetInstance(_Map.Culture, null).RequestCopyrightString(_Map.Mode.MapStyle, _Map.BoundingRectangle, _Map.ZoomLevel, _Map.CredentialsProvider, _Map.Culture, new Action<CopyrightResult>(CopyrightCallback));
        }

        private void CopyrightCallback(CopyrightResult result)
        {
            if (result is null || !(result.Culture == _Map.Culture) || (!(result.BoundingRectangle == _Map.BoundingRectangle) || result.ZoomLevel != _Map.ZoomLevel))
                return;
            foreach (var copyright1 in _Copyrights)
            {
                var copyright = copyright1;
                var attributionInfoList1 = new List<AttributionInfo>();
                foreach (var copyrightString in result.CopyrightStrings)
                {
                    var attributionInfo = new AttributionInfo(copyrightString);
                    if (!copyright.Attributions.Contains(attributionInfo))
                        attributionInfoList1.Add(attributionInfo);
                }
                var attributionInfoList2 = new List<AttributionInfo>();
                foreach (var attribution in copyright.Attributions)
                {
                    if (!result.CopyrightStrings.Contains(attribution.Text))
                        attributionInfoList2.Add(attribution);
                }
                attributionInfoList2.ForEach(attribInfo => copyright.Attributions.Remove(attribInfo));
                attributionInfoList1.ForEach(attribInfo => copyright.Attributions.Add(attribInfo));
            }
        }

        private void _Map_ViewChangeStart(object sender, MapEventArgs e)
        {
            if (!_TemplateApplied)
                return;
            _UpdateTimer.IsEnabled = true;
            _copyrightUpdateTimer.Stop();
        }

        private void _Map_ViewChangeEnd(object sender, MapEventArgs e)
        {
            _UpdateTimer.IsEnabled = false;
            _copyrightUpdateTimer.Stop();
            _copyrightUpdateTimer.Start();
            if (!_TemplateApplied)
                return;
            UpdateScale();
        }

        private void UpdateScale()
        {
            foreach (var scale in _Scales)
                scale.MetersPerPixel = MercatorUtility.ZoomToScale(MercatorModeLogicalAreaSizeInScreenSpaceAtLevel1, _Map.ZoomLevel, _Map.Center);
        }

        private static IEnumerable<DependencyObject> GetDescendents(
          DependencyObject root)
        {
            var count = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < count; ++i)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                foreach (var descendent in GetDescendents(child))
                    yield return descendent;
            }
        }
    }
}
