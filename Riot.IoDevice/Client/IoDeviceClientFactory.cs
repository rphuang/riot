using HttpLib;
using Riot.Client;
using System;
using System.Collections.Generic;

namespace Riot.IoDevice.Client
{
    /// <summary>
    /// implement factory for IO Device client nodes
    /// </summary>
    public class IoDeviceClientFactory : IotClientFactory
    {
        /// <summary>
        /// create client node (and possible subnodes) based on the endpoints
        /// </summary>
        /// <param name="endpoints">the list of endpoints</param>
        /// <returns>returns the root client node that may contain all the child nodes</returns>
        protected override IotClientNode CreateClientNode(IList<HttpServiceEndpoint> endpoints, IotHttpClient client)
        {
            IotGenericClient root = new IotGenericClient(client);
            foreach (HttpServiceEndpoint endpoint in endpoints)
            {
                if (string.Equals("HygroThermoSensor", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    HygroThermoSensorClient device = new HygroThermoSensorClient(endpoint.Path, client, null);
                    root.AddNode(device);
                }
                if (string.Equals("Motor", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    MotorClient device = new MotorClient(endpoint.Path, client, null);
                    root.AddNode(device);
                }
                if (string.Equals("Servo", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    ServoClient device = new ServoClient(endpoint.Path, client, null);
                    root.AddNode(device);
                }
                if (string.Equals("RGBLed", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    RGBLedClient device = new RGBLedClient(endpoint.Path, client, null);
                    root.AddNode(device);
                }
                if (string.Equals("StripLedPattern", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    StripLedClient device = new StripLedClient(endpoint.Path, client, null);
                    root.AddNode(device);
                }
                if (string.Equals("Ultrasonic", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    UltrasonicClient device = new UltrasonicClient(endpoint.Path, client, null);
                    root.AddNode(device);
                }
                if (string.Equals("DistanceScan", endpoint.Type, StringComparison.OrdinalIgnoreCase))
                {
                    DistanceScanClient device = new DistanceScanClient(endpoint.Path, client, null);
                    root.AddNode(device);
                }
            }
            if (root.Children.Count > 0) return root;
            return null;
        }
    }
}
