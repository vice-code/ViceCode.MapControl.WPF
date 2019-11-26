using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Maps.MapControl.WPF.Overlays
{
    [CompilerGenerated]
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [DebuggerNonUserCode]
    internal class OverlayResources
    {
        private ResourceManager resourceMan;
        private CultureInfo resourceCulture;

        internal OverlayResources()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals((object)this.resourceMan, (object)null))
                    this.resourceMan = new ResourceManager("Microsoft.Maps.MapControl.WPF.Overlays.OverlayResources", typeof(OverlayResources).Assembly);
                return this.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal CultureInfo Culture
        {
            get
            {
                return this.resourceCulture;
            }
            set
            {
                this.resourceCulture = value;
            }
        }

        internal string FeetSingular
        {
            get
            {
                return this.ResourceManager.GetString(nameof(FeetSingular), this.resourceCulture);
            }
        }

        internal string FeetPlural
        {
            get
            {
                return this.ResourceManager.GetString(nameof(FeetPlural), this.resourceCulture);
            }
        }

        internal string InvalidCredentialsErrorMessage
        {
            get
            {
                return this.ResourceManager.GetString(nameof(InvalidCredentialsErrorMessage), this.resourceCulture);
            }
        }

        internal string KilometersSingular
        {
            get
            {
                return this.ResourceManager.GetString(nameof(KilometersSingular), this.resourceCulture);
            }
        }

        internal string KilometersPlural
        {
            get
            {
                return this.ResourceManager.GetString(nameof(KilometersPlural), this.resourceCulture);
            }
        }

        internal string LoadingConfigurationErrorMessage
        {
            get
            {
                return this.ResourceManager.GetString(nameof(LoadingConfigurationErrorMessage), this.resourceCulture);
            }
        }

        internal string LoadingUriSchemeErrorMessage
        {
            get
            {
                return this.ResourceManager.GetString(nameof(LoadingUriSchemeErrorMessage), this.resourceCulture);
            }
        }

        internal string MetersSingular
        {
            get
            {
                return this.ResourceManager.GetString(nameof(MetersSingular), this.resourceCulture);
            }
        }

        internal string MetersPlural
        {
            get
            {
                return this.ResourceManager.GetString(nameof(MetersPlural), this.resourceCulture);
            }
        }

        internal string MilesSingular
        {
            get
            {
                return this.ResourceManager.GetString(nameof(MilesSingular), this.resourceCulture);
            }
        }

        internal string MilesPlural
        {
            get
            {
                return this.ResourceManager.GetString(nameof(MilesPlural), this.resourceCulture);
            }
        }

        internal string YardsSingular
        {
            get
            {
                return this.ResourceManager.GetString(nameof(YardsSingular), this.resourceCulture);
            }
        }

        internal string YardsPlural
        {
            get
            {
                return this.ResourceManager.GetString(nameof(YardsPlural), this.resourceCulture);
            }
        }
    }
}
