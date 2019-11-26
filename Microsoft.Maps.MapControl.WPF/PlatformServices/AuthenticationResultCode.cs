using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "AuthenticationResultCode", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    internal enum AuthenticationResultCode
    {
        [EnumMember] None = 0,
        [EnumMember] NoCredentials = 1,
        [EnumMember] ValidCredentials = 2,
        [EnumMember] InvalidCredentials = 3,
        [EnumMember] CredentialsExpired = 4,
        [EnumMember] NotAuthorized = 7,
    }
}
