using Devices.Models;
using Devices.Services;
using FormsLib;
using Riot.Pi;
using Riot.Pi.Client;
using SettingsLib;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Grid;

namespace Devices.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GpioPage : ContentPage
    {
        public GpioPage(Topic topic)
        {
            InitializeComponent();
            _monitorTopic = topic;
            Title = _monitorTopic.Name;

            DiscoverService discoverService = DiscoverService.GetOrCreateService(topic.Server, topic.Credential);
            _gpio = discoverService.GetClientNode<GpioClient>();

            _pinNameLabels = new Label[] { pin1Name, pin2Name, pin3Name, pin4Name, pin5Name, pin6Name, pin7Name, pin8Name, pin9Name, pin10Name,
                pin11Name, pin12Name, pin13Name, pin14Name, pin15Name, pin16Name, pin17Name, pin18Name, pin19Name, pin20Name,
                pin21Name, pin22Name, pin23Name, pin24Name, pin25Name, pin26Name, pin27Name, pin28Name, pin29Name, pin30Name,
                pin31Name, pin32Name, pin33Name, pin34Name, pin35Name, pin36Name, pin37Name, pin38Name, pin39Name, pin40Name };
            CustomInitialize();
            _updatePiStats = true;
            StartUpdate();
        }

        protected override void OnAppearing()
        {
            //StartUpdate();
        }

        protected override void OnDisappearing()
        {
            StopUpdate();
        }

        protected virtual void CustomInitialize()
        {
            // custom pin name is defined in the setting group, the group ID is defined in property GpioPinGroup
            if (_monitorTopic.HasProperty("GpioPinGroup"))
            {
                SettingGroup group;
                if (DeviceSettings.Instance.TryGetSettingGroup(_monitorTopic["GpioPinGroup"].ToString(), out group))
                {
                    foreach (var item in group.Settings)
                    {
                        try
                        {
                            DisplayPinName(int.Parse(item.Key), item.Value);
                        }
                        catch { }
                    }
                }
            }
        }

        protected void DisplayPinName(int pin, string name)
        {
            Label label = _pinNameLabels[pin-1];
            if (label != null)
            {
                label.Text = name;
            }
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
                    int delay = (int)(DeviceSettings.Instance.MonitorRefreshRate * 1000);
                    await Task.Delay(delay);   // in milliseconds
                }).ConfigureAwait(true);
            }
        }

        private void Display()
        {
            _gpio.Get();

            IGridList<View> gridList = piGpioGrid.Children;
            foreach (GpioPinData pin in _gpio.GpioData.Pins)
            {
                DisplayPinMode(pin);
                DisplayPinValue(pin);
            }
            //gridList.Clear();
            //int rowIndex = 0;

            //AddHeaders(gridList, rowIndex++);
        }

        private void AddHeaders(IGridList<View> gridList, int rowIndex)
        {
            GridUtil.AddLabel("Pin#", gridList, rowIndex, 0, 0, 1, Color.Blue, false, LayoutOptions.Start);
            GridUtil.AddLabel("Mode", gridList, rowIndex, 0, 1, 1, Color.Blue, false, LayoutOptions.Start);
            GridUtil.AddLabel("Value", gridList, rowIndex, 0, 2, 1, Color.Blue, false, LayoutOptions.Start);
            GridUtil.AddLabel("Value", gridList, rowIndex, 0, 3, 1, Color.Blue, false, LayoutOptions.Start);
            GridUtil.AddLabel("Mode", gridList, rowIndex, 0, 4, 1, Color.Blue, false, LayoutOptions.Start);
            GridUtil.AddLabel("Pin#", gridList, rowIndex, 0, 5, 1, Color.Blue, false, LayoutOptions.Start);
        }

        private void AddLabel(string text, IGridList<View> gridList, int row, int col, int colSpan = 1)
        {
            GridUtil.AddLabel("Pin#", gridList, row, 0, 1, 1, Color.Blue, false, LayoutOptions.Start);
            Label label = GridUtil.AddLabel(text, gridList, row, col, colSpan);
        }

        private void DisplayPinMode(GpioPinData pin)
        {
            Label label = null;
            switch (pin.Pin)
            {
                case 3:
                    label = pin3Mode;
                    break;
                case 5:
                    label = pin5Mode;
                    break;
                case 7:
                    label = pin7Mode;
                    break;
                case 8:
                    label = pin8Mode;
                    break;
                case 10:
                    label = pin10Mode;
                    break;
                case 11:
                    label = pin11Mode;
                    break;
                case 12:
                    label = pin12Mode;
                    break;
                case 13:
                    label = pin13Mode;
                    break;
                case 15:
                    label = pin15Mode;
                    break;
                case 16:
                    label = pin16Mode;
                    break;
                case 18:
                    label = pin18Mode;
                    break;
                case 19:
                    label = pin19Mode;
                    break;
                case 21:
                    label = pin21Mode;
                    break;
                case 22:
                    label = pin22Mode;
                    break;
                case 23:
                    label = pin23Mode;
                    break;
                case 24:
                    label = pin24Mode;
                    break;
                case 26:
                    label = pin26Mode;
                    break;
                case 27:
                    label = pin27Mode;
                    break;
                case 28:
                    label = pin28Mode;
                    break;
                case 29:
                    label = pin29Mode;
                    break;
                case 31:
                    label = pin31Mode;
                    break;
                case 32:
                    label = pin32Mode;
                    break;
                case 33:
                    label = pin33Mode;
                    break;
                case 35:
                    label = pin35Mode;
                    break;
                case 36:
                    label = pin36Mode;
                    break;
                case 37:
                    label = pin37Mode;
                    break;
                case 38:
                    label = pin38Mode;
                    break;
                case 40:
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

        private void DisplayPinValue(GpioPinData pin)
        {
            Label label = null;
            switch (pin.Pin)
            {
                case 3:
                    label = pin3Value;
                    break;
                case 5:
                    label = pin5Value;
                    break;
                case 7:
                    label = pin7Value;
                    break;
                case 8:
                    label = pin8Value;
                    break;
                case 10:
                    label = pin10Value;
                    break;
                case 11:
                    label = pin11Value;
                    break;
                case 12:
                    label = pin12Value;
                    break;
                case 13:
                    label = pin13Value;
                    break;
                case 15:
                    label = pin15Value;
                    break;
                case 16:
                    label = pin16Value;
                    break;
                case 18:
                    label = pin18Value;
                    break;
                case 19:
                    label = pin19Value;
                    break;
                case 21:
                    label = pin21Value;
                    break;
                case 22:
                    label = pin22Value;
                    break;
                case 23:
                    label = pin23Value;
                    break;
                case 24:
                    label = pin24Value;
                    break;
                case 26:
                    label = pin26Value;
                    break;
                case 27:
                    label = pin27Value;
                    break;
                case 28:
                    label = pin28Value;
                    break;
                case 29:
                    label = pin29Value;
                    break;
                case 31:
                    label = pin31Value;
                    break;
                case 32:
                    label = pin32Value;
                    break;
                case 33:
                    label = pin33Value;
                    break;
                case 35:
                    label = pin35Value;
                    break;
                case 36:
                    label = pin36Value;
                    break;
                case 37:
                    label = pin37Value;
                    break;
                case 38:
                    label = pin38Value;
                    break;
                case 40:
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

        private Topic _monitorTopic;
        private GpioClient _gpio;
        private bool _updatePiStats;
        private Label[] _pinNameLabels;

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