using Riot;
using Riot.IoDevice.Client;
using Riot.Pi.Client;
using Riot.SmartPlug.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Services
{
    /// <summary>
    /// service for the PiStats via HttpSystem 
    /// </summary>
    public class DiscoverService
    {
        /// <summary>
        /// Get Or Create Service for the specified server
        /// </summary>
        public static DiscoverService GetOrCreateService(string serverAndPort, string credential, bool create = false)
        {
            DiscoverService service;
            if (_serviceDictionary.TryGetValue(serverAndPort, out service))
            {
                if (create)
                {
                    service = new DiscoverService(serverAndPort, credential);
                    _serviceDictionary[serverAndPort] = service;
                }
            }
            else
            {
                service = new DiscoverService(serverAndPort, credential);
                _serviceDictionary.Add(serverAndPort, service);
            }
            return service;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public DiscoverService(string serverAndPort, string credential)
        {
            // discover server endpoints and create client nodes
            IotClientNode rootNode = IotClientFactory.Discover(serverAndPort, credential);
            BuildNodeDictionary(rootNode, _nodeDictionary);
        }

        /// <summary>
        /// get a single client node by type
        /// </summary>
        public T GetClientNode<T>() where T : IotClientNode
        {
            IList<T> nodelist = GetClientNodes<T>();
            return nodelist?.FirstOrDefault();
        }

        /// <summary>
        /// get list of client node by type
        /// </summary>
        public IList<T> GetClientNodes<T>() where T : IotClientNode
        {
            string typeName = typeof(T).Name;
            List<IotClientNode> nodelist;
            if (_nodeDictionary.TryGetValue(typeName, out nodelist))
            {
                List<T> results = new List<T>();
                foreach (T node in nodelist) results.Add(node);
                return results;
            }
            return null;
        }

        private void BuildNodeDictionary(IotClientNode node, Dictionary<string, List<IotClientNode>> dictionary)
        {
            string typeName = node.GetType().Name;
            List<IotClientNode> nodelist;
            if (!dictionary.TryGetValue(typeName, out nodelist))
            {
                nodelist = new List<IotClientNode>();
                dictionary.Add(typeName, nodelist);
            }
            nodelist.Add(node);
            if (node.Children != null)
            {
                foreach (IotClientNode childnode in node.Children)
                {
                    BuildNodeDictionary(childnode, dictionary);
                }
            }
        }

        // instance dictionary key: type name, value: list of IotClientNode
        private Dictionary<string, List<IotClientNode>> _nodeDictionary = new Dictionary<string, List<IotClientNode>>(StringComparer.OrdinalIgnoreCase);
        // static dictionary key: serverAndPort, value: DiscoverService
        private static Dictionary<string, DiscoverService> _serviceDictionary = new Dictionary<string, DiscoverService>(StringComparer.OrdinalIgnoreCase);
        // create client factories
        private static PiClientFactory pi = new PiClientFactory();
        private static IoDeviceClientFactory iodevice = new IoDeviceClientFactory();
        private static SmartPlugClientFactory plug = new SmartPlugClientFactory();
        private static PiCarClientFactory picar = new PiCarClientFactory();
    }
}
