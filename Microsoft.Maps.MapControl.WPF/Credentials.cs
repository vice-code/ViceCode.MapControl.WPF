using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF
{
    [DataContract(Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    public class Credentials
    {
        [DataMember]
        public string ApplicationId { get; set; }

        [DataMember]
        public string Token { get; set; }

        public static bool operator ==(Credentials credentials1, Credentials credentials2)
        {
            if (ReferenceEquals(credentials1, credentials2))
                return true;
            if (credentials1 is null || credentials2 is null || !(credentials1.ApplicationId == credentials2.ApplicationId))
                return false;
            return credentials1.Token == credentials2.Token;
        }

        public static bool operator !=(Credentials credentials1, Credentials credentials2) => !(credentials1 == credentials2);

        public override bool Equals(object obj)
        {
            if (obj is Credentials credentials)
                return this == credentials;
            return false;
        }

        public override int GetHashCode() => ApplicationId.GetHashCode() ^ Token.GetHashCode();

        public override string ToString()
        {
            var str = string.Empty;
            if (!string.IsNullOrEmpty(ApplicationId))
                str = ApplicationId;
            else if (!string.IsNullOrEmpty(Token))
                str = Token;
            return str;
        }
    }
}
