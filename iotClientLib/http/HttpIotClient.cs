using HttpLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace IotClientLib
{
    /// <summary>
    /// An IOT client using http protocol to communicate with server
    /// </summary>
    public class HttpIotClient
    {
        /// <summary>
        /// set user and password
        /// </summary>
        public void SetCredential(string username, string password)
        {
            _encodedCredential = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            if (!string.IsNullOrEmpty(_encodedCredential))
            {
                _requestHeaderParams = new Dictionary<string, string>();
                _requestHeaderParams.Add("Authorization", "Basic " + _encodedCredential);
            }
        }

        /// <summary>
        /// initialize to the specified server and port
        /// </summary>
        public void SetServer(string server, int port)
        {
            _server = server;
            _port = port;
        }

        /// <summary>
        /// send a get request to server using http protocol
        /// </summary>
        /// <param name="path">the device/component path for the url.</param>
        /// <returns>returns the json response from server</returns>
        public string Get(string path)
        {
            HttpWebResponse httpResponse;
            return Get(path, out httpResponse);
        }

        /// <summary>
        /// send a get request to server using http protocol
        /// </summary>
        /// <param name="path">the device/component path for the url.</param>
        /// <param name="httpResponse">the HttpWebResponse from server</param>
        /// <returns>returns the json response from server</returns>
        public string Get(string path, out HttpWebResponse httpResponse)
        {
            string url = string.Format(UrlFormat, _server, _port, path);

            _lastMessage = _request.Get(url, _requestHeaderParams, out httpResponse, true);
            _lastStatusCode = httpResponse == null ? 0 : (int)httpResponse.StatusCode;
            return _lastMessage;
        }

        /// <summary>
        /// post a command to server using http protocol
        /// </summary>
        /// <param name="path">the device/component path for the url.</param>
        /// <param name="json">the json body to be sent to server.</param>
        /// <returns>returns the json response from server</returns>
        public string Post(string path, string json)
        {
            HttpWebResponse httpResponse;
            return Post(path, json, out httpResponse);
        }

        /// <summary>
        /// post a command to server using http protocol
        /// </summary>
        /// <param name="path">the device/component path for the url.</param>
        /// <param name="json">the json body to be sent to server.</param>
        /// <param name="httpResponse">the HttpWebResponse from server</param>
        /// <returns>returns the json response from server</returns>
        public string Post(string path, string json, out HttpWebResponse httpResponse)
        {
            string url = string.Format(UrlFormat, _server, _port, path);

            _lastMessage = _request.Post(url, json, _requestHeaderParams, out httpResponse, true);
            _lastStatusCode = httpResponse == null ? 0 : (int)httpResponse.StatusCode;
            return _lastMessage;
        }

        /// <summary>
        /// gets available endpoints from the server
        /// </summary>
        /// <returns>returns a list of endpoints</returns>
        public IList<HttpEndpoint> DiscoverAvailableEndpoints()
        {
            string json = Get(string.Empty);
            // deserialize
            List<HttpEndpoint> response = JsonConvert.DeserializeObject<List<HttpEndpoint>>(json);
            return response;
        }

        /// <summary>
        /// get last request message for PiClient
        /// </summary>
        public int LastStatusCode { get { return _lastStatusCode; } }

        /// <summary>
        /// get last request message for PiClient
        /// </summary>
        public string LastMessage { get { return _lastMessage; } }

        private const string UrlFormat = "http://{0}:{1}/{2}";
        private string _server;
        private int _port;
        private string _encodedCredential;
        private IDictionary<string, string> _requestHeaderParams;
        private HttpRequest _request = new HttpRequest();
        private string _lastMessage;
        private int _lastStatusCode;
    }
}
