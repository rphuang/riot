using Riot;
using Riot.Client;
using Riot.Pi.Client;
using System;
using System.Collections.Generic;

namespace Devices.Services
{
    /// <summary>
    /// implement factory for PiCar client node
    /// </summary>
    public class PiCarClientFactory : IotClientFactory
    {
        /// <summary>
        /// create and initialize a PiCar
        /// </summary>
        public static PiCar CreatePiCar(string serverAndPort, string credential)
        {
            DiscoverService discoverService = DiscoverService.GetOrCreateService(serverAndPort, credential);
            SystemClient httpSystem = discoverService.GetClientNode<SystemClient>();
            PiCar picar = discoverService.GetClientNode<PiCar>();
            picar.Server = httpSystem;
            return picar;
        }

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
                if (string.Equals("PiCar", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    PiCar picar = new PiCar(endpoint.Path, client);
                    root.AddNode(picar);
                }
            }
            if (root.Children.Count > 0) return root;
            return null;
        }
    }
}
