using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Maps.MapControl.WPF.Core;

namespace Microsoft.Maps.MapControl.WPF
{
    public class ApplicationIdCredentialsProvider : CredentialsProvider, INotifyPropertyChanged
    {
        private string applicationId;
        private string sessionId;
        private List<Action<Credentials>> callbackQueue;

        public ApplicationIdCredentialsProvider()
          : this(string.Empty)
        {
        }

        public ApplicationIdCredentialsProvider(string applicationId) => ApplicationId = applicationId;

        public string ApplicationId
        {
            get => applicationId;
            set
            {
                applicationId = value;
                OnPropertyChanged(nameof(ApplicationId));
            }
        }

        public override void GetCredentials(Action<Credentials> callback)
        {
            if (callbackQueue is object)
                callbackQueue.Add(callback);
            else
                CallCallback(callback);
        }

        internal void StartSession()
        {
            if (callbackQueue is null)
                callbackQueue = new List<Action<Credentials>>();
            sessionId = null;
        }

        public override string SessionId => sessionId;

        internal void SetSessionId(string id)
        {
            sessionId = id;
            var callbackQueue = this.callbackQueue;
            this.callbackQueue = null;
            if (callbackQueue is null)
                return;
            foreach (var callback in callbackQueue)
                CallCallback(callback);
        }

        internal void EndSession()
        {
            sessionId = null;
            var callbackQueue = this.callbackQueue;
            this.callbackQueue = null;
            if (callbackQueue is null)
                return;
            foreach (var callback in callbackQueue)
                CallCallback(callback);
        }

        private void CallCallback(Action<Credentials> callback)
        {
            callback?.Invoke(new Credentials()
            {
                ApplicationId = string.IsNullOrEmpty(sessionId) ? ApplicationId : sessionId,
                Token = null
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged is null)
                return;
            var e = new PropertyChangedEventArgs(propertyName);
            propertyChanged(this, e);
        }
    }
}
