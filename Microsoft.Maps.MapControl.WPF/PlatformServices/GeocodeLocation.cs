using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "GeocodeLocation", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class GeocodeLocation : Location
    {
        [OptionalField]
        private string CalculationMethodField;

        [DataMember]
        internal string CalculationMethod
        {
            get
            {
                return this.CalculationMethodField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.CalculationMethodField, (object)value))
                    return;
                this.CalculationMethodField = value;
                this.RaisePropertyChanged(nameof(CalculationMethod));
            }
        }
    }
}
