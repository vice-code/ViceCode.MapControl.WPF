using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [KnownType(typeof(ImageryMetadataBirdseyeResult))]
    [DataContract(Name = "ImageryMetadataResult", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class ImageryMetadataResult : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private SizeOfint ImageSizeField;
        [OptionalField]
        private string ImageUriField;
        [OptionalField]
        private ObservableCollection<string> ImageUriSubdomainsField;
        [OptionalField]
        private ObservableCollection<ImageryProvider> ImageryProvidersField;
        [OptionalField]
        private RangeOfdateTime VintageField;
        [OptionalField]
        private RangeOfint ZoomRangeField;

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
        internal SizeOfint ImageSize
        {
            get
            {
                return this.ImageSizeField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ImageSizeField, (object)value))
                    return;
                this.ImageSizeField = value;
                this.RaisePropertyChanged(nameof(ImageSize));
            }
        }

        [DataMember]
        internal string ImageUri
        {
            get
            {
                return this.ImageUriField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ImageUriField, (object)value))
                    return;
                this.ImageUriField = value;
                this.RaisePropertyChanged(nameof(ImageUri));
            }
        }

        [DataMember]
        internal ObservableCollection<string> ImageUriSubdomains
        {
            get
            {
                return this.ImageUriSubdomainsField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ImageUriSubdomainsField, (object)value))
                    return;
                this.ImageUriSubdomainsField = value;
                this.RaisePropertyChanged(nameof(ImageUriSubdomains));
            }
        }

        [DataMember]
        internal ObservableCollection<ImageryProvider> ImageryProviders
        {
            get
            {
                return this.ImageryProvidersField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ImageryProvidersField, (object)value))
                    return;
                this.ImageryProvidersField = value;
                this.RaisePropertyChanged(nameof(ImageryProviders));
            }
        }

        [DataMember]
        internal RangeOfdateTime Vintage
        {
            get
            {
                return this.VintageField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.VintageField, (object)value))
                    return;
                this.VintageField = value;
                this.RaisePropertyChanged(nameof(Vintage));
            }
        }

        [DataMember]
        internal RangeOfint ZoomRange
        {
            get
            {
                return this.ZoomRangeField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ZoomRangeField, (object)value))
                    return;
                this.ZoomRangeField = value;
                this.RaisePropertyChanged(nameof(ZoomRange));
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
