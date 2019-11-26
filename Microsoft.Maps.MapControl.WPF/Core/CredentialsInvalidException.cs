using System;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    [Serializable]
    public class CredentialsInvalidException : Exception
    {
        public CredentialsInvalidException()
        {
        }

        public CredentialsInvalidException(string message)
          : base(message)
        {
        }

        public CredentialsInvalidException(string message, Exception innerException)
          : base(message, innerException)
        {
        }
    }
}
