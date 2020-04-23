using IotClientLib;
using System;
using System.Collections.Generic;
using System.Text;

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

            _httpGpio = new HttpGpio("gpio", name, _httpIotClient, null);
        }

        private HttpIotClient _httpIotClient;
        private HttpGpio _httpGpio;
    }
}
