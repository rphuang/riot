using Devices.Models;
using Devices.Services;
using FormsLib;
using Riot;
using Riot.SmartPlug;
using Riot.SmartPlug.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Grid;

namespace Devices.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty("TopicId", "id")]
    public partial class Hs1xxPage : ContentPage
    {
        /// <summary>
        /// Topic ID passed from routing if available
        /// </summary>
        public string TopicId { get; set; }

        public Hs1xxPage()
        {
            DeviceTopic topic = (Shell.Current as AppShell).CurrentTopic as DeviceTopic;
            if (!string.IsNullOrEmpty(TopicId))
            {
                Topic item;
                if (Topics.TryGetTopic(TopicId, out item)) topic = item as DeviceTopic;
            }
            InitializeComponent();
            Title = topic.Name;
            _DeviceTopic = topic;
        }

        public Hs1xxPage(DeviceTopic topic)
        {
            InitializeComponent();
            Title = topic.Name;
            _DeviceTopic = topic;
        }

        protected override void OnAppearing()
        {
            Initialize();
            Display();
        }

        protected override void OnDisappearing()
        {
        }

        private void Initialize()
        {
            if (_initialized) return;

            DiscoverService discoverService = DiscoverService.GetOrCreateService(_DeviceTopic.Server, _DeviceTopic.Credential);
            IList<KasaHs1xxClient> nodes = discoverService.GetClientNodes<KasaHs1xxClient>();
            _hs1xxClient = nodes?.Where((KasaHs1xxClient item) => string.Equals(item.Id, _DeviceTopic.Path, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            _initialized = true;
        }

        private void Display()
        {
            IGridList<View> gridList = dataGrid.Children;
            gridList.Clear();
            int rowIndex = 0;

            if (_hs1xxClient == null)
            {
                AddLabel($"Smart Plug {_DeviceTopic.Name} is not available.", gridList, rowIndex++, 0, 2);
                _initialized = false;
                return;
            }

            HttpResponse response = _hs1xxClient.System.GetResponse();
            if (!response.Success)
            {
                AddLabel($"Error getting data for {_DeviceTopic.Name}", gridList, rowIndex++, 0);
                GridUtil.AddLabel(response.ErrorMessage, gridList, rowIndex++, 0, 2, 10, Color.Default, false, LayoutOptions.End);
                return;
            }

            string timeJson = _hs1xxClient.Time.Get();
            KasaHs1xxSystemData systemData = _hs1xxClient.System.SystemData;
            KasaHs1xxTimeData timeData = _hs1xxClient.Time.TimeData;

            if (systemData.Relay_state == 0) OnOffToolbarItem.Text = "On";
            else OnOffToolbarItem.Text = "Off";
            if (systemData.Led_off == 0) LedOffToolbarItem.Text = "LedOff";
            else LedOffToolbarItem.Text = "LedOn";

            DisplayHeaderAndValueRow("Alias", systemData.Alias, gridList, rowIndex++);
            DisplayHeaderAndValueRow("Time", timeData.Time, gridList, rowIndex++);
            DisplayHeaderAndValueRow("Relay State", systemData.Relay_state == 1 ? "On" : "Off", gridList, rowIndex++);
            DisplayHeaderAndValueRow("On Time", SecondToString(systemData.On_time), gridList, rowIndex++);
            DisplayHeaderAndValueRow("LED Always Off", systemData.Led_off == 1 ? "True" : "False", gridList, rowIndex++);
            DisplayHeaderAndValueRow("Next Action", systemData.next_action.Action.ToString(), gridList, rowIndex++);
            DisplayHeaderAndValueRow("Next Action Time", SecondToString(systemData.next_action.schd_sec), gridList, rowIndex++);
            DisplayHeaderAndValueRow("Next Action Type", systemData.next_action.Type.ToString(), gridList, rowIndex++);
            DisplayHeaderAndValueRow("WIFI Signal", systemData.Rssi.ToString(), gridList, rowIndex++);
            DisplayHeaderAndValueRow("Active Mode", systemData.Active_mode, gridList, rowIndex++);
            DisplayHeaderAndValueRow("Device Name", systemData.Dev_name, gridList, rowIndex++);
            DisplayHeaderAndValueRow("Error Code", systemData.Err_code.ToString(), gridList, rowIndex++);
            DisplayHeaderAndValueRow("Feature", systemData.Feature, gridList, rowIndex++);
            DisplayHeaderAndValueRow("HW Version", systemData.hw_ver, gridList, rowIndex++);
            DisplayHeaderAndValueRow("Icon Hash", systemData.Icon_hash, gridList, rowIndex++);
            DisplayHeaderAndValueRow("Latitude", systemData.Latitude_i.ToString(), gridList, rowIndex++);
            DisplayHeaderAndValueRow("Longitude", systemData.Longitude_i.ToString(), gridList, rowIndex++);
            DisplayHeaderAndValueRow("Mac Address", systemData.Mac, gridList, rowIndex++);
            DisplayHeaderAndValueRow("NTC State", systemData.Ntc_state.ToString(), gridList, rowIndex++);
            DisplayHeaderAndValueRow("Software Version", systemData.Sw_ver, gridList, rowIndex++);
            DisplayHeaderAndValueRow("Status", systemData.Status, gridList, rowIndex++);
            DisplayHeaderAndValueRow("Updating", systemData.Updating.ToString(), gridList, rowIndex++);
            // display IDs - 1st row for label 2nd row for data
            AddLabel("Device ID", gridList, rowIndex++, 0);
            GridUtil.AddLabel(systemData.DeviceId, gridList, rowIndex++, 0, 2, 1, Color.Default, false, LayoutOptions.End);
            AddLabel("Hardware ID", gridList, rowIndex++, 0);
            GridUtil.AddLabel(systemData.HwId, gridList, rowIndex++, 0, 2, 1, Color.Default, false, LayoutOptions.End);
            AddLabel("OEM ID", gridList, rowIndex++, 0);
            GridUtil.AddLabel(systemData.OemId, gridList, rowIndex++, 0, 2, 1, Color.Default, false, LayoutOptions.End);
            AddLabel("Next Action ID", gridList, rowIndex++, 0);
            GridUtil.AddLabel(systemData.next_action.Id, gridList, rowIndex++, 0, 2, 1, Color.Default, false, LayoutOptions.End);
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

        private string SecondToString(int seconds)
        {
            int hour = seconds / 3600;
            int minute = (seconds - 3600 * hour) / 60;
            int second = seconds - 3600 * hour - minute * 60;
            return $"{hour}:{minute:00}:{second:00}";
        }

        private bool _initialized;
        private DeviceTopic _DeviceTopic;
        private KasaHs1xxClient _hs1xxClient;

        private void OnOffToolbarItem_Clicked(object sender, EventArgs e)
        {
            string rawJson = _hs1xxClient.System.Get();
            KasaHs1xxSystemData systemData = _hs1xxClient.System.SystemData;
            if (systemData.Relay_state == 0)
            {
                _hs1xxClient.System.TurnPlugOnOff(true);
                OnOffToolbarItem.Text = "Off";
            }
            else
            {
                _hs1xxClient.System.TurnPlugOnOff(false);
                OnOffToolbarItem.Text = "On";
            }
            Display();
        }

        private void LedToolbarItem_Clicked(object sender, EventArgs e)
        {
            string rawJson = _hs1xxClient.System.Get();
            KasaHs1xxSystemData systemData = _hs1xxClient.System.SystemData;
            if (systemData.Led_off == 0)
            {
                _hs1xxClient.System.SetLedAlwaysOff(true);
                LedOffToolbarItem.Text = "Led On";
            }
            else
            {
                _hs1xxClient.System.SetLedAlwaysOff(false);
                LedOffToolbarItem.Text = "Led Off";
            }
            Display();
        }

        private void RefreshToolbarItem_Clicked(object sender, EventArgs e)
        {
            Display();
        }

        private void RebootToolbarItem_Clicked(object sender, EventArgs e)
        {
            _hs1xxClient.Reboot(1);
        }
    }
}