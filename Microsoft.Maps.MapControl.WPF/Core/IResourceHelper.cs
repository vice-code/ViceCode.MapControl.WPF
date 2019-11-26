using System.Globalization;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal interface IResourceHelper<TResource> where TResource : class
    {
        TResource CreateResource();

        void SetResourceCulture(TResource resource, CultureInfo culture);
    }
}
