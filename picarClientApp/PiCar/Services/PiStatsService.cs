using IotClientLib;
using System;
using System.Collections.Generic;
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

        private void Initialize(string name, string serverAddress, int serverPort, string userName, string userPassword)
        {
            _httpIotClient = new HttpIotClient();
            _httpIotClient.SetServer(serverAddress, serverPort);
            _httpIotClient.SetCredential(userName, userPassword);

            _httpSystem = new HttpSystem(name, _httpIotClient, null);
        }

        private HttpIotClient _httpIotClient;
        private HttpSystem _httpSystem;
    }
}
