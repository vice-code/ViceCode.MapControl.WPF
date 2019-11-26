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
    internal class LoadingErrorStrings
    {
        private ResourceManager resourceMan;
        private CultureInfo resourceCulture;

        internal LoadingErrorStrings()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals((object)this.resourceMan, (object)null))
                    this.resourceMan = new ResourceManager("Microsoft.Maps.MapControl.WPF.Overlays.LoadingErrorStrings", typeof(LoadingErrorStrings).Assembly);
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

        internal string InvalidCredentialsErrorMessage
        {
            get
            {
                return this.ResourceManager.GetString(nameof(InvalidCredentialsErrorMessage), this.resourceCulture);
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
    }
}
