using Devices.Models;
using Devices.Views;
using PlatformLib;
using System;
using Xamarin.Forms;

namespace Devices
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public Topic CurrentTopic { get; private set; }

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PiStatsPage), typeof(PiStatsPage));
            Routing.RegisterRoute(nameof(Hs1xxPage), typeof(Hs1xxPage));
            Routing.RegisterRoute(nameof(PiCarPage), typeof(PiCarPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

            // load topic items
            Topics.InitializeTopics();
            foreach (Topic topic in Topics.TopicItems.Values)
            {
                if (topic != null)
                {
                    string topicType = topic.Type.ToLower();
                    switch (topicType)
                    {
                        case "device":
                            AddShellItem(topic.Id, new Hs1xxPage(topic as DeviceTopic));
                            break;
                        case "control":
                            AddShellItem(topic.Id, new PiCarPage(topic as ControlTopic));
                            break;
                        default:
                            AddShellItem(topic.Id, new PiStatsPage(topic));
                            break;
                    }
                }
                if (CurrentTopic == null) CurrentTopic = topic;
            }
            AddShellItem("Settings", new SettingsPage());

            //MenuItem menu = new MenuItem { Text = "Menu Item 1" };
            //menu.Clicked += OnMenuItemClicked;
            //Items.Add(menu);
            //ItemMenu item = new ItemMenu { Text = "Item 1", Item = new ItemLib.Item { Id = "AId", Name="AName", Type = "AType"} };
            //item.Clicked += OnMenuItemClicked;
            //Items.Add(item);
        }

        private void AddShellItem(string title, ContentPage page)
        {
            ShellSection shellSection = new ShellSection { Title = title, };

            shellSection.Items.Add(new ShellContent() { Content = page });
            Items.Add(shellSection);
        }

        private async void OnItemClicked(object sender, EventArgs e)
        {
            //ItemMenu menu = sender as ItemMenu;
            //Topic topic = menu?.Item as Topic;
            //if (topic != null)
            //{
            //    CurrentTopic = topic;
            //    string topicType = topic.Type.ToLower();
            //    switch (topicType)
            //    {
            //        case "device":
            //            await Shell.Current.GoToAsync("Hs1xxPage");
            //            break;
            //        case "control":
            //            await Shell.Current.GoToAsync("PiCarPage");
            //            break;
            //        default:
            //            await Shell.Current.GoToAsync("PiStatsPage");
            //            break;
            //    }
            //}
            //else if (menu.Text == "Settings")
            //{
            //    await Shell.Current.GoToAsync("SettingsPage");
            //}
            await Shell.Current.GoToAsync("SettingsPage");
        }
    }
}
