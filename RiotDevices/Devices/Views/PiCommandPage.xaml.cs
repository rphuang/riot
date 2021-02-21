using Devices.Models;
using Riot.Pi.Client;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Devices.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PiCommandPage : ContentPage
    {
        public PiCommandPage(Topic topic, SystemClient piStatsService)
        {
            InitializeComponent();
            Title = topic.Name;

            _monitorTopic = topic;
            _piSystemClient = piStatsService;
        }

        private Topic _monitorTopic;
        private SystemClient _piSystemClient;

        async private void ExitPiCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void ShutdownButton_Clicked(object sender, EventArgs e)
        {
            commandResponse.Text = _piSystemClient.Post("sleep 1; sudo shutdown -h 0");
        }

        private void RebootButton_Clicked(object sender, EventArgs e)
        {
            commandResponse.Text = _piSystemClient.Post("sleep 1; sudo reboot");
        }

        private void CommandButton_Clicked(object sender, EventArgs e)
        {
            commandResponse.Text = _piSystemClient.Post(commandEntry.Text);
        }
    }
}