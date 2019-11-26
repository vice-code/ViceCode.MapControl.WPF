using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "SearchPoint", Namespace = "http://dev.virtualearth.net/webservices/v1/search")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class SearchPoint : ShapeBase
    {
        [OptionalField]
        private Location PointField;

        [DataMember]
        internal Location Point
        {
            get
            {
                return this.PointField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.PointField, (object)value))
                    return;
                this.PointField = value;
                this.RaisePropertyChanged(nameof(Point));
            }
        }
    }
}
