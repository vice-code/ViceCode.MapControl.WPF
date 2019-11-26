using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DebuggerStepThrough]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DataContract(Name = "ImageryMetadataResponse", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [Serializable]
    internal class ImageryMetadataResponse : ResponseBase
    {
        [OptionalField]
        private ObservableCollection<ImageryMetadataResult> ResultsField;

        [DataMember]
        internal ObservableCollection<ImageryMetadataResult> Results
        {
            get
            {
                return this.ResultsField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ResultsField, (object)value))
                    return;
                this.ResultsField = value;
                this.RaisePropertyChanged(nameof(Results));
            }
        }
    }
}
