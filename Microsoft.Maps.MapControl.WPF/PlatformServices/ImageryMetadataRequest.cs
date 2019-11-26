using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "ImageryMetadataRequest", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class ImageryMetadataRequest : RequestBase
    {
        [OptionalField]
        private ImageryMetadataOptions OptionsField;
        [OptionalField]
        private MapStyle StyleField;

        [DataMember]
        internal ImageryMetadataOptions Options
        {
            get
            {
                return this.OptionsField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.OptionsField, (object)value))
                    return;
                this.OptionsField = value;
                this.RaisePropertyChanged(nameof(Options));
            }
        }

        [DataMember]
        internal MapStyle Style
        {
            get
            {
                return this.StyleField;
            }
            set
            {
                if (this.StyleField.Equals((object)value))
                    return;
                this.StyleField = value;
                this.RaisePropertyChanged(nameof(Style));
            }
        }
    }
}
