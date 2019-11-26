using System;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    [Serializable]
    public class ConfigurationNotLoadedException : Exception
    {
        public ConfigurationNotLoadedException()
        {
        }

        public ConfigurationNotLoadedException(string message)
          : base(message)
        {
        }

        public ConfigurationNotLoadedException(string message, Exception innerException)
          : base(message, innerException)
        {
        }
    }
}
