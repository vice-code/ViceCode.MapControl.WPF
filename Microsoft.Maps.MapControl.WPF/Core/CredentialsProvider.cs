using System;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    public abstract class CredentialsProvider
    {
        public abstract void GetCredentials(Action<Credentials> callback);

        public abstract string SessionId { get; }
    }
}
