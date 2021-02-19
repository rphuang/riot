using Riot.Client;
using System;
using System.Collections.Generic;

namespace Riot.Pi.Client
{
    /// <summary>
    /// implement factory for Pi client nodes
    /// </summary>
    public class PiClientFactory : IotClientFactory
    {
        /// <summary>
        /// create client node (and possible subnodes) based on the endpoints
        /// </summary>
        /// <param name="endpoints">the list of endpoints</param>
        /// <returns>returns the root client node that may contain all the child nodes</returns>
        protected override IotClientNode CreateClientNode(IList<HttpEndpoint> endpoints, IotHttpClient client)
        {
            IotGenericClient root = new IotGenericClient(client);
            foreach (HttpEndpoint endpoint in endpoints)
            {
                if (string.Equals("PiSystem", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    SystemClient sys = new SystemClient(endpoint.Path, client, null);
                    root.AddNode(sys);
                }
                else if (string.Equals("PiGpio", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    GpioClient gpio = new GpioClient(endpoint.Path, client, null);
                    root.AddNode(gpio);
                }
            }
            if (root.Children.Count > 0) return root;
            return null;
        }
    }
}
