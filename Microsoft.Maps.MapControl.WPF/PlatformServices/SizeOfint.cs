using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DebuggerStepThrough]
    [DataContract(Name = "SizeOfint", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class SizeOfint : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private int HeightField;
        [OptionalField]
        private int WidthField;

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
        internal int Height
        {
            get
            {
                return this.HeightField;
            }
            set
            {
                if (this.HeightField.Equals(value))
                    return;
                this.HeightField = value;
                this.RaisePropertyChanged(nameof(Height));
            }
        }

        [DataMember]
        internal int Width
        {
            get
            {
                return this.WidthField;
            }
            set
            {
                if (this.WidthField.Equals(value))
                    return;
                this.WidthField = value;
                this.RaisePropertyChanged(nameof(Width));
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
