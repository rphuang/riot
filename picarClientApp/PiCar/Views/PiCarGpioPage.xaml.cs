using IotClientLib;
using PiCar.Models;
using PiCar.Services;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Grid;

namespace PiCar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PiCarGpioPage : ContentPage
    {
        public PiCarGpioPage(MonitorTopic topic)
        {
            InitializeComponent();
            _monitorTopic = topic;
            Title = _monitorTopic.Name;
            _gpioService = new GpioService(topic.Name, topic.ServerAddress, topic.ServerPort, topic.UserName, topic.UserPassword);
            _updatePiStats = true;
            StartUpdate();
        }

        private async void UpdateDisplayAsync()
        {
            while (_updatePiStats)
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
                    int delay = (int)(Settings.Instance.MonitorRefreshRate * 1000);
                    await Task.Delay(delay);   // in milliseconds
                }).ConfigureAwait(true);
            }
        }

        private void Display()
        {
            HttpGpio gpio = _gpioService.Gpio;
            gpio.Get();

            IGridList<View> gridList = piGpioGrid.Children;
            foreach (IotGpioPin pin in gpio.Pins)
            {
                DisplayPinMode(pin);
                DisplayPinValue(pin);
            }
            //gridList.Clear();
            //int rowIndex = 0;

            //AddHeaders(gridList, rowIndex++);
        }

        private void DisplayPinMode(IotGpioPin pin)
        {
            Label label = null;
            switch (pin.Pin)
            {
                case "3":
                    label = pin3Mode;
                    break;
                case "5":
                    label = pin5Mode;
                    break;
                case "7":
                    label = pin7Mode;
                    break;
                case "8":
                    label = pin8Mode;
                    break;
                case "10":
                    label = pin10Mode;
                    break;
                case "11":
                    label = pin11Mode;
                    break;
                case "12":
                    label = pin12Mode;
                    break;
                case "13":
                    label = pin13Mode;
                    break;
                case "15":
                    label = pin15Mode;
                    break;
                case "16":
                    label = pin16Mode;
                    break;
                case "18":
                    label = pin18Mode;
                    break;
                case "19":
                    label = pin19Mode;
                    break;
                case "21":
                    label = pin21Mode;
                    break;
                case "22":
                    label = pin22Mode;
                    break;
                case "23":
                    label = pin23Mode;
                    break;
                case "24":
                    label = pin24Mode;
                    break;
                case "26":
                    label = pin26Mode;
                    break;
                case "27":
                    label = pin27Mode;
                    break;
                case "28":
                    label = pin28Mode;
                    break;
                case "29":
                    label = pin29Mode;
                    break;
                case "31":
                    label = pin31Mode;
                    break;
                case "32":
                    label = pin32Mode;
                    break;
                case "33":
                    label = pin33Mode;
                    break;
                case "35":
                    label = pin35Mode;
                    break;
                case "36":
                    label = pin36Mode;
                    break;
                case "37":
                    label = pin37Mode;
                    break;
                case "38":
                    label = pin38Mode;
                    break;
                case "40":
                    label = pin40Mode;
                    break;
            }
            if (label != null)
            {
                string mode = "IN";
                if (pin.Mode == 0) mode = "OUT";
                label.Text = mode;
            }
        }

        private void DisplayPinValue(IotGpioPin pin)
        {
            Label label = null;
            switch (pin.Pin)
            {
                case "3":
                    label = pin3Value;
                    break;
                case "5":
                    label = pin5Value;
                    break;
                case "7":
                    label = pin7Value;
                    break;
                case "8":
                    label = pin8Value;
                    break;
                case "10":
                    label = pin10Value;
                    break;
                case "11":
                    label = pin11Value;
                    break;
                case "12":
                    label = pin12Value;
                    break;
                case "13":
                    label = pin13Value;
                    break;
                case "15":
                    label = pin15Value;
                    break;
                case "16":
                    label = pin16Value;
                    break;
                case "18":
                    label = pin18Value;
                    break;
                case "19":
                    label = pin19Value;
                    break;
                case "21":
                    label = pin21Value;
                    break;
                case "22":
                    label = pin22Value;
                    break;
                case "23":
                    label = pin23Value;
                    break;
                case "24":
                    label = pin24Value;
                    break;
                case "26":
                    label = pin26Value;
                    break;
                case "27":
                    label = pin27Value;
                    break;
                case "28":
                    label = pin28Value;
                    break;
                case "29":
                    label = pin29Value;
                    break;
                case "31":
                    label = pin31Value;
                    break;
                case "32":
                    label = pin32Value;
                    break;
                case "33":
                    label = pin33Value;
                    break;
                case "35":
                    label = pin35Value;
                    break;
                case "36":
                    label = pin36Value;
                    break;
                case "37":
                    label = pin37Value;
                    break;
                case "38":
                    label = pin38Value;
                    break;
                case "40":
                    label = pin40Value;
                    break;
            }
            if (label != null)
            {
                label.Text = pin.Value.ToString();
            }
        }

        private void StopUpdate()
        {
            _updatePiStats = false;
            StartStopGpioButton.Text = "Start";
        }

        private void StartUpdate()
        {
            StartStopGpioButton.Text = "Stop";
            _updatePiStats = true;
            UpdateDisplayAsync();
        }

        private MonitorTopic _monitorTopic;
        private GpioService _gpioService;
        private bool _updatePiStats;

        async private void ExitGpio_Clicked(object sender, EventArgs e)
        {
            StopUpdate();
            await Navigation.PopModalAsync();
        }

        private void StartStopGpioButton_Clicked(object sender, EventArgs e)
        {
            if (_updatePiStats) StopUpdate();
            else StartUpdate();
        }

        async private void ExportGpio_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}