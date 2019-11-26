using System.Globalization;
using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF.Overlays
{
    internal sealed class LoadingErrorResourcesHelper : IResourceHelper<LoadingErrorStrings>
    {
        public LoadingErrorStrings CreateResource()
        {
            // ISSUE: object of a compiler-generated type is created
            return new LoadingErrorStrings();
        }

        public void SetResourceCulture(LoadingErrorStrings resource, CultureInfo culture) => resource.Culture = culture;
    }
}
