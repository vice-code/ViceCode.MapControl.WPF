using System.Globalization;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal sealed class CoreResourcesHelper : IResourceHelper<CoreResources>
    {
        public CoreResources CreateResource()
        {
            // ISSUE: object of a compiler-generated type is created
            return new CoreResources();
        }

        public void SetResourceCulture(CoreResources resource, CultureInfo culture) => CoreResources.Culture = culture;
    }
}
