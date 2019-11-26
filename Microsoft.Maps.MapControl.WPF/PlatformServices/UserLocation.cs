using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "UserLocation", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class UserLocation : Location
    {
        [OptionalField]
        private Confidence ConfidenceField;

        [DataMember]
        internal Confidence Confidence
        {
            get
            {
                return this.ConfidenceField;
            }
            set
            {
                if (this.ConfidenceField.Equals((object)value))
                    return;
                this.ConfidenceField = value;
                this.RaisePropertyChanged(nameof(Confidence));
            }
        }
    }
}
