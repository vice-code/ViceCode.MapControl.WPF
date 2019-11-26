using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DebuggerStepThrough]
    [DataContract(Name = "Rectangle", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class Rectangle : ShapeBase
    {
        [OptionalField]
        private Location NortheastField;
        [OptionalField]
        private Location SouthwestField;

        [DataMember]
        internal Location Northeast
        {
            get
            {
                return this.NortheastField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.NortheastField, (object)value))
                    return;
                this.NortheastField = value;
                this.RaisePropertyChanged(nameof(Northeast));
            }
        }

        [DataMember]
        internal Location Southwest
        {
            get
            {
                return this.SouthwestField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.SouthwestField, (object)value))
                    return;
                this.SouthwestField = value;
                this.RaisePropertyChanged(nameof(Southwest));
            }
        }
    }
}
