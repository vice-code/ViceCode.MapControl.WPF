using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    internal class GetImageryMetadataCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public GetImageryMetadataCompletedEventArgs(
          object[] results,
          Exception exception,
          bool cancelled,
          object userState)
          : base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public ImageryMetadataResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return (ImageryMetadataResponse)this.results[0];
            }
        }
    }
}
