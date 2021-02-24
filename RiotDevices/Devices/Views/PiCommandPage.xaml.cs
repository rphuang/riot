using Devices.Models;
using Riot.Pi.Client;
using SettingsLib;
using System;
using System.Collections.Generic;
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

            // determine custom commands from settings
            IEnumerable<SettingGroup> groups = DeviceSettings.Instance.GetSettingGroups("Type", "PiCommand");
            if (groups != null)
            {
                int index = 0;
                foreach (SettingGroup group in groups)
                {
                    switch (index)
                    {
                        case 0:
                            Command1Button.Text = group.Settings["Name"];
                            _command1 = group.Settings["Command"];
                            break;
                        case 1:
                            Command2Button.Text = group.Settings["Name"];
                            _command2 = group.Settings["Command"];
                            break;
                    }
                    index++;
                }
            }
        }

        private void DisplayCommandResponse(string rawText)
        {
            commandResponse.Text = rawText?.Replace("\\n", "\r\n");
        }

        private Topic _monitorTopic;
        private SystemClient _piSystemClient;
        private string _command1;
        private string _command2;

        async private void ExitPiCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void ShutdownButton_Clicked(object sender, EventArgs e)
        {
            DisplayCommandResponse(_piSystemClient.Post("sleep 1; sudo shutdown -h 0"));
        }

        private void RebootButton_Clicked(object sender, EventArgs e)
        {
            DisplayCommandResponse(_piSystemClient.Post("sleep 1; sudo reboot"));
        }

        private void Command1Button_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_command1)) DisplayCommandResponse(_piSystemClient.Post(_command1));
        }

        private void Command2Button_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_command2)) DisplayCommandResponse(_piSystemClient.Post(_command2));
        }

        private async void CommandButton_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Custom Command", "Enter the command to send.");
            if (!string.IsNullOrEmpty(result))
            {
                DisplayCommandResponse(_piSystemClient.Post(result));
            }
        }

        private async void GrepSyslogButton_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("grep /var/log/syslog", "Enter the string to search.");
            if (!string.IsNullOrEmpty(result))
            {
                DisplayCommandResponse(_piSystemClient.Post($"grep {result} /var/log/syslog"));
            }
        }

        private async void RestartButton_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("systemctl restart", "Enter the service to restart.");
            if (!string.IsNullOrEmpty(result))
            {
                DisplayCommandResponse(_piSystemClient.Post($"sudo systemctl restart {result}"));
            }
        }
    }
}