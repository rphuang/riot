using System;
using System.Collections.Generic;
using System.Text;

namespace IotClientLib
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
        /// the description of the endpoint
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// the url of the endpoint
        /// </summary>
        public String Url { get; set; }
    }
}
