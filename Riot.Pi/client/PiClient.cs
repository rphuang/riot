using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.Pi.Client
{
    /// <summary>
    /// implement a container for all nodes for a pi
    /// </summary>
    public class PiClient : IotClientNode
    {
        /// <summary>
        /// constructor with server:port and user:password
        /// </summary>
        public PiClient(IotHttpClient client) : base(string.Empty, client, null)
        { }

        /// <summary>
        /// constructor with server:port and user:password
        /// </summary>
        public PiClient(string server, string credential) : base(string.Empty, null, null)
        {
            Client = new IotHttpClient(server, credential);
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            return true;
        }
    }
}
