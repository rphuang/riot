using FormsLib;
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
    public partial class PiStatsPage : ContentPage
    {
        public PiStatsPage(MonitorTopic topic)
        {
            InitializeComponent();
            Title = topic.Name;

            _monitorTopic = topic;
            _piStatsService = new PiStatsService(topic.Name, topic.ServerAddress, topic.ServerPort, topic.UserName, topic.UserPassword);
            StartRefresh();
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
            //_piStatsService.PiSystem.Get();
            IotCpu cpu = _piStatsService.PiSystem.Cpu;
            IotMemory memory = _piStatsService.PiSystem.Memory;
            cpu.Get();
            memory.Get();

            IGridList<View> gridList = piStatsGrid.Children;
            gridList.Clear();
            int rowIndex = 0;

            AddLabel("Server Address", gridList, rowIndex, 0, 2);
            AddLabel(_monitorTopic.Server, gridList, rowIndex, 3, 2);
            rowIndex++;
            AddLabel("Core Temperature", gridList, rowIndex, 0, 2);
            AddLabel(cpu.Temperature.ToString(), gridList, rowIndex, 3, 2);
            rowIndex++;
            AddLabel("Usage", gridList, rowIndex, 1, 1);
            AddLabel("User", gridList, rowIndex, 2, 1);
            AddLabel("System", gridList, rowIndex, 3, 1);
            AddLabel("Idle", gridList, rowIndex, 4, 1);
            rowIndex++;
            DisplayCpuRow(cpu, gridList, rowIndex);
            rowIndex++;
            if (cpu.Cores != null)
            {
                foreach (IotCpu core in cpu.Cores.Values)
                {
                    DisplayCpuRow(core, gridList, rowIndex);
                    rowIndex++;
                }
            }
            AddLabel("Total Memory (MB)", gridList, rowIndex, 0, 2);
            AddLabel(memory.Total.ToString(), gridList, rowIndex, 2, 2);
            rowIndex++;
            DisplayMemoryRow("Used Memory (MB)", memory.Used, memory.Total, gridList, rowIndex);
            rowIndex++;
            DisplayMemoryRow("Cached Memory (MB)", memory.Cached, memory.Total, gridList, rowIndex);
            rowIndex++;
            DisplayMemoryRow("Free Memory (MB)", memory.Free, memory.Total, gridList, rowIndex);
            rowIndex++;
            DisplayMemoryRow("Available Memory (MB)", memory.Available, memory.Total, gridList, rowIndex);
        }

        private void DisplayCpuRow(IotCpu cpu, IGridList<View> gridList, int rowIndex)
        {
            AddLabel(cpu.Name.ToString(), gridList, rowIndex, 0, 1);
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
            StartStopButton.Text = "Stop";
            _updatePiStats = true;
            UpdateDisplayAsync();
        }

        private MonitorTopic _monitorTopic;
        private PiStatsService _piStatsService;
        private bool _updatePiStats;

        private void StartStopButton_Clicked(object sender, EventArgs e)
        {
            if (_updatePiStats) StopRefresh();
            else StartRefresh();
        }

        async private void GpioButton_Clicked(object sender, EventArgs e)
        {
            StopRefresh();
            await Navigation.PushModalAsync(new NavigationPage(new PiCarGpioPage(_monitorTopic)));
        }

        async private void EditButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new MonitorTopicPage(_monitorTopic)));
        }

        async private void CommandButton_Clicked(object sender, EventArgs e)
        {
            StopRefresh();
            await Navigation.PushModalAsync(new NavigationPage(new PiCommandPage(_monitorTopic, _piStatsService)));
        }
    }
}