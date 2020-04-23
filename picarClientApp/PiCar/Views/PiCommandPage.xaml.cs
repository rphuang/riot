using PiCar.Models;
using PiCar.Services;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PiCar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PiCommandPage : ContentPage
    {
        public PiCommandPage(MonitorTopic topic, PiStatsService piStatsService)
        {
            InitializeComponent();
            Title = topic.Name;

            _monitorTopic = topic;
            _piStatsService = piStatsService;
        }

        private MonitorTopic _monitorTopic;
        private PiStatsService _piStatsService;

        async private void ExitPiCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void ShutdownButton_Clicked(object sender, EventArgs e)
        {
            commandResponse.Text = _piStatsService.PiSystem.Post("sleep 1; sudo shutdown -h 0");
        }

        private void RebootButton_Clicked(object sender, EventArgs e)
        {
            commandResponse.Text = _piStatsService.PiSystem.Post("sleep 1; sudo reboot");
        }

        private void CommandButton_Clicked(object sender, EventArgs e)
        {
            commandResponse.Text = _piStatsService.PiSystem.Post(commandEntry.Text);
        }
    }
}