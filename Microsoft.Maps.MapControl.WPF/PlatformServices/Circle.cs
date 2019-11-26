using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DebuggerStepThrough]
    [DataContract(Name = "Circle", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [Serializable]
    internal class Circle : ShapeBase
    {
        [OptionalField]
        private Location CenterField;
        [OptionalField]
        private DistanceUnit DistanceUnitField;
        [OptionalField]
        private double RadiusField;

        [DataMember]
        internal Location Center
        {
            get
            {
                return this.CenterField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.CenterField, (object)value))
                    return;
                this.CenterField = value;
                this.RaisePropertyChanged(nameof(Center));
            }
        }

        [DataMember]
        internal DistanceUnit DistanceUnit
        {
            get
            {
                return this.DistanceUnitField;
            }
            set
            {
                if (this.DistanceUnitField.Equals((object)value))
                    return;
                this.DistanceUnitField = value;
                this.RaisePropertyChanged(nameof(DistanceUnit));
            }
        }

        [DataMember]
        internal double Radius
        {
            get
            {
                return this.RadiusField;
            }
            set
            {
                if (this.RadiusField.Equals(value))
                    return;
                this.RadiusField = value;
                this.RaisePropertyChanged(nameof(Radius));
            }
        }
    }
}
