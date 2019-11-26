using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Maps.MapControl.WPF.PlatformServices;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal static class WebServicesUtility
    {
        internal static ImageryServiceClient CreateImageryServiceClient(string imageryServiceAddress) =>
            new ImageryServiceClient(new CustomBinding(new BindingElement[2]
            {
                 new BinaryMessageEncodingBindingElement(),
                 Map.UseHttps ? new HttpsTransportBindingElement() : new HttpTransportBindingElement()
            }), new EndpointAddress(imageryServiceAddress.Replace("{UriScheme}", Map.UriScheme)));

    }
}
