using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "MapStyle", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    internal enum MapStyle
    {
        [EnumMember] Road,
        [EnumMember] Aerial,
        [EnumMember] AerialWithLabels,
        [EnumMember] Birdseye,
        [EnumMember] BirdseyeWithLabels,
        [EnumMember] Road_v0,
        [EnumMember] AerialWithLabels_v0,
        [EnumMember] BirdseyeWithLabels_v0,
        [EnumMember] Road_v1,
        [EnumMember] AerialWithLabels_v1,
        [EnumMember] BirdseyeWithLabels_v1,
    }
}
