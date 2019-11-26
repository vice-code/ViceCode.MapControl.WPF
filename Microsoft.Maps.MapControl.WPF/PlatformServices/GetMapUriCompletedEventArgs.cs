using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    internal class GetMapUriCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public GetMapUriCompletedEventArgs(
          object[] results,
          Exception exception,
          bool cancelled,
          object userState)
          : base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public MapUriResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return (MapUriResponse)this.results[0];
            }
        }
    }
}
