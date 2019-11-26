using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DataContract(Name = "ResponseStatusCode", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    internal enum ResponseStatusCode
    {
        [EnumMember] Success,
        [EnumMember] BadRequest,
        [EnumMember] ServerError,
    }
}
