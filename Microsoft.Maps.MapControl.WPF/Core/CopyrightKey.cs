using Microsoft.Maps.MapControl.WPF.PlatformServices;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class CopyrightKey
    {
        internal string Culture { get; }
        internal MapStyle Style { get; }
        internal CopyrightKey(string _culture, MapStyle _style)
        {
            Culture = _culture;
            Style = _style;
        }
    }
}
