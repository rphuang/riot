using PiCar.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PiCar.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        private Dictionary<string, NavigationPage> _pages = new Dictionary<string, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            _pages.Add("About", (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(HomeMenuItem menu)
        {
            NavigationPage newPage = null;
            Topic topic = menu.Topic;
            if (!_pages.TryGetValue(menu.Title, out newPage))
            {
                if (topic != null)
                {
                    string topicType = topic.TopicType.ToLower();
                    switch (topicType)
                    {
                        case "control":
                            newPage = new NavigationPage(new PiCarPage(topic as ControlTopic));
                            break;
                        case "monitor":
                            newPage = new NavigationPage(new PiStatsPage(topic as MonitorTopic));
                            break;
                    }
                }
                else if (menu.MenuType == MenuItemType.Settings)
                {
                    newPage = new NavigationPage(new SettingsPage());
                }
                if (newPage == null) newPage = new NavigationPage(new AboutPage());
                _pages.Add(menu.Title, newPage);
            }

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}