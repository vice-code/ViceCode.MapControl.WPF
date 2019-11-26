using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [KnownType(typeof(MapUriResponse))]
    [KnownType(typeof(ImageryMetadataResponse))]
    [DataContract(Name = "ResponseBase", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class ResponseBase : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private Uri BrandLogoUriField;
        [OptionalField]
        private ResponseSummary ResponseSummaryField;

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
        internal Uri BrandLogoUri
        {
            get
            {
                return this.BrandLogoUriField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.BrandLogoUriField, (object)value))
                    return;
                this.BrandLogoUriField = value;
                this.RaisePropertyChanged(nameof(BrandLogoUri));
            }
        }

        [DataMember]
        internal ResponseSummary ResponseSummary
        {
            get
            {
                return this.ResponseSummaryField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ResponseSummaryField, (object)value))
                    return;
                this.ResponseSummaryField = value;
                this.RaisePropertyChanged(nameof(ResponseSummary));
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
