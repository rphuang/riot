using System.Collections.Generic;

namespace Riot
{
    /// <summary>
    /// encapsulate a target site for sending notifications using http
    /// </summary>
    public class HttpTargetSite
    {
        /// <summary>
        /// the server address of the site (i.e., 192.168.1.111:8000)
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// the Credential of the site (format as user:password)
        /// </summary>
        public string Credential
        {
            get { return _credential; }
            set { SetAndEncodeCredential(value); }
        }

        /// <summary>
        /// the Token of the site
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// set user and password
        /// </summary>
        public void SetCredential(string username, string password)
        {
            SetAndEncodeCredential(username + ":" + password);
        }

        /// <summary>
        /// initialize to the specified server and port
        /// </summary>
        public void SetServer(string server, int port)
        {
            Server = server + ":" + port;
        }

        /// <summary>
        /// get http request header params (contains encoded credential & token)
        /// </summary>
        public IDictionary<string, string> HttpRequestHeaderParams { get { return _requestHeaderParams; } }

        /// <summary>
        /// get encoded credential
        /// </summary>
        internal string EncodedCredential { get { return _encodedCredential; } }

        private void SetAndEncodeCredential(string credential)
        {
            _credential = credential;
            _encodedCredential = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(credential));
            if (!string.IsNullOrEmpty(_encodedCredential))
            {
                _requestHeaderParams = new Dictionary<string, string>();
                _requestHeaderParams.Add("Authorization", "Basic " + _encodedCredential);
            }
        }

        private string _credential;
        private string _encodedCredential;
        private IDictionary<string, string> _requestHeaderParams;
    }
}
