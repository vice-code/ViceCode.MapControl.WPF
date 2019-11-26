using System;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    public class MapConfigurationLoadedEventArgs : EventArgs
    {
        public MapConfigurationLoadedEventArgs(Exception error) => Error = error;

        public Exception Error { get; private set; }
    }
}
