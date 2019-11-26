using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "ImageryMetadataBirdseyeResult", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [Serializable]
    internal class ImageryMetadataBirdseyeResult : ImageryMetadataResult
    {
        [OptionalField]
        private Heading HeadingField;
        [OptionalField]
        private int TilesXField;
        [OptionalField]
        private int TilesYField;

        [DataMember]
        internal Heading Heading
        {
            get
            {
                return this.HeadingField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.HeadingField, (object)value))
                    return;
                this.HeadingField = value;
                this.RaisePropertyChanged(nameof(Heading));
            }
        }

        [DataMember]
        internal int TilesX
        {
            get
            {
                return this.TilesXField;
            }
            set
            {
                if (this.TilesXField.Equals(value))
                    return;
                this.TilesXField = value;
                this.RaisePropertyChanged(nameof(TilesX));
            }
        }

        [DataMember]
        internal int TilesY
        {
            get
            {
                return this.TilesYField;
            }
            set
            {
                if (this.TilesYField.Equals(value))
                    return;
                this.TilesYField = value;
                this.RaisePropertyChanged(nameof(TilesY));
            }
        }
    }
}
