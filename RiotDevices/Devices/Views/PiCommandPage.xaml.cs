using Devices.Models;
using Riot.Pi.Client;
using SettingsLib;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Grid;

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

            InitializeCommandsGrid();
        }

        private void DisplayCommandResponse(string rawText)
        {
            commandResponse.Text = rawText?.Replace("\\n", "\r\n");
        }

        private class CommandDef
        {
            /// <summary>
            /// the name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// the command
            /// </summary>
            public string Command { get; set; }

            /// <summary>
            /// whether user confirm is required
            /// </summary>
            public bool UserConfirm { get; set; }

            /// <summary>
            /// whether user input is required
            /// </summary>
            public bool UserInput { get; set; }

            /// <summary>
            /// format string for the command when user input is required
            /// </summary>
            public string CommandFormat { get; set; }

            /// <summary>
            /// the title for the prompt popup
            /// </summary>
            public string PromptTitle { get; set; }

            /// <summary>
            /// the message for the prompt popup
            /// </summary>
            public string PromptMessage { get; set; }
        }

        private class CommandButton : Button
        {
            /// <summary>
            /// the CommandDef for the button
            /// </summary>
            public CommandDef CommandDef { get; set; }
        }

        private void InitializeCommandsGrid()
        {
            // determine custom commands from settings
            IEnumerable<SettingGroup> groups = DeviceSettings.Instance.GetSettingGroups("Type", "PiCommand");
            if (groups != null)
            {
                int index = 0;
                foreach (SettingGroup group in groups)
                {
                    string topicId;
                    if (group.Settings.TryGetValue("Topic", out topicId))
                    {
                        if (!string.Equals(_monitorTopic.Id, topicId, StringComparison.OrdinalIgnoreCase)) continue;
                    }
                    string name;
                    string cmd;
                    string confirm;
                    group.Settings.TryGetValue("Name", out name);
                    group.Settings.TryGetValue("Command", out cmd);
                    group.Settings.TryGetValue("Confirm", out confirm);
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(cmd)) continue;

                    IGridList<View> gridList = CommandsGrid.Children;
                    CommandButton button = CreateCommandButton(name, cmd, confirm);
                    if (button != null)
                    {
                        int row = index / 2;
                        int col = index % 2;
                        gridList.Add(button, col, col + 1, row, row + 1);

                        index++;
                    }
                }
            }

        }

        private CommandButton CreateCommandButton(string name, string cmd, string confirmStr)
        {
            if (string.IsNullOrEmpty(cmd)) return null;

            // command string syntax: command_text_1 {prompt_title, prompt_message} command_text_2
            bool confirm = false;
            if (!string.IsNullOrEmpty(confirmStr)) confirm = string.Equals("Yes", confirmStr, StringComparison.OrdinalIgnoreCase);
            CommandDef def = new CommandDef { Name = name, Command = cmd, UserConfirm = confirm };
            CommandButton button = new CommandButton { CommandDef = def, Text = name };
            button.Clicked += CommandButton_Clicked;
            int index = cmd.IndexOf('{');
            int index2 = cmd.IndexOf('}');
            if (index < 0 || index2 < 0 || index+3 >= index2) return button;   // minimum 3 chars like: {a,b}

            def.CommandFormat = cmd.Substring(0, index).Trim() + " {0} ";
            if (index2 < cmd.Length) def.CommandFormat += cmd.Substring(index2+1).Trim();
            string[] parts = cmd.Substring(index + 1, index2 - index - 1).Split(CommaDelimiter);
            def.PromptTitle = parts[0].Trim();
            def.PromptMessage = parts[1].Trim();
            def.UserInput = true;
            return button;
        }

        private readonly static char[] CommaDelimiter = { ',' };
        private Topic _monitorTopic;
        private SystemClient _piSystemClient;

        async private void ExitPiCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void CommandButton_Clicked(object sender, EventArgs e)
        {
            CommandButton button = sender as CommandButton;
            CommandDef commandDef = (sender as CommandButton)?.CommandDef;
            if (commandDef == null) return;

            if (commandDef.UserInput)
            {
                string result = await DisplayPromptAsync(commandDef.PromptTitle, commandDef.PromptMessage);
                if (!string.IsNullOrEmpty(result))
                {
                    string cmd = string.Format(commandDef.CommandFormat, result);
                    DisplayCommandResponse(_piSystemClient?.Post(cmd));
                }
            }
            else
            {
                if (commandDef.UserConfirm)
                {
                    bool answer = await DisplayAlert(commandDef.Name, "Do you really want to send the command?", "Yes", "Cancel");
                    if (!answer) return;
                }
                DisplayCommandResponse(_piSystemClient?.Post(commandDef.Command));
            }
        }
    }
}