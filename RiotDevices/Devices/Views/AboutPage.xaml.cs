using Devices.Models;
using FormsLib;
using Riot.Phone.Service;
using System;
using System.ComponentModel;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Grid;

namespace Devices.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            Initialize();
            Start();
        }

        protected override void OnDisappearing()
        {
            Stop();
        }

        private void Initialize()
        {
            if (_initialized) return;

            _phoneService = PhoneService.Instance;
            _initialized = true;
        }

        private async void UpdateDisplayAsync()
        {
            while (_updateDisplay)
            {
                try
                {
                    Display();
                }
                catch (Exception err)
                {
                    // todo: debug exception
                }
                await Task.Run(async () =>
                {
                    int delay = (int)(DeviceSettings.Instance.MonitorRefreshRate * 1000);
                    await Task.Delay(delay);   // in milliseconds
                }).ConfigureAwait(true);
            }
        }

        private void Display()
        {
            IGridList<View> gridList = dataGrid.Children;
            gridList.Clear();
            int rowIndex = 0;

            DisplayHeaderAndValueRow("Device Name", _phoneService.DeviceInfo.Name, gridList, rowIndex++);
            string display = $"{_phoneService.DeviceInfo.Height:0.} x {_phoneService.DeviceInfo.Width:0.}";
            DisplayHeaderAndValueRow("Display", display, gridList, rowIndex++);
            DisplayHeaderAndValueRow("Version", _phoneService.DeviceInfo.VersionString, gridList, rowIndex++);

            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            DisplayHeaderAndValueRow("Host Name", hostName, gridList, rowIndex++);
            // Get the IP  
            var entry = Dns.GetHostEntry(hostName);
            string myIP = entry.AddressList[0].ToString();
            DisplayHeaderAndValueRow("IP Address", myIP, gridList, rowIndex++);

            try
            {
                DisplayHeaderAndValueRow("Acceleration", ToText(_phoneService.Accelerometer.Acceleration.Value), gridList, rowIndex++);
                DisplayHeaderAndValueRow("Pressure", ToText(_phoneService.Barometer.Pressure.Value), gridList, rowIndex++);
                DisplayHeaderAndValueRow("Heading", ToText(_phoneService.Compass.Heading.Value), gridList, rowIndex++);
                DisplayHeaderAndValueRow("Angular Velocity", ToText(_phoneService.Gyroscope.AngularVelocity.Value), gridList, rowIndex++);
                DisplayHeaderAndValueRow("Magnetic Field", ToText(_phoneService.Magnetometer.MagneticField.Value), gridList, rowIndex++);
                DisplayHeaderAndValueRow("Orientation", ToText(_phoneService.OrientationSensor.Orientation.Value), gridList, rowIndex++);
                Location location = _phoneService.GpsLocation.Location.Value;
                DisplayHeaderAndValueRow("Latitude", location != null? ToTextLong(location.Latitude) : string.Empty, gridList, rowIndex++);
                DisplayHeaderAndValueRow("Longitude", location != null ? ToTextLong(location.Longitude) : string.Empty, gridList, rowIndex++);
                DisplayHeaderAndValueRow("Altitude", location != null ? ToText(location.Altitude) : string.Empty, gridList, rowIndex++);
                DisplayHeaderAndValueRow("Accuracy", location != null ? ToText(location.Accuracy) : string.Empty, gridList, rowIndex++);
                DisplayHeaderAndValueRow("Speed", location != null ? ToText(location.Speed) : string.Empty, gridList, rowIndex++);
            }
            catch (Exception err)
            {
                rowIndex++;
                DisplayHeaderAndValueRow("Exception", "Check for permissions", gridList, rowIndex++);
            }
        }

        private void DisplayHeaderAndValueRow(string header, string value, IGridList<View> gridList, int rowIndex)
        {
            AddLabel(header, gridList, rowIndex, 0);
            AddLabel(value.ToString(), gridList, rowIndex, 1);
        }

        private void AddLabel(string text, IGridList<View> gridList, int row, int col, int colSpan = 1)
        {
            Label label = GridUtil.AddLabel(text, gridList, row, col, colSpan);
        }

        private string ToText(Vector3 value)
        {
            return $"{value.X:0.000}, {value.Y:0.000}, {value.Z:0.000}";
        }
        private string ToText(Quaternion value)
        {
            return $"{value.X:0.000}, {value.Y:0.000}, {value.Z:0.000}, {value.W:0.000}";
        }
        private string ToText(double value)
        {
            return $"{value:0.0000}";
        }
        private string ToTextLong(double value)
        {
            return $"{value:0.000000}";
        }
        private string ToText(double? value)
        {
            return value == null? string.Empty : $"{value:0.0000}";
        }

        private void Stop()
        {
            _updateDisplay = false;
            _phoneService.StopAll();
            StartStopButton.Text = "Start";
        }

        private void Start()
        {
            Initialize();
            _phoneService.StartAll();
            StartStopButton.Text = "Stop";
            _updateDisplay = true;
            UpdateDisplayAsync();
        }

        private void AbortRefresh()
        {
            Stop();
            _initialized = false;
        }

        private bool _initialized;
        private bool _updateDisplay;
        private PhoneService _phoneService;

        private void StartStopButton_Clicked(object sender, EventArgs e)
        {
            if (_updateDisplay) Stop();
            else Start();
        }
    }
}