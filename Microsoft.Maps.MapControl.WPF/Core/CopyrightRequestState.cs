using System;
using Microsoft.Maps.MapControl.WPF.PlatformServices;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class CopyrightRequestState
    {
        internal string Culture { get; }

        internal MapStyle Style { get; }

        internal LocationRect BoundingRectangle { get; }

        internal double ZoomLevel { get; }

        internal Credentials Credentials { get; }

        internal Action<CopyrightResult> CopyrightCallback { get; }

        internal CopyrightRequestState(string _culture, MapStyle _style, LocationRect _boundingRectangle, double _zoomLevel, Credentials _credentials, Action<CopyrightResult> _copyrightCallback)
        {
            Culture = _culture;
            Style = _style;
            BoundingRectangle = _boundingRectangle;
            ZoomLevel = _zoomLevel;
            Credentials = _credentials;
            CopyrightCallback = _copyrightCallback;
        }
    }
}
