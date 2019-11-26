using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DataContract(Name = "ExecutionOptions", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [DebuggerStepThrough]
    [Serializable]
    internal class ExecutionOptions : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private bool SuppressFaultsField;

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
        internal bool SuppressFaults
        {
            get
            {
                return this.SuppressFaultsField;
            }
            set
            {
                if (this.SuppressFaultsField.Equals(value))
                    return;
                this.SuppressFaultsField = value;
                this.RaisePropertyChanged(nameof(SuppressFaults));
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
