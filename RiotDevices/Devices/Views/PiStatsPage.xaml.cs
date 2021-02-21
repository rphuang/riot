using Devices.Models;
using Devices.Services;
using FormsLib;
using Riot;
using Riot.IoDevice.Client;
using Riot.Pi;
using Riot.Pi.Client;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Grid;

namespace Devices.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty("TopicId", "id")]
    public partial class PiStatsPage : ContentPage
    {
        /// <summary>
        /// Topic ID passed from routing if available
        /// </summary>
        public string TopicId { get; set; }

        public PiStatsPage()
        {
            Topic topic;
            if (!Topics.TryGetTopic(TopicId, out topic))
            {
                topic = (Shell.Current as AppShell).CurrentTopic;
            }
            InitializeComponent();
            Title = topic.Name;
            _monitorTopic = topic;
        }

        public PiStatsPage(Topic topic)
        {
            InitializeComponent();
            Title = topic.Name;
            _monitorTopic = topic;
        }

        protected override void OnAppearing()
        {
            Initialize();
            StartRefresh();
        }

        protected override void OnDisappearing()
        {
            StopRefresh();
        }

        private void Initialize()
        {
            if (_initialized) return;

            _discoverService = DiscoverService.GetOrCreateService(_monitorTopic.Server, _monitorTopic.Credential);
            _piSystem = _discoverService.GetClientNode<SystemClient>();
            _hygroThermoSensor = _discoverService.GetClientNode<HygroThermoSensorClient>();
            _initialized = true;
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
            IGridList<View> gridList = piStatsGrid.Children;
            gridList.Clear();
            int rowIndex = 0;

            if (_piSystem == null)
            {
                AddLabel($"Service {_monitorTopic.Name} is not available.", gridList, rowIndex++, 0, 5);
                AbortRefresh();
                return;
            }

            HttpResponse response = _piSystem.CpuClient.GetResponse();
            if (!response.Success)
            {
                AddLabel($"Error getting data for {_monitorTopic.Name}", gridList, rowIndex++, 0);
                GridUtil.AddLabel(response.ErrorMessage, gridList, rowIndex++, 0, 5, 10, Color.Default, false, LayoutOptions.End);
                StopRefresh();
                return;
            }
            _piSystem.MemoryClient.Get();
            CpuData cpu = _piSystem.CpuClient.CpuData;
            MemoryData memory = _piSystem.MemoryClient.MemoryData;

            DisplayHeaderAndValueRow("Server Address", _monitorTopic.Server, gridList, rowIndex++, 3);
            DisplayHeaderAndValueRow("CPU Core Temperature", cpu.Temperature.ToString(), gridList, rowIndex++, 3);
            AddLabel("Usage", gridList, rowIndex, 1, 1);
            AddLabel("User", gridList, rowIndex, 2, 1);
            AddLabel("System", gridList, rowIndex, 3, 1);
            AddLabel("Idle", gridList, rowIndex, 4, 1);
            rowIndex++;
            DisplayCpuRow(_monitorTopic.Name, cpu, gridList, rowIndex++);
            if (cpu.Cores != null)
            {
                foreach (var data in cpu.Cores)
                {
                    DisplayCpuRow(data.Key, data.Value, gridList, rowIndex++);
                }
            }
            DisplayHeaderAndValueRow("Total Memory (MB)", memory.Total.ToString(), gridList, rowIndex++, 2);
            DisplayMemoryRow("Used Memory (MB)", memory.Used, memory.Total, gridList, rowIndex++);
            DisplayMemoryRow("Cached Memory (MB)", memory.Cached, memory.Total, gridList, rowIndex++);
            DisplayMemoryRow("Free Memory (MB)", memory.Free, memory.Total, gridList, rowIndex++);
            DisplayMemoryRow("Available Memory (MB)", memory.Available, memory.Total, gridList, rowIndex++);

            if (_hygroThermoSensor != null)
            {
                string json = _hygroThermoSensor.Get();
                DisplayHeaderAndValueRow("Humidity (%)", _hygroThermoSensor.HygroThermoData.Humidity.ToString(), gridList, rowIndex++, 2);
                DisplayHeaderAndValueRow("Temperature (C)", _hygroThermoSensor.HygroThermoData.Temperature.ToString(), gridList, rowIndex++, 2);
            }
        }

        private void DisplayCpuRow(string id, CpuData cpu, IGridList<View> gridList, int rowIndex)
        {
            AddLabel(id, gridList, rowIndex, 0, 1);
            AddLabel(cpu.Usage.ToString(), gridList, rowIndex, 1, 1);
            AddLabel(cpu.UserUsage.ToString(), gridList, rowIndex, 2, 1);
            AddLabel(cpu.SystemUsage.ToString(), gridList, rowIndex, 3, 1);
            AddLabel(cpu.Idle.ToString(), gridList, rowIndex, 4, 1);
        }

        private void DisplayMemoryRow(string header, int value, int total, IGridList<View> gridList, int rowIndex)
        {
            AddLabel(header, gridList, rowIndex, 0, 2);
            AddLabel(value.ToString(), gridList, rowIndex, 2, 1);
            string percent = string.Format("{0:0.00}%", (100.0 * value / total));
            AddLabel(percent, gridList, rowIndex, 3, 1);
        }

        private void DisplayHeaderAndValueRow(string header, string value, IGridList<View> gridList, int rowIndex, int valueColumn = 2)
        {
            AddLabel(header, gridList, rowIndex, 0, valueColumn);
            AddLabel(value.ToString(), gridList, rowIndex, valueColumn, 2);
        }

        private void AddLabel(string text, IGridList<View> gridList, int row, int col, int colSpan = 1)
        {
            Label label = GridUtil.AddLabel(text, gridList, row, col, colSpan);
        }

        private void StopRefresh()
        {
            _updatePiStats = false;
            StartStopButton.Text = "Start";
        }

        private void StartRefresh()
        {
            Initialize();
            StartStopButton.Text = "Stop";
            _updatePiStats = true;
            UpdateDisplayAsync();
        }

        private void AbortRefresh()
        {
            StopRefresh();
            _initialized = false;
        }

        private bool _initialized;
        private Topic _monitorTopic;
        private DiscoverService _discoverService;
        private bool _updatePiStats;
        private SystemClient _piSystem;
        private HygroThermoSensorClient _hygroThermoSensor;

        private void StartStopButton_Clicked(object sender, EventArgs e)
        {
            if (_updatePiStats) StopRefresh();
            else StartRefresh();
        }

        async private void GpioButton_Clicked(object sender, EventArgs e)
        {
            StopRefresh();
            GpioPage page = new GpioPage(_monitorTopic);
            await Navigation.PushModalAsync(new NavigationPage(page));
        }

        async private void EditButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new MonitorTopicPage(_monitorTopic)));
        }

        async private void CommandButton_Clicked(object sender, EventArgs e)
        {
            StopRefresh();
            await Navigation.PushModalAsync(new NavigationPage(new PiCommandPage(_monitorTopic, _piSystem)));
        }
    }
}