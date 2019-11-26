using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DataContract(Name = "MapUriOptions", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DebuggerStepThrough]
    [Serializable]
    internal class MapUriOptions : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private ObservableCollection<string> DisplayLayersField;
        [OptionalField]
        private SizeOfint ImageSizeField;
        [OptionalField]
        private ImageType ImageTypeField;
        [OptionalField]
        private bool PreventIconCollisionField;
        [OptionalField]
        private MapStyle StyleField;
        [OptionalField]
        private UriScheme UriSchemeField;
        [OptionalField]
        private int? ZoomLevelField;

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
        internal ObservableCollection<string> DisplayLayers
        {
            get
            {
                return this.DisplayLayersField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.DisplayLayersField, (object)value))
                    return;
                this.DisplayLayersField = value;
                this.RaisePropertyChanged(nameof(DisplayLayers));
            }
        }

        [DataMember]
        internal SizeOfint ImageSize
        {
            get
            {
                return this.ImageSizeField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.ImageSizeField, (object)value))
                    return;
                this.ImageSizeField = value;
                this.RaisePropertyChanged(nameof(ImageSize));
            }
        }

        [DataMember]
        internal ImageType ImageType
        {
            get
            {
                return this.ImageTypeField;
            }
            set
            {
                if (this.ImageTypeField.Equals((object)value))
                    return;
                this.ImageTypeField = value;
                this.RaisePropertyChanged(nameof(ImageType));
            }
        }

        [DataMember]
        internal bool PreventIconCollision
        {
            get
            {
                return this.PreventIconCollisionField;
            }
            set
            {
                if (this.PreventIconCollisionField.Equals(value))
                    return;
                this.PreventIconCollisionField = value;
                this.RaisePropertyChanged(nameof(PreventIconCollision));
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

        [DataMember]
        internal UriScheme UriScheme
        {
            get
            {
                return this.UriSchemeField;
            }
            set
            {
                if (this.UriSchemeField.Equals((object)value))
                    return;
                this.UriSchemeField = value;
                this.RaisePropertyChanged(nameof(UriScheme));
            }
        }

        [DataMember]
        internal int? ZoomLevel
        {
            get
            {
                return this.ZoomLevelField;
            }
            set
            {
                if (this.ZoomLevelField.Equals((object)value))
                    return;
                this.ZoomLevelField = value;
                this.RaisePropertyChanged(nameof(ZoomLevel));
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
