using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    [DataContract]
    internal class Session
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
    }
}
