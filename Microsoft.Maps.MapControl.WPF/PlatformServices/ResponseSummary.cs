using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DebuggerStepThrough]
    [DataContract(Name = "ResponseSummary", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class ResponseSummary : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private AuthenticationResultCode AuthenticationResultCodeField;
        [OptionalField]
        private string CopyrightField;
        [OptionalField]
        private string FaultReasonField;
        [OptionalField]
        private ResponseStatusCode StatusCodeField;
        [OptionalField]
        private string TraceIdField;

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
        internal AuthenticationResultCode AuthenticationResultCode
        {
            get
            {
                return this.AuthenticationResultCodeField;
            }
            set
            {
                if (this.AuthenticationResultCodeField.Equals((object)value))
                    return;
                this.AuthenticationResultCodeField = value;
                this.RaisePropertyChanged(nameof(AuthenticationResultCode));
            }
        }

        [DataMember]
        internal string Copyright
        {
            get
            {
                return this.CopyrightField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.CopyrightField, (object)value))
                    return;
                this.CopyrightField = value;
                this.RaisePropertyChanged(nameof(Copyright));
            }
        }

        [DataMember]
        internal string FaultReason
        {
            get
            {
                return this.FaultReasonField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.FaultReasonField, (object)value))
                    return;
                this.FaultReasonField = value;
                this.RaisePropertyChanged(nameof(FaultReason));
            }
        }

        [DataMember]
        internal ResponseStatusCode StatusCode
        {
            get
            {
                return this.StatusCodeField;
            }
            set
            {
                if (this.StatusCodeField.Equals((object)value))
                    return;
                this.StatusCodeField = value;
                this.RaisePropertyChanged(nameof(StatusCode));
            }
        }

        [DataMember]
        internal string TraceId
        {
            get
            {
                return this.TraceIdField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.TraceIdField, (object)value))
                    return;
                this.TraceIdField = value;
                this.RaisePropertyChanged(nameof(TraceId));
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
