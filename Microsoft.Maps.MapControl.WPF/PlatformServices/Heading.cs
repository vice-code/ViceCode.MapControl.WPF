using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DebuggerStepThrough]
    [DataContract(Name = "Heading", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class Heading : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private double OrientationField;

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
        internal double Orientation
        {
            get
            {
                return this.OrientationField;
            }
            set
            {
                if (this.OrientationField.Equals(value))
                    return;
                this.OrientationField = value;
                this.RaisePropertyChanged(nameof(Orientation));
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
