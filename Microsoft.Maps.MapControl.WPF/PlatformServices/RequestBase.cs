using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "RequestBase", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [KnownType(typeof(ImageryMetadataRequest))]
    [DebuggerStepThrough]
    [KnownType(typeof(MapUriRequest))]
    [Serializable]
    internal class RequestBase : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private Credentials CredentialsField;
        [OptionalField]
        private string CultureField;
        [OptionalField]
        private ExecutionOptions ExecutionOptionsField;
        [OptionalField]
        private UserProfile UserProfileField;

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
        internal Credentials Credentials
        {
            get
            {
                return this.CredentialsField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.CredentialsField, (object)value))
                    return;
                this.CredentialsField = value;
                this.RaisePropertyChanged(nameof(Credentials));
            }
        }

        [DataMember]
        internal string Culture
        {
            get
            {
                return this.CultureField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.CultureField, (object)value))
                    return;
                this.CultureField = value;
                this.RaisePropertyChanged(nameof(Culture));
            }
        }

        [DataMember]
        internal ExecutionOptions ExecutionOptions
        {
            get
            {
                return this.ExecutionOptionsField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ExecutionOptionsField, (object)value))
                    return;
                this.ExecutionOptionsField = value;
                this.RaisePropertyChanged(nameof(ExecutionOptions));
            }
        }

        [DataMember]
        internal UserProfile UserProfile
        {
            get
            {
                return this.UserProfileField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.UserProfileField, (object)value))
                    return;
                this.UserProfileField = value;
                this.RaisePropertyChanged(nameof(UserProfile));
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
