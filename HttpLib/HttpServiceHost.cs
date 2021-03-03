using LogLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HttpLib
{
    /// <summary>
    /// simple http web service host
    /// </summary>
    public class HttpServiceHost
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="server">the root prefix string for all paths to listen. ex: http://*:5678 </param>
        public HttpServiceHost(string rootPrefix)
        {
            RootPrefix = rootPrefix;
        }

        /// <summary>
        /// KeepAlive response to client
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// root prefix to listen
        /// </summary>
        public string RootPrefix { get; set; }

        public IList<string> Prefixes { get; private set; } = new List<string>();

        /// <summary>
        /// initialize 
        /// </summary>
        public HttpServiceHost Init()
        {
            _listener = new HttpListener();
            foreach (var item in HttpServiceRequestHandler.Handlers)
            {
                string prefix = RootPrefix + item.Key;
                Prefixes.Add(prefix);
                if (!prefix.EndsWith("/")) prefix += "/";
                _listener.Prefixes.Add(prefix);
            }
            string csv = string.Join(",", Prefixes);
            LogUtil.WriteAction($"HttpServiceHost initialized root: {RootPrefix} handlers: {Prefixes.Count} listening: {csv}");
            return this;
        }

        /// <summary>
        /// start listener
        /// </summary>
        public HttpServiceHost Start()
        {
            LogUtil.WriteAction($"HttpServiceHost starting");
            _listener.Start();
            return this;
        }

        /// <summary>
        /// stop listener
        /// </summary>
        public HttpServiceHost Stop()
        {
            LogUtil.WriteAction($"HttpServiceHost stopping");
            _listener.Stop();
            return this;
        }

        /// <summary>
        /// get context
        /// </summary>
        /// <returns></returns>
        public async Task<HttpServiceResponse> GetContextAsync()
        {
            HttpListenerContext context = await _listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            LogUtil.WriteAction($"{request.HttpMethod} {request.Url.AbsoluteUri}");

            HttpServiceRequestHandler handler = null;
            string matchedPath = null;
            if (!GetRequestHandler(request.Url.AbsolutePath, out handler, out matchedPath))
            {
                return HttpServiceRequestHandler.CreateResponseForBadRequest(new HttpServiceContext { Context = context, MatchedPath = matchedPath }, "NoHandler");
            }

            HttpServiceContext hostContext = new HttpServiceContext { Context = context, MatchedPath = matchedPath };
            HttpServiceResponse hostResponse = null;
            try
            {
                if (string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    hostResponse = handler.ProcessGetRequest(hostContext);
                }
                else if (string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
                {
                    hostResponse = handler.ProcessGetRequest(hostContext);
                }
                else if (string.Equals(request.HttpMethod, "PUT", StringComparison.OrdinalIgnoreCase))
                {
                    hostResponse = handler.ProcessGetRequest(hostContext);
                }

                if (hostResponse != null)
                {
                    if (!string.IsNullOrEmpty(hostResponse.Content))
                    {
                        // send content to response
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(hostResponse.Content);
                        // Get a response stream and write the response
                        response.ContentLength64 = buffer.Length;
                        response.KeepAlive = KeepAlive;
                        System.IO.Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        // must close the output stream.
                        output.Close();
                    }
                    else
                    {
                        response.ContentLength64 = 0;
                    }
                    if (string.IsNullOrEmpty(response.ContentType)) response.ContentType = "application/json";
                }
                else
                {
                    hostResponse = HttpServiceRequestHandler.CreateResponseForInternalError(hostContext, handler.Name);
                }
            }
            catch (Exception err)
            {
                hostResponse = HttpServiceRequestHandler.CreateResponseForInternalError(hostContext, handler.Name, err);
            }
            LogUtil.WriteAction($"{request.HttpMethod} {request.Url.AbsoluteUri} Status: {response.StatusCode} Result: {hostResponse.Content} Error: {hostResponse.ErrorMessage}");
            return hostResponse;
        }

        private bool GetRequestHandler(string absolutePath, out HttpServiceRequestHandler handler, out string matchedPath)
        {
            handler = null;
            matchedPath = null;
            string path = absolutePath;
            // first: find handler that matches the whole path
            if (HttpServiceRequestHandler.Handlers.TryGetValue(absolutePath, out handler))
            {
                matchedPath = absolutePath;
            }
            else
            {
                // then remove the last segment of the path and find a match until reaching "/"
                while (true)
                {
                    int index = path.LastIndexOf('/');
                    if (index == 0)
                    {
                        if (HttpServiceRequestHandler.Handlers.TryGetValue("/", out handler)) matchedPath = "/";
                        break;
                    }
                    else
                    {
                        path = path.Substring(0, index);
                        if (HttpServiceRequestHandler.Handlers.TryGetValue(absolutePath, out handler))
                        {
                            matchedPath = path;
                        }
                    }
                }
            }
            return handler != null && !string.IsNullOrEmpty(matchedPath);
        }

        private HttpListener _listener;
    }
}
