using System;

namespace Riot
{
    /// <summary>
    /// encapsulate definition of an endpoint (http path) provided by the http service
    /// </summary>
    public class HttpEndpoint
    {
        /// <summary>
        /// the name of the endpoint
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// the path of the endpoint
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// the type of the endpoint
        /// </summary>
        public String Type { get; set; }

        /// <summary>
        /// the parent name of the endpoint
        /// </summary>
        public String Parent { get; set; }

        /// <summary>
        /// the url of the endpoint
        /// </summary>
        public String Url { get; set; }
    }
}
