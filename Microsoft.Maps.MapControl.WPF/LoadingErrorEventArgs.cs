using System;

namespace Microsoft.Maps.MapControl.WPF
{
    public class LoadingErrorEventArgs : MapEventArgs
    {
        public LoadingErrorEventArgs(Exception loadingException) => LoadingException = loadingException;

        public Exception LoadingException { get; private set; }
    }
}
