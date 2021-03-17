using HttpLib;
using System.Collections.Generic;
using System.Net;

namespace Riot.Phone
{
    /// <summary>
    /// defines base class for phone sensor data that has status
    /// </summary>
    public class WireData : IotData, ISubscribe, INotify
    {
        /// <summary>
        /// subscribe sensor data change notification
        /// </summary>
        /// <param name="targetAddress">the target service's address</param>
        public void Subscribe(string targetAddress)
        {
            Subscribe(targetAddress, null, null);
        }

        /// <summary>
        /// subscribe sensor data change notification
        /// </summary>
        /// <param name="targetAddress">the target service's address (ipaddress:port)</param>
        /// <param name="credential">the credential for requests to target service (user:password)</param>
        public void Subscribe(string targetAddress, string credential)
        {
            Subscribe(targetAddress, credential, null);
        }

        /// <summary>
        /// subscribe sensor data change notification
        /// </summary>
        /// <param name="targetAddress">the target service's address (ipaddress:port)</param>
        /// <param name="credential">the credential for requests to target service (user:password)</param>
        /// <param name="token">the token to be included for every requests</param>
        public void Subscribe(string targetAddress, string credential, string token)
        {
            HttpTargetSite endpoint = new HttpTargetSite { Server = targetAddress, Credential = credential, Token = token };
            Subscribe(endpoint);
        }

        /// <summary>
        /// subscribe sensor data change notification
        /// </summary>
        /// <param name="endpoint">the target HttpTargetEndpoint</param>
        public virtual void Subscribe(HttpTargetSite endpoint)
        {
            _notifications.Add(endpoint);
        }

        /// <summary>
        /// send json data to _notifications
        /// </summary>
        public void SendNotification()
        {
            string json = Serialize();
            SendNotification(json, _notifications);
        }

        /// <summary>
        /// send json data to recipients
        /// </summary>
        /// <param name="json">json data</param>
        protected void SendNotification(string json, List<HttpTargetSite> recipients)
        {
            foreach (HttpTargetSite endpoint in recipients)
            {
                string url = string.Format(UrlFormat, endpoint.Server, FullPath);
                HttpWebResponse httpResponse;
                string lastMessage = _request.Post(url, json, endpoint.HttpRequestHeaderParams, out httpResponse, true);
                int lastStatusCode = httpResponse == null ? 0 : (int)httpResponse.StatusCode;
            }
        }

        /// <summary>
        /// sensor change notification - list of target services for sending http post request
        /// </summary>
        protected List<HttpTargetSite> _notifications = new List<HttpTargetSite>();
        protected HttpRequest _request = new HttpRequest();
        protected const string UrlFormat = "http://{0}/{1}";
    }
}
