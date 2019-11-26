using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DataContract(Name = "Pushpin", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [Serializable]
    internal class Pushpin : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private string IconStyleField;
        [OptionalField]
        private string LabelField;
        [OptionalField]
        private Location LocationField;

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
        internal string IconStyle
        {
            get
            {
                return this.IconStyleField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.IconStyleField, (object)value))
                    return;
                this.IconStyleField = value;
                this.RaisePropertyChanged(nameof(IconStyle));
            }
        }

        [DataMember]
        internal string Label
        {
            get
            {
                return this.LabelField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.LabelField, (object)value))
                    return;
                this.LabelField = value;
                this.RaisePropertyChanged(nameof(Label));
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
