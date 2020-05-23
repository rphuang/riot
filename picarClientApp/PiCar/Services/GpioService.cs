using IotClientLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PiCar.Services
{
    /// <summary>
    /// service for the Gpio 
    /// </summary>
    public class GpioService
    {
        /// <summary>
        /// constructor
        /// </summary>
        public GpioService(string name, string serverAddress, int serverPort, string userName, string userPassword)
        {
            Initialize(name, serverAddress, serverPort, userName, userPassword);
        }

        /// <summary>
        /// The Gpio 
        /// </summary>
        public HttpGpio Gpio { get { return _httpGpio; } }

        private void Initialize(string name, string serverAddress, int serverPort, string userName, string userPassword)
        {
            _httpIotClient = new HttpIotClient();
            _httpIotClient.SetServer(serverAddress, serverPort);
            _httpIotClient.SetCredential(userName, userPassword);

            // discover the gpio path from the server
            string path = "gpio";
            IList<HttpEndpoint> endpoints = _httpIotClient.DiscoverAvailableEndpoints();
            var gpioEndpoints = endpoints.Where((HttpEndpoint item) => string.Equals(item.Name, "PiGpio", StringComparison.OrdinalIgnoreCase));
            if (gpioEndpoints.Count() > 0)
            {
                path = gpioEndpoints.First().Path;
            }
            _httpGpio = new HttpGpio(path, name, _httpIotClient, null);
        }

        private HttpIotClient _httpIotClient;
        private HttpGpio _httpGpio;
    }
}
