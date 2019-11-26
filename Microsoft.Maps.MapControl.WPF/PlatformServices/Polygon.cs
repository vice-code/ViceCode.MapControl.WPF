using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "Polygon", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class Polygon : ShapeBase
    {
        [OptionalField]
        private ObservableCollection<Location> VerticesField;

        [DataMember]
        internal ObservableCollection<Location> Vertices
        {
            get
            {
                return this.VerticesField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.VerticesField, (object)value))
                    return;
                this.VerticesField = value;
                this.RaisePropertyChanged(nameof(Vertices));
            }
        }
    }
}
