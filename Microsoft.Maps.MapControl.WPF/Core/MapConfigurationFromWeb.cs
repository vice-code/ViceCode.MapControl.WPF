using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Xml;
using Microsoft.Maps.MapControl.WPF.Resources;

namespace Microsoft.Maps.MapControl.WPF.Core
{
    internal class MapConfigurationFromWeb : MapConfigurationProvider
    {
        private readonly object configLock = new object();
        private readonly Uri configurationUri;
        private bool requestsPending;
        private readonly Dictionary<string, Collection<MapConfigurationGetSectionRequest>> requestQueue;
        private readonly Collection<string> requestedSections;
        private System.Timers.Timer requestTimer;

        public MapConfigurationFromWeb(Uri configurationUri)
        {
            this.configurationUri = configurationUri;
            Sections = new Dictionary<string, MapConfigurationSection>();
            requestQueue = new Dictionary<string, Collection<MapConfigurationGetSectionRequest>>();
            requestedSections = new Collection<string>();
        }

        public override event EventHandler<MapConfigurationLoadedEventArgs> Loaded;

        public override void LoadConfiguration()
        {
        }

        public override void Cancel()
        {
            lock (configLock)
                requestQueue.Clear();
        }

        public override void GetConfigurationSection(
          string version,
          string sectionName,
          string culture,
          MapConfigurationCallback callback,
          bool reExecuteCallback,
          object userState)
        {
            _version = version;
            _culture = culture;
            var flag1 = ContainConfigurationSection(version, sectionName, culture);
            var requestKey = GetConfigurationKey(version, sectionName, culture);
            if (!flag1)
            {
                bool flag2;
                lock (configLock)
                {
                    flag1 = ContainConfigurationSection(version, sectionName, culture) || requestedSections.Contains(requestKey);
                    flag2 = !flag1 && !requestQueue.ContainsKey(requestKey);
                    AddRequestToPendingQueue(version, sectionName, culture, callback, userState, requestKey);
                    requestsPending = true;
                }
                if (flag2)
                {
                    var sectionReader = (XmlReader)null;
                    try
                    {
                        using var storageFileStream = GetIsolatedStorageFileStream(requestKey, FileMode.OpenOrCreate);
                        if (storageFileStream.Length > 0L)
                        {
                            sectionReader = XmlReader.Create(new StreamReader(storageFileStream));
                        }
                        else
                        {
                            var manifestResourceStream = typeof(MapConfiguration).Assembly.GetManifestResourceStream("Microsoft.Maps.MapControl.WPF.Data.DesignConfig.xml");
                            if (manifestResourceStream is object)
                                sectionReader = XmlReader.Create(manifestResourceStream);
                        }
                        if (sectionReader is object)
                            ConfigLoaded(requestKey, ParseConfiguration(sectionReader));
                    }
                    catch (XmlException)
                    {
                    }
                    finally
                    {
                        sectionReader?.Dispose();
                    }
                    requestTimer = new System.Timers.Timer(Map.LoggingDelay > 0 ? Map.LoggingDelay : 1.0)
                    {
                        AutoReset = false
                    };
                    requestTimer.Elapsed += (sender, e) =>
                     {
                         requestTimer.Dispose();
                         requestTimer = null;
                         using var webClient = new WebClient();
                         webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(LoadFromServer_OpenReadCompleted);
                         webClient.OpenReadAsync(configurationUri, requestKey);
                     };
                    requestTimer.Start();
                }
            }
            else
            {
                lock (configLock)
                {
                    if (requestsPending)
                    {
                        if (reExecuteCallback)
                            AddRequestToPendingQueue(version, sectionName, culture, callback, userState, requestKey);
                    }
                }
            }
            if (!flag1 || callback is null)
                return;
            callback(GetSection(version, sectionName, culture), userState);
        }

        private void AddRequestToPendingQueue(
          string version,
          string sectionName,
          string culture,
          MapConfigurationCallback callback,
          object userState,
          string requestKey)
        {
            if (!requestQueue.ContainsKey(requestKey))
                requestQueue.Add(requestKey, new Collection<MapConfigurationGetSectionRequest>());
            requestQueue[requestKey].Add(new MapConfigurationGetSectionRequest(version, sectionName, culture, callback, userState));
        }

        private void LoadFromServer_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var userState = e.UserState as string;
            try
            {
                if (e.Error is object)
                    throw e.Error;
                if (e.Result is null)
                    throw new ConfigurationNotLoadedException(ExceptionStrings.MapConfiguration_WebService_InvalidResult);
                var byteList = new List<byte>();
                var buffer = new byte[1000];
                for (var index1 = e.Result.Read(buffer, 0, 1000); 0 < index1; index1 = e.Result.Read(buffer, 0, 1000))
                {
                    if (index1 == 1000)
                    {
                        byteList.AddRange(buffer);
                    }
                    else
                    {
                        for (var index2 = 0; index2 < index1; ++index2)
                            byteList.Add(buffer[index2]);
                    }
                }
                var array = byteList.ToArray();
                using (var memoryStream = new MemoryStream(array))
                {
                    var sectionReader = XmlReader.Create(memoryStream);
                    using (var storageFileStream = GetIsolatedStorageFileStream(userState, FileMode.Create))
                        new BinaryWriter(storageFileStream).Write(array);
                    ConfigLoaded(userState, ParseConfiguration(sectionReader));
                }
            }
            catch (Exception ex)
            {
                if (Loaded is object)
                    Loaded(this, new MapConfigurationLoadedEventArgs(ex));
            }
            lock (configLock)
                requestQueue.Remove(userState);
        }

        private void ConfigLoaded(
          string requestKey,
          Dictionary<string, MapConfigurationSection> sections)
        {
            var getSectionRequestList = new List<MapConfigurationGetSectionRequest>();
            lock (configLock)
            {
                foreach (var key in sections.Keys)
                    Sections[key] = sections[key];
                if (!requestedSections.Contains(requestKey))
                    requestedSections.Add(requestKey);
                if (requestQueue.ContainsKey(requestKey))
                {
                    foreach (var getSectionRequest in requestQueue[requestKey])
                        getSectionRequestList.Add(getSectionRequest);
                }
            }
            if (Loaded is object)
                Loaded(this, new MapConfigurationLoadedEventArgs(null));
            foreach (var getSectionRequest in getSectionRequestList)
            {
                if (getSectionRequest.Callback is object)
                    getSectionRequest.CallbackDispatcher.BeginInvoke(getSectionRequest.Callback, GetSection(getSectionRequest.Version, getSectionRequest.SectionName, getSectionRequest.Culture), getSectionRequest.UserState);
            }
        }

        private IsolatedStorageFileStream GetIsolatedStorageFileStream(
          string requestKey,
          FileMode mode)
        {
            var storeForAssembly = IsolatedStorageFile.GetUserStoreForAssembly();
            return new IsolatedStorageFileStream(string.Format("WPFMapcontrolIS2_{0}", requestKey), mode, storeForAssembly);
        }
    }
}
