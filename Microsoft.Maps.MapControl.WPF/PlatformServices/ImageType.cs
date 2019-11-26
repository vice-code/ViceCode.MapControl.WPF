using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "ImageType", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    internal enum ImageType
    {
        [EnumMember] Default,
        [EnumMember] Png,
        [EnumMember] Jpeg,
        [EnumMember] Gif,
    }
}
