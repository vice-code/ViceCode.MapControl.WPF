using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "DeviceType", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    internal enum DeviceType
    {
        [EnumMember] Desktop,
        [EnumMember] Mobile,
    }
}
