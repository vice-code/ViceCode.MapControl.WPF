using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "MapUriRequest", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DebuggerStepThrough]
    [Serializable]
    internal class MapUriRequest : RequestBase
    {
        [OptionalField]
        private Location CenterField;
        [OptionalField]
        private Location MajorRoutesDestinationField;
        [OptionalField]
        private MapUriOptions OptionsField;
        [OptionalField]
        private ObservableCollection<Pushpin> PushpinsField;

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
        internal Location MajorRoutesDestination
        {
            get
            {
                return this.MajorRoutesDestinationField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.MajorRoutesDestinationField, (object)value))
                    return;
                this.MajorRoutesDestinationField = value;
                this.RaisePropertyChanged(nameof(MajorRoutesDestination));
            }
        }

        [DataMember]
        internal MapUriOptions Options
        {
            get
            {
                return this.OptionsField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.OptionsField, (object)value))
                    return;
                this.OptionsField = value;
                this.RaisePropertyChanged(nameof(Options));
            }
        }

        [DataMember]
        internal ObservableCollection<Pushpin> Pushpins
        {
            get
            {
                return this.PushpinsField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.PushpinsField, (object)value))
                    return;
                this.PushpinsField = value;
                this.RaisePropertyChanged(nameof(Pushpins));
            }
        }
    }
}
