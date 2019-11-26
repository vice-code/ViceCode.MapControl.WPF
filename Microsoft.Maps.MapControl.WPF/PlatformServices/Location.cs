using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [KnownType(typeof(UserLocation))]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DataContract(Name = "Location", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [KnownType(typeof(GeocodeLocation))]
    [Serializable]
    internal class Location : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private double AltitudeField;
        [OptionalField]
        private double LatitudeField;
        [OptionalField]
        private double LongitudeField;

        [Browsable(false)]
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [DataMember]
        internal double Altitude
        {
            get
            {
                return this.AltitudeField;
            }
            set
            {
                if (this.AltitudeField.Equals(value))
                    return;
                this.AltitudeField = value;
                this.RaisePropertyChanged(nameof(Altitude));
            }
        }

        [DataMember]
        internal double Latitude
        {
            get
            {
                return this.LatitudeField;
            }
            set
            {
                if (this.LatitudeField.Equals(value))
                    return;
                this.LatitudeField = value;
                this.RaisePropertyChanged(nameof(Latitude));
            }
        }

        [DataMember]
        internal double Longitude
        {
            get
            {
                return this.LongitudeField;
            }
            set
            {
                if (this.LongitudeField.Equals(value))
                    return;
                this.LongitudeField = value;
                this.RaisePropertyChanged(nameof(Longitude));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged is null)
                return;
            propertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
