using System.Globalization;
using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF.Overlays
{
    internal sealed class OverlayResourcesHelper : IResourceHelper<OverlayResources>
    {
        public OverlayResources CreateResource()
        {
            // ISSUE: object of a compiler-generated type is created
            return new OverlayResources();
        }

        public void SetResourceCulture(OverlayResources resource, CultureInfo culture) => resource.Culture = culture;
    }
}
