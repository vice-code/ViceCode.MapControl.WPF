using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Microsoft.Maps.MapControl.WPF.PlatformServices
{
    [ServiceContract(ConfigurationName = "PlatformServices.IImageryService", Namespace = "http://dev.virtualearth.net/webservices/v1/imagery/contracts")]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    internal interface IImageryService
    {
        [FaultContract(typeof(ResponseSummary), Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetImageryMetadataResponseSummaryFault", Name = "ResponseSummary", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
        [OperationContract(Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetImageryMetadata", ReplyAction = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetImageryMetadataResponse")]
        ImageryMetadataResponse GetImageryMetadata(ImageryMetadataRequest request);

        [OperationContract(Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetImageryMetadata", AsyncPattern = true, ReplyAction = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetImageryMetadataResponse")]
        IAsyncResult BeginGetImageryMetadata(
          ImageryMetadataRequest request,
          AsyncCallback callback,
          object asyncState);

        ImageryMetadataResponse EndGetImageryMetadata(IAsyncResult result);

        [FaultContract(typeof(ResponseSummary), Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetMapUriResponseSummaryFault", Name = "ResponseSummary", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
        [OperationContract(Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetMapUri", ReplyAction = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetMapUriResponse")]
        MapUriResponse GetMapUri(MapUriRequest request);

        [OperationContract(Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetMapUri", AsyncPattern = true, ReplyAction = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetMapUriResponse")]
        IAsyncResult BeginGetMapUri(
          MapUriRequest request,
          AsyncCallback callback,
          object asyncState);

        MapUriResponse EndGetMapUri(IAsyncResult result);
    }
}
