using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.SmartPlug.Client
{
    /// <summary>
    /// implement factory for smart plug client nodes
    /// </summary>
    public class SmartPlugClientFactory : IotClientFactory
    {
        /// <summary>
        /// create client node (and possible subnodes) based on the endpoints
        /// </summary>
        /// <param name="endpoints">the list of endpoints</param>
        /// <returns>returns the root client node that may contain all the child nodes</returns>
        protected override IotClientNode CreateClientNode(IList<HttpEndpoint> endpoints, IotHttpClient client)
        {
            SmartPlugClient root = null;
            List<IotClientNode> subnodes = new List<IotClientNode>();
            foreach (HttpEndpoint endpoint in endpoints)
            {
                if (string.Equals("KasaSmartPlug", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    root = new SmartPlugClient(endpoint.Path, client);
                }
                else if (string.Equals("KasaHS1xx", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    KasaHs1xxClient sys = new KasaHs1xxClient(endpoint.Path, client);
                    subnodes.Add(sys);
                }
            }
            if (root != null && subnodes.Count > 0)
            {
                foreach (IotClientNode node in subnodes)
                {
                    if (!string.IsNullOrEmpty(root.Id)) node.Id = node.Id.Substring(root.Id.Length + 1);
                    root.AddNode(node);
                }
            }
            return root;
        }
    }
}
