using HttpLib;
using System;
using System.Net;

namespace Devices.Services
{
    /// <summary>
    /// this RequestHandler process quit request that stops the service
    /// </summary>
    class QuitRequestHandler : HttpServiceRequestHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public QuitRequestHandler(string path, string credentials)
            : base("Quit", "Quit", path, credentials)
        {
        }

        /// <summary>
        /// whether a quit had been requested
        /// </summary>
        public bool GetAndResetQuitRequested
        {
            get
            {
                if (_quitRequested)
                {
                    _quitRequested = false;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// derived class must implement to handle GET request
        /// </summary>
        protected override HttpServiceResponse ProcessGetRequest(HttpServiceContext context)
        {
            HttpListenerRequest request = context.Context.Request;
            HttpListenerResponse response = context.Context.Response;

            // only respond if the request is to the quit path
            if (string.Equals(Path, request.Url.AbsolutePath, StringComparison.OrdinalIgnoreCase))
            {
                _quitRequested = true;
                return CreateSuccessResponse(context, Name, 200, "{ \"response\": \"Quiting\" }");
            }
            return CreateResponseForBadRequest(context, Name, "InvalidRootPath: " + request.Url.AbsolutePath);
        }

        /// <summary>
        /// derived class must implement to handle Post request
        /// </summary>
        protected override HttpServiceResponse ProcessPostRequest(HttpServiceContext context)
        {
            return ProcessGetRequest(context);
        }

        private bool _quitRequested;
    }
}
