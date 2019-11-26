using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "RangeOfdateTime", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class RangeOfdateTime : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private DateTime FromField;
        [OptionalField]
        private DateTime ToField;

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
        internal DateTime From
        {
            get
            {
                return this.FromField;
            }
            set
            {
                if (this.FromField.Equals(value))
                    return;
                this.FromField = value;
                this.RaisePropertyChanged(nameof(From));
            }
        }

        [DataMember]
        internal DateTime To
        {
            get
            {
                return this.ToField;
            }
            set
            {
                if (this.ToField.Equals(value))
                    return;
                this.ToField = value;
                this.RaisePropertyChanged(nameof(To));
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
