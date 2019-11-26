using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    internal class ImageryServiceClient : ClientBase<IImageryService>, IImageryService
    {
        private ClientBase<IImageryService>.BeginOperationDelegate onBeginGetImageryMetadataDelegate;
        private ClientBase<IImageryService>.EndOperationDelegate onEndGetImageryMetadataDelegate;
        private SendOrPostCallback onGetImageryMetadataCompletedDelegate;
        private ClientBase<IImageryService>.BeginOperationDelegate onBeginGetMapUriDelegate;
        private ClientBase<IImageryService>.EndOperationDelegate onEndGetMapUriDelegate;
        private SendOrPostCallback onGetMapUriCompletedDelegate;

        public ImageryServiceClient()
        {
        }

        public ImageryServiceClient(string endpointConfigurationName)
          : base(endpointConfigurationName)
        {
        }

        public ImageryServiceClient(string endpointConfigurationName, string remoteAddress)
          : base(endpointConfigurationName, remoteAddress)
        {
        }

        public ImageryServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
          : base(endpointConfigurationName, remoteAddress)
        {
        }

        public ImageryServiceClient(Binding binding, EndpointAddress remoteAddress)
          : base(binding, remoteAddress)
        {
        }

        public event EventHandler<GetImageryMetadataCompletedEventArgs> GetImageryMetadataCompleted;

        public event EventHandler<GetMapUriCompletedEventArgs> GetMapUriCompleted;

        public ImageryMetadataResponse GetImageryMetadata(
          ImageryMetadataRequest request)
        {
            return this.Channel.GetImageryMetadata(request);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginGetImageryMetadata(
          ImageryMetadataRequest request,
          AsyncCallback callback,
          object asyncState)
        {
            return this.Channel.BeginGetImageryMetadata(request, callback, asyncState);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ImageryMetadataResponse EndGetImageryMetadata(IAsyncResult result)
        {
            return this.Channel.EndGetImageryMetadata(result);
        }

        private IAsyncResult OnBeginGetImageryMetadata(
          object[] inValues,
          AsyncCallback callback,
          object asyncState)
        {
            return this.BeginGetImageryMetadata((ImageryMetadataRequest)inValues[0], callback, asyncState);
        }

        private object[] OnEndGetImageryMetadata(IAsyncResult result)
        {
            return new object[1]
            {
        (object) this.EndGetImageryMetadata(result)
            };
        }

        private void OnGetImageryMetadataCompleted(object state)
        {
            if (this.GetImageryMetadataCompleted is null)
                return;
            ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs)state;
            this.GetImageryMetadataCompleted((object)this, new GetImageryMetadataCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        public void GetImageryMetadataAsync(ImageryMetadataRequest request)
        {
            this.GetImageryMetadataAsync(request, (object)null);
        }

        public void GetImageryMetadataAsync(ImageryMetadataRequest request, object userState)
        {
            if (this.onBeginGetImageryMetadataDelegate is null)
                this.onBeginGetImageryMetadataDelegate = new ClientBase<IImageryService>.BeginOperationDelegate(this.OnBeginGetImageryMetadata);
            if (this.onEndGetImageryMetadataDelegate is null)
                this.onEndGetImageryMetadataDelegate = new ClientBase<IImageryService>.EndOperationDelegate(this.OnEndGetImageryMetadata);
            if (this.onGetImageryMetadataCompletedDelegate is null)
                this.onGetImageryMetadataCompletedDelegate = new SendOrPostCallback(this.OnGetImageryMetadataCompleted);
            this.InvokeAsync(this.onBeginGetImageryMetadataDelegate, new object[1]
            {
        (object) request
            }, this.onEndGetImageryMetadataDelegate, this.onGetImageryMetadataCompletedDelegate, userState);
        }

        public MapUriResponse GetMapUri(MapUriRequest request)
        {
            return this.Channel.GetMapUri(request);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginGetMapUri(
          MapUriRequest request,
          AsyncCallback callback,
          object asyncState)
        {
            return this.Channel.BeginGetMapUri(request, callback, asyncState);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public MapUriResponse EndGetMapUri(IAsyncResult result)
        {
            return this.Channel.EndGetMapUri(result);
        }

        private IAsyncResult OnBeginGetMapUri(
          object[] inValues,
          AsyncCallback callback,
          object asyncState)
        {
            return this.BeginGetMapUri((MapUriRequest)inValues[0], callback, asyncState);
        }

        private object[] OnEndGetMapUri(IAsyncResult result)
        {
            return new object[1]
            {
        (object) this.EndGetMapUri(result)
            };
        }

        private void OnGetMapUriCompleted(object state)
        {
            if (this.GetMapUriCompleted is null)
                return;
            ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs)state;
            this.GetMapUriCompleted((object)this, new GetMapUriCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        public void GetMapUriAsync(MapUriRequest request)
        {
            this.GetMapUriAsync(request, (object)null);
        }

        public void GetMapUriAsync(MapUriRequest request, object userState)
        {
            if (this.onBeginGetMapUriDelegate is null)
                this.onBeginGetMapUriDelegate = new ClientBase<IImageryService>.BeginOperationDelegate(this.OnBeginGetMapUri);
            if (this.onEndGetMapUriDelegate is null)
                this.onEndGetMapUriDelegate = new ClientBase<IImageryService>.EndOperationDelegate(this.OnEndGetMapUri);
            if (this.onGetMapUriCompletedDelegate is null)
                this.onGetMapUriCompletedDelegate = new SendOrPostCallback(this.OnGetMapUriCompleted);
            this.InvokeAsync(this.onBeginGetMapUriDelegate, new object[1]
            {
        (object) request
            }, this.onEndGetMapUriDelegate, this.onGetMapUriCompletedDelegate, userState);
        }
    }
}
