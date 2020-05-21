using IotClientLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PiCar.Services
{
    /// <summary>
    /// service for the PiStats via HttpSystem 
    /// </summary>
    public class PiStatsService
    {
        /// <summary>
        /// constructor
        /// </summary>
        public PiStatsService(string name, string serverAddress, int serverPort, string userName, string userPassword)
        {
            Initialize(name, serverAddress, serverPort, userName, userPassword);
        }

        /// <summary>
        /// The Pi system 
        /// </summary>
        public HttpSystem PiSystem { get { return _httpSystem; } }

        /// <summary>
        /// The HygroThermoSensor 
        /// </summary>
        public HttpHygroThermoSensor HygroThermoSensor { get { return _httpHygroThermoSensor; } }

        private void Initialize(string name, string serverAddress, int serverPort, string userName, string userPassword)
        {
            _httpIotClient = new HttpIotClient();
            _httpIotClient.SetServer(serverAddress, serverPort);
            _httpIotClient.SetCredential(userName, userPassword);

            // discover the capabilities the server offers
            IList<HttpEndpoint> endpoints = _httpIotClient.DiscoverAvailableEndpoints();
            var sysEndpoints = endpoints.Where((HttpEndpoint item) => string.Equals(item.Name, "PiSystem", StringComparison.OrdinalIgnoreCase));
            if (sysEndpoints.Count() > 0)
            {
                string path = sysEndpoints[0].Path;
                _httpSystem = new HttpSystem(path, name, _httpIotClient, null);
            }
            var dhtEndpoints = endpoints.Where((HttpEndpoint item) => string.Equals(item.Name, "HygroThermoSensor", StringComparison.OrdinalIgnoreCase));
            if (dhtEndpoints.Count() > 0)
            {
                string path = dhtEndpoints[0].Path;
                _httpHygroThermoSensor = new HttpHygroThermoSensor(path, "HygroThermoSensor", _httpIotClient, null);
            }
        }

        private HttpIotClient _httpIotClient;
        private HttpSystem _httpSystem;
        private HttpHygroThermoSensor _httpHygroThermoSensor;
    }
}
