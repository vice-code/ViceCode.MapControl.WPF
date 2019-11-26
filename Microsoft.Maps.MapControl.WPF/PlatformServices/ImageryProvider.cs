using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DataContract(Name = "ImageryProvider", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [DebuggerStepThrough]
    [Serializable]
    internal class ImageryProvider : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private string AttributionField;
        [OptionalField]
        private ObservableCollection<CoverageArea> CoverageAreasField;

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
        internal string Attribution
        {
            get
            {
                return this.AttributionField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.AttributionField, (object)value))
                    return;
                this.AttributionField = value;
                this.RaisePropertyChanged(nameof(Attribution));
            }
        }

        [DataMember]
        internal ObservableCollection<CoverageArea> CoverageAreas
        {
            get
            {
                return this.CoverageAreasField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.CoverageAreasField, (object)value))
                    return;
                this.CoverageAreasField = value;
                this.RaisePropertyChanged(nameof(CoverageAreas));
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
