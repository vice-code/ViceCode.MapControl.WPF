using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "UserProfile", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DebuggerStepThrough]
    [Serializable]
    internal class UserProfile : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private Heading CurrentHeadingField;
        [OptionalField]
        private UserLocation CurrentLocationField;
        [OptionalField]
        private DeviceType DeviceTypeField;
        [OptionalField]
        private DistanceUnit DistanceUnitField;
        [OptionalField]
        private string IPAddressField;
        [OptionalField]
        private ShapeBase MapViewField;
        [OptionalField]
        private SizeOfint ScreenSizeField;

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
        internal Heading CurrentHeading
        {
            get
            {
                return this.CurrentHeadingField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.CurrentHeadingField, (object)value))
                    return;
                this.CurrentHeadingField = value;
                this.RaisePropertyChanged(nameof(CurrentHeading));
            }
        }

        [DataMember]
        internal UserLocation CurrentLocation
        {
            get
            {
                return this.CurrentLocationField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.CurrentLocationField, (object)value))
                    return;
                this.CurrentLocationField = value;
                this.RaisePropertyChanged(nameof(CurrentLocation));
            }
        }

        [DataMember]
        internal DeviceType DeviceType
        {
            get
            {
                return this.DeviceTypeField;
            }
            set
            {
                if (this.DeviceTypeField.Equals((object)value))
                    return;
                this.DeviceTypeField = value;
                this.RaisePropertyChanged(nameof(DeviceType));
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
        internal string IPAddress
        {
            get
            {
                return this.IPAddressField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.IPAddressField, (object)value))
                    return;
                this.IPAddressField = value;
                this.RaisePropertyChanged(nameof(IPAddress));
            }
        }

        [DataMember]
        internal ShapeBase MapView
        {
            get
            {
                return this.MapViewField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.MapViewField, (object)value))
                    return;
                this.MapViewField = value;
                this.RaisePropertyChanged(nameof(MapView));
            }
        }

        [DataMember]
        internal SizeOfint ScreenSize
        {
            get
            {
                return this.ScreenSizeField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ScreenSizeField, (object)value))
                    return;
                this.ScreenSizeField = value;
                this.RaisePropertyChanged(nameof(ScreenSize));
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
