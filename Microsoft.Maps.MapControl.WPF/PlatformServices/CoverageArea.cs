using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "CoverageArea", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class CoverageArea : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private Rectangle BoundingRectangleField;
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
        internal Rectangle BoundingRectangle
        {
            get
            {
                return this.BoundingRectangleField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.BoundingRectangleField, (object)value))
                    return;
                this.BoundingRectangleField = value;
                this.RaisePropertyChanged(nameof(BoundingRectangle));
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
