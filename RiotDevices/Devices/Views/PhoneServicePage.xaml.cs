using Devices.Models;
using HttpLib;
using Riot;
using Riot.Phone.Service;
using System;
using System.Net;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Devices.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneServicePage : ContentPage // BasePage
    {
        public PhoneServicePage()
        {
            InitializeComponent();
            Title = "Phone Service";
        }

        private PhoneServiceHost _host;
        private bool _initialized;
        private bool _active;
        private string _port;
        private IotHttpClient _client;
        private static readonly char[] CommaDelimiter = { ',' };

        protected override void OnAppearing()
        {
            Initialize();
            // temporary: make sure all sensors are started 
            if (_active) _host.Start();
        }

        protected override void OnDisappearing()
        {
            // todo: impl stop
            //Stop();
        }

        private void Initialize()
        {
            if (_initialized) return;

            DeviceSettings settings = DeviceSettings.Instance;
            _host = new PhoneServiceHost(settings.ServerPrefix, settings.ServiceRootPath, settings.ServiceActionRootPath, settings.ServiceCredentials);
            DisplayIpAddress();
            _host.Init();
            _initialized = true;
        }

        private void Start()
        {
            Initialize();
            _active = true;
            StartStopButton.Text = "Stop";
            // todo: enable stop when host support timeout
            StartStopButton.IsEnabled = false;
            string lsv = string.Join("\n", _host.Prefixes);
            TextLabel.Text = $"HttpHost started at {DateTime.Now} listening:\n{lsv}";
            _host.Start();
            GetContextAsync();
            ProcessActionAsync();
        }

        private void Stop()
        {
            _active = false;
            _host.Stop();
            StartStopButton.Text = "Start";
            TextLabel.Text = $"HttpHost stopped at {DateTime.Now} \n";
        }

        private async void GetContextAsync()
        {
            while (_active)
            {
                HttpServiceResponse hostResponse = null;
                await Task.Run(async () =>
                {
                    try
                    {
                        hostResponse = await _host.GetContextAsync();
                    }
                    catch (Exception err)
                    {
                    }
                }).ConfigureAwait(true);

                if (hostResponse != null)
                {
                    HttpListenerRequest request = hostResponse.Request;
                    HttpListenerResponse response = hostResponse.Response;
                    if (hostResponse.Success)
                    {
                        TextLabel.Text = $"{request.HttpMethod} {request.Url.AbsoluteUri} Status: {response.StatusCode} Result:\n{hostResponse.Content}";
                    }
                    else
                    {
                        TextLabel.Text = $"{request.HttpMethod} {request.Url.AbsoluteUri} Status: {response.StatusCode} Result:\n{hostResponse.Content}\n Error:{hostResponse.ErrorMessage}";
                    }
                }
            }
            _host.Stop();
        }

        private async void ProcessActionAsync()
        {
            while (true)
            {
                await Task.Run(async () =>
                {
                    await Task.Delay(500);   // in milliseconds
                }).ConfigureAwait(true);
                try
                {
                    UiActionBag.ActionDef action = UiActionBag.Instance.TryExecuteAction();
                    if (action != null)
                    {
                        TextLabel.Text = $"Processed action: {action.ActionService.Id}";
                    }
                }
                catch (Exception err)
                {
                    TextLabel.Text = $"Error processing action\n{err.ToString()}";
                }
            }
        }

        private void DisplayIpAddress()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            HostNameLabel.Text = hostName;
            // Get the IP  
            var entry = Dns.GetHostEntry(hostName);
            string myIP = entry.AddressList[0].ToString();
            AddressLabel.Text = myIP;
            //var ip = Dns.GetHostAddresses(hostName);
            RootPrefixLabel.Text = _host.ServerPrefix;
        }

        private void StartStopButton_Clicked(object sender, EventArgs e)
        {
            if (_active) Stop();
            else Start();
        }

        private async void TestButton_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Send test message", "Enter path with optional json string for post:");
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Trim();
                string path = result;
                string json = null;
                int index = result.IndexOf(" ");
                if (index > 0)
                {
                    path = result.Substring(0, index);
                    json = result.Substring(index + 1);
                }

                if (_client == null)
                {
                    DeviceSettings settings = DeviceSettings.Instance;
                    index = settings.ServerPrefix.IndexOf(':', 6);  // skip http: or https:
                    if (index > 1) _port = settings.ServerPrefix.Substring(index);
                    else _port = string.Empty;
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
        }
    }
}