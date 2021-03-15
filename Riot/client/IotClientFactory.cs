using HttpLib;
using System.Collections.Generic;

namespace Riot
{
    /// <summary>
    /// the factory for client node
    /// - use the static method to discover and create all client nodes
    /// - concrete factory class must implement the CreateClientNode 
    /// </summary>
    public abstract class IotClientFactory
    {
        /// <summary>
        /// Discover and create all RIOT nodes for the host server
        /// </summary>
        public static IotClientNode Discover(string serverAndPort, string userAndPassword)
        {
            IotHttpClient client = new IotHttpClient(serverAndPort, userAndPassword);
            IotClientNode root = new IotClientNode("", client, null);
            IList<HttpServiceEndpoint> endpoints = client.DiscoverAvailableEndpoints();
            if (endpoints?.Count > 0)
            {
                foreach (IotClientFactory factory in Factories)
                {
                    IotClientNode node = factory.CreateClientNode(endpoints, client);
                    if (node != null) root.AddNode(node);
                }
            }
            return root;
        }

        /// <summary>
        /// create client node (and possible subnodes) based on the endpoints
        /// </summary>
        /// <param name="endpoints">the list of endpoints</param>
        /// <returns>returns the root client node that may contain all the child nodes</returns>
        protected abstract IotClientNode CreateClientNode(IList<HttpServiceEndpoint> endpoints, IotHttpClient client);

        protected IotClientFactory()
        {
            Factories.Add(this);
        }

        private static List<IotClientFactory> Factories { get; set; } = new List<IotClientFactory>();
    }
}
