using System.Windows.Threading;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class MapConfigurationGetSectionRequest
    {
        public MapConfigurationGetSectionRequest(string version, string sectionName, string culture, MapConfigurationCallback callback, object userState)
        {
            Version = version;
            SectionName = sectionName;
            Culture = culture;
            Callback = callback;
            UserState = userState;
            CallbackDispatcher = Dispatcher.CurrentDispatcher;
        }

        public string Version { get; private set; }

        public string SectionName { get; private set; }

        public string Culture { get; private set; }

        public Dispatcher CallbackDispatcher { get; private set; }

        public MapConfigurationCallback Callback { get; private set; }

        public object UserState { get; private set; }
    }
}
