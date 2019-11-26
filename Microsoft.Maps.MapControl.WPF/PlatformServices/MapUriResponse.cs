using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DataContract(Name = "MapUriResponse", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [DebuggerStepThrough]
    [Serializable]
    internal class MapUriResponse : ResponseBase
    {
        [OptionalField]
        private string UriField;

        [DataMember]
        internal string Uri
        {
            get
            {
                return this.UriField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.UriField, (object)value))
                    return;
                this.UriField = value;
                this.RaisePropertyChanged(nameof(Uri));
            }
        }
    }
}
