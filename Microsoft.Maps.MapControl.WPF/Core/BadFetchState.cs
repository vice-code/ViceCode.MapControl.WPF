using System;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class BadFetchState
    {
        internal DateTime TryAgainAt { get; private set; }

        internal Credentials CredentialsLastUsed { get; private set; }

        internal Location Location { get; private set; }

        internal BadFetchState(DateTime againAt, Credentials credentials, Location location)
        {
            TryAgainAt = againAt;
            CredentialsLastUsed = credentials;
            Location = location;
        }
    }
}
