using HttpLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;

namespace HttpLib
{
    /// <summary>
    /// Request handler to respond with available endpoints. This should be used for the root path.
    /// </summary>
    public class EndpointRequestHandler : HttpServiceRequestHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public EndpointRequestHandler(string name, string path) : base(name, "ServiceEndpoint", path)
        {
        }

        /// <summary>
        /// derived class must implement to handle GET request
        /// </summary>
        public override HttpServiceResponse ProcessGetRequest(HttpServiceContext context)
        {
            HttpListenerRequest request = context.Context.Request;
            HttpListenerResponse response = context.Context.Response;

            // only respond if the request is to the root path
            if (string.Equals(Path, request.Url.AbsolutePath, StringComparison.OrdinalIgnoreCase))
            {
                string rootUri = $"{request.Url.Scheme}//{request.Url.Host}:{request.Url.Port}";
                List<HttpServiceEndpoint> items = new List<HttpServiceEndpoint>();
                foreach (HttpServiceRequestHandler handler in Handlers.Values)
                {
                    items.Add(CreateHttpServiceEndpoint(handler, rootUri));
                }

                string responseString = JsonConvert.SerializeObject(items, DefaultJsonSerializerSettings);
                response.ContentType = "application/json";
                return new HttpServiceResponse { Request = request, Response = response, Content = responseString, Success = true };
            }
            return CreateResponseForBadRequest(context, Name, "Invalid Root Path");
        }

        private HttpServiceEndpoint CreateHttpServiceEndpoint(HttpServiceRequestHandler handler, string rootUri)
        {
            return new HttpServiceEndpoint
            {
                Name = handler.Name,
                Type = handler.Type,
                Path = handler.Path,
                Url = rootUri + Path,
            };
        }
    }
}
