using Devices.Models;
using HttpLib;
using Riot;
using Riot.Phone.Service;
using System;
using System.Threading.Tasks;

namespace Devices.Services
{
    /// <summary>
    /// service host for the app
    /// </summary>
    class AppServiceHost : PhoneServiceHost
    {
        /// <summary>
        /// constructor
        /// </summary>
        public AppServiceHost(string serverPrefix, string rootPath, string actionRootPath, string credentials)
            : base(serverPrefix, rootPath, actionRootPath, credentials)
        {
        }

        /// <summary>
        /// initialize 
        /// </summary>
        public override HttpServiceHost Init()
        {
            _quitRequestHandler = new QuitRequestHandler(_rootPath + "/quit", _credentials);
            return base.Init();
        }

        /// <summary>
        /// start all handlers
        /// </summary>
        public override void Start()
        {
            base.Start();
            _active = true;
        }

        /// <summary>
        /// start all handlers
        /// </summary>
        public override void Stop()
        {
            _active = false;
            base.Stop();
        }

        /// <summary>
        /// ProcessRequests runs async in separate thread until Stop() or received a quit request
        /// The IResponseHandler will be called on the original calling thread after each request processed
        /// </summary>
        public async void ProcessRequestsAsync(IResponseHandler handler)
        {
            while (_active)
            {
                HttpServiceResponse hostResponse = null;
                await Task.Run(async () =>
                {
                    try
                    {
                        hostResponse = await GetContextAsync();
                    }
                    catch (Exception err)
                    {
                    }
                }).ConfigureAwait(true);
                // calls handler to process the response
                handler?.Process(hostResponse);
                // stop the loop if quit requested
                if (_quitRequestHandler.GetAndResetQuitRequested)
                {
                    break;
                }
            }
            Stop();
        }

        /// <summary>
        /// invoke this method to exit the loop of ProcessRequestsAsync
        /// </summary>
        public void QuitProcessRequests()
        {
            string path = "quit";
            if (!string.IsNullOrEmpty(_rootPath)) path = $"{_rootPath}/{path}";
            SendSelfRequest(path, null);
        }

        /// <summary>
        /// send a request to the host itself
        /// </summary>
        /// <param name="path"></param>
        /// <param name="json"></param>
        public void SendSelfRequest(string path, string json)
        {
            if (_client == null)
            {
                _port = GetServicePort();
                DeviceSettings settings = DeviceSettings.Instance;
                string credential = settings.ServiceCredentials;
                if (!string.IsNullOrEmpty(credential))
                {
                    string[] parts = credential.Split(CommaDelimiter);
                    credential = parts[0];
                }
                _client = new IotHttpClient($"localhost{_port}", credential);
            }
            if (string.IsNullOrEmpty(json)) _client.GetResponse(path);
            else _client.Post(path, json);
        }

        /// <summary>
        /// get the service port in ":nnnn"
        /// </summary>
        public string GetServicePort()
        {
            DeviceSettings settings = DeviceSettings.Instance;
            string port;
            int index = settings.ServerPrefix.IndexOf(':', 6);  // skip http: or https:
            if (index > 1) port = settings.ServerPrefix.Substring(index);
            else port = string.Empty;
            return port;
        }

        private bool _active;
        private QuitRequestHandler _quitRequestHandler;
        private string _port;
        private IotHttpClient _client;
        private static readonly char[] CommaDelimiter = { ',' };
    }
}
