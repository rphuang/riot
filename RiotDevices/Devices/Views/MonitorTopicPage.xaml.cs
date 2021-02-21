using Devices.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Devices.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MonitorTopicPage : ContentPage
    {
        public MonitorTopicPage(Topic topic)
        {
            InitializeComponent();
            _originalTopic = topic;
            _monitorTopic = new Topic { Name = topic.Name, Server = topic.Server, Credential = topic.Credential };
            Title = _monitorTopic.Name;
            BindingContext = _monitorTopic;
        }

        private Topic _originalTopic;
        private Topic _monitorTopic;

        async private void ExitEdit_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        async private void SaveEdit_Clicked(object sender, EventArgs e)
        {
            Topics.ReplaceTopic(_originalTopic.Id, _monitorTopic);
            await Navigation.PopModalAsync();
        }

        async private void Add_Clicked(object sender, EventArgs e)
        {
            Topics.AddOrSaveTopic(_monitorTopic);
            await Navigation.PopModalAsync();
        }

        async private void Delete_Clicked(object sender, EventArgs e)
        {
            Topics.DeleteTopic(_monitorTopic);
            await Navigation.PopModalAsync();
        }
    }
}