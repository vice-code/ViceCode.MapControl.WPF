using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF.Overlays
{
    public partial class LoadingErrorMessage : UserControl
    {
        public LoadingErrorMessage() => InitializeComponent();

        public void SetConfigurationError(string culture) => ErrorMessage.Text = string.Format(ResourceUtility.GetCultureInfo(culture), ResourceUtility.GetResource<LoadingErrorStrings, LoadingErrorResourcesHelper>(culture).LoadingConfigurationErrorMessage);

        public void SetUriSchemeError(string culture) => ErrorMessage.Text = string.Format(ResourceUtility.GetCultureInfo(culture), ResourceUtility.GetResource<LoadingErrorStrings, LoadingErrorResourcesHelper>(culture).LoadingUriSchemeErrorMessage);

        public void SetCredentialsError(string culture) => ErrorMessage.Text = string.Format(ResourceUtility.GetCultureInfo(culture), ResourceUtility.GetResource<LoadingErrorStrings, LoadingErrorResourcesHelper>(culture).InvalidCredentialsErrorMessage);
    }
}
