using System;
using System.Collections.Generic;
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
        /// the name for the HttpRequestHandler
        /// </summary>
        public string Name { get; set; }

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
        public static HttpServiceResponse CreateResponseForBadRequest(HttpServiceContext context, string handlerName)
        {
            return CreateErrorResponse(context, handlerName, 400, "Bad Request");
        }

        /// <summary>
        /// create a response for 500 - internal error
        /// </summary>
        public static HttpServiceResponse CreateResponseForInternalError(HttpServiceContext context, string handlerName)
        {
            return CreateErrorResponse(context, handlerName, 500, "Internal Server Error");
        }

        /// <summary>
        /// create a response for 500 - internal error
        /// </summary>
        public static HttpServiceResponse CreateResponseForInternalError(HttpServiceContext context, string handlerName, Exception exception)
        {
            return CreateErrorResponse(context, handlerName, 500, "Internal Server Error", exception?.ToString());
        }

        /// <summary>
        /// create a response for error conditions
        /// </summary>
        public static HttpServiceResponse CreateErrorResponse(HttpServiceContext context, string handlerName, int errorCode, string errorDescription, string errorMessage = null)
        {
            HttpListenerResponse response = context.Context.Response;
            response.ContentType = "application/json";
            response.ContentLength64 = 0;
            response.StatusCode = errorCode;
            response.StatusDescription = errorDescription;
            string content = null;
            if (string.IsNullOrEmpty(errorMessage)) content = $"{{ \"status\": \"{errorDescription}\" }}";
            else content = $"{{ \"status\": \"{errorDescription}\", \"response\": \"{errorMessage}\" }}";
            return new HttpServiceResponse
            {
                Success = false,
                Request = context.Context.Request,
                Response = response,
                Content = content,
                HandlerName = handlerName
            };
        }

        /// <summary>
        /// construct a HttpRequestHandler with the path to handle
        /// </summary>
        /// <param name="path">the path in url must start with "/"</param>
        protected HttpServiceRequestHandler(string path)
        {
            if (Handlers.ContainsKey(path)) Handlers[path] = this;
            else Handlers.Add(path, this);
        }
    }
}
