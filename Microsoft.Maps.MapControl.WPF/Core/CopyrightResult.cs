using System.Collections.Generic;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class CopyrightResult
    {
        internal CopyrightResult(IList<string> copyrightStrings, string culture, LocationRect boundingRectangle, double zoomLevel)
        {
            CopyrightStrings = copyrightStrings;
            Culture = culture;
            BoundingRectangle = boundingRectangle;
            ZoomLevel = zoomLevel;
        }

        public IList<string> CopyrightStrings { get; private set; }

        public string Culture { get; private set; }

        public LocationRect BoundingRectangle { get; private set; }

        public double ZoomLevel { get; private set; }
    }
}
