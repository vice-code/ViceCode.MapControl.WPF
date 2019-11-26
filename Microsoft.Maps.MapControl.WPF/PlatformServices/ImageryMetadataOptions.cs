using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DebuggerStepThrough]
    [DataContract(Name = "ImageryMetadataOptions", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [Serializable]
    internal class ImageryMetadataOptions : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private Heading HeadingField;
        [OptionalField]
        private Location LocationField;
        [OptionalField]
        private bool ReturnImageryProvidersField;
        [OptionalField]
        private UriScheme UriSchemeField;
        [OptionalField]
        private int? ZoomLevelField;

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
        internal Heading Heading
        {
            get
            {
                return this.HeadingField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.HeadingField, (object)value))
                    return;
                this.HeadingField = value;
                this.RaisePropertyChanged(nameof(Heading));
            }
        }

        [DataMember]
        internal Location Location
        {
            get
            {
                return this.LocationField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.LocationField, (object)value))
                    return;
                this.LocationField = value;
                this.RaisePropertyChanged(nameof(Location));
            }
        }

        [DataMember]
        internal bool ReturnImageryProviders
        {
            get
            {
                return this.ReturnImageryProvidersField;
            }
            set
            {
                if (this.ReturnImageryProvidersField.Equals(value))
                    return;
                this.ReturnImageryProvidersField = value;
                this.RaisePropertyChanged(nameof(ReturnImageryProviders));
            }
        }

        [DataMember]
        internal UriScheme UriScheme
        {
            get
            {
                return this.UriSchemeField;
            }
            set
            {
                if (this.UriSchemeField.Equals((object)value))
                    return;
                this.UriSchemeField = value;
                this.RaisePropertyChanged(nameof(UriScheme));
            }
        }

        [DataMember]
        internal int? ZoomLevel
        {
            get
            {
                return this.ZoomLevelField;
            }
            set
            {
                if (this.ZoomLevelField.Equals((object)value))
                    return;
                this.ZoomLevelField = value;
                this.RaisePropertyChanged(nameof(ZoomLevel));
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
