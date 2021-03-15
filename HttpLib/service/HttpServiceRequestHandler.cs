using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HttpLib
{
    /// <summary>
    /// the base class for handling client http requests
    /// </summary>
    public class HttpServiceRequestHandler
    {
        /// <summary>
        /// the name for the HttpServiceRequestHandler
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the type for the HttpServiceRequestHandler
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// the path for the HttpServiceRequestHandler
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// the HttpRequestHandlers instantiated to handle requests
        /// </summary>
        public static IDictionary<string, HttpServiceRequestHandler> Handlers { get; } = new Dictionary<string, HttpServiceRequestHandler>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// derived class must implement to handle GET request
        /// </summary>
        public virtual HttpServiceResponse ProcessGetRequest(HttpServiceContext context)
        {
            return CreateResponseForBadRequest(context, Name);
        }

        /// <summary>
        /// derived class must implement to handle Post request
        /// </summary>
        public virtual HttpServiceResponse ProcessPostRequest(HttpServiceContext context)
        {
            return CreateResponseForBadRequest(context, Name);
        }

        /// <summary>
        /// derived class should implement to handle Put request to override default behavior that forward to POST
        /// </summary>
        public virtual HttpServiceResponse ProcessPutRequest(HttpServiceContext context)
        {
            return ProcessPostRequest(context);
        }

        /// <summary>
        /// create a response for 400 - BadRequest
        /// </summary>
        public static HttpServiceResponse CreateResponseForBadRequest(HttpServiceContext context, string handlerName, string errorMessage = null)
        {
            return CreateErrorResponse(context, handlerName, 400, errorMessage);
        }

        /// <summary>
        /// create a response for 500 - internal error
        /// </summary>
        public static HttpServiceResponse CreateResponseForInternalError(HttpServiceContext context, string handlerName, string errorMessage = null)
        {
            return CreateErrorResponse(context, handlerName, 500, errorMessage);
        }

        /// <summary>
        /// create a response for 500 - internal error
        /// </summary>
        public static HttpServiceResponse CreateResponseForInternalError(HttpServiceContext context, string handlerName, Exception exception)
        {
            return CreateErrorResponse(context, handlerName, 500, exception?.ToString());
        }

        /// <summary>
        /// create a response for error conditions
        /// </summary>
        public static HttpServiceResponse CreateErrorResponse(HttpServiceContext context, string handlerName, int errorCode, string errorMessage = null)
        {
            string content = null;
            if (!string.IsNullOrEmpty(errorMessage)) content = $"{{ \"response\": \"{errorMessage}\" }}";
            return CreateResponse(context, handlerName, false, errorCode, content);
        }

        /// <summary>
        /// create a response for success conditions
        /// </summary>
        public static HttpServiceResponse CreateSuccessResponse(HttpServiceContext context, string handlerName, int statusCode, string jsonContent = null)
        {
            return CreateResponse(context, handlerName, true, statusCode, jsonContent);
        }

        /// <summary>
        /// create a response
        /// </summary>
        public static HttpServiceResponse CreateResponse(HttpServiceContext context, string handlerName, bool success, int statusCode, string jsonContent = null)
        {
            HttpListenerResponse response = context.Context.Response;
            response.ContentType = "application/json";
            response.ContentLength64 = 0;
            response.StatusCode = statusCode;
            response.StatusDescription = ((HttpStatusCode)statusCode).ToString();
            return new HttpServiceResponse
            {
                Success = success,
                Request = context.Context.Request,
                Response = response,
                Content = jsonContent,
                HandlerName = handlerName
            };
        }

        /// <summary>
        /// Default JsonSerializerSettings
        /// </summary>
        public static JsonSerializerSettings DefaultJsonSerializerSettings { get; set; }  = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
            Formatting = Formatting.Indented
        };

        /// <summary>
        /// construct a HttpRequestHandler with the path to handle
        /// </summary>
        /// <param name="path">the path in url must start with "/"</param>
        protected HttpServiceRequestHandler(string name, string type, string path)
        {
            Name = name;
            Type = type;
            Path = path;
            if (Handlers.ContainsKey(path)) Handlers[path] = this;
            else Handlers.Add(path, this);
        }

        /// <summary>
        /// get the text from request stream
        /// </summary>
        protected string GetFromRequestStream(HttpListenerRequest request)
        {
            string dataString = null;
            using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string rawString = reader.ReadToEnd();
                dataString = System.Web.HttpUtility.UrlDecode(rawString);
            }
            return dataString;
        }
    }
}
