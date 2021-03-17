using HttpLib;
using Newtonsoft.Json;
using System;
using System.Net;

namespace Riot.Phone.Service
{
    /// <summary>
    /// Request handler to process phone's sensor requests
    /// </summary>
    public class PhoneRequestHandler : HttpServiceRequestHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public PhoneRequestHandler(IotNode service, string name, string path, string type, string defaultDataName, string credentials)
            : base(name, type, path, credentials)
        {
            _service = service;
            _defaultDataName = defaultDataName;
        }

        /// <summary>
        /// derived class must implement to handle GET request
        /// </summary>
        protected override HttpServiceResponse ProcessGetRequest(HttpServiceContext context)
        {
            HttpListenerRequest request = context.Context.Request;
            HttpListenerResponse response = context.Context.Response;

            string dataName = _defaultDataName;
            if (context.UnmatchedSegments?.Count > 0) dataName = context.UnmatchedSegments[0];
            IotData data = _service.GetData<IotData>(dataName);
            if (data != null)
            {
                string responseString = JsonConvert.SerializeObject(data, DefaultJsonSerializerSettings);
                response.ContentType = "application/json";
                return CreateSuccessResponse(context, Name, 200, responseString);
            }
            return CreateResponseForBadRequest(context, Name, $"InvalidDataName: {dataName}");
        }

        /// <summary>
        /// derived class must implement to handle Post request
        /// </summary>
        protected override HttpServiceResponse ProcessPostRequest(HttpServiceContext context)
        {
            BaseServiceNode service = _service as BaseServiceNode;
            if (service == null) return CreateResponseForBadRequest(context, Name, $"PostNotSupported {context.Context.Request.Url.AbsoluteUri}");

            string dataName = nameof(BaseServiceNode.Status);
            if (context.UnmatchedSegments?.Count > 0 && string.Equals(dataName, context.UnmatchedSegments[0], StringComparison.OrdinalIgnoreCase))
            {
                SensorStatusData data = null;
                string json = GetFromRequestStream(context.Context.Request);
                try
                {
                    data = JsonConvert.DeserializeObject<SensorStatusData>(json);
                }
                catch (Exception err)
                {
                    return CreateResponseForBadRequest(context, Name, $"InvalidData {json}");
                }

                service.SetStatus(data.SensorRate, data.IsOn);
                return CreateSuccessResponse(context, Name, 200);
            }

            return CreateResponseForBadRequest(context, Name, $"InvalidPostRequestPath {context.Context.Request.Url.AbsoluteUri}");
        }

        protected IotNode _service;
        protected string _defaultDataName;
    }
}
