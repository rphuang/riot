using PiCar.Models;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PiCar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MonitorTopicPage : ContentPage
    {
        public MonitorTopicPage(MonitorTopic topic)
        {
            InitializeComponent();
            _originalTopic = topic;
            _monitorTopic = new MonitorTopic { Name = topic.Name, Server = topic.Server, Credential = topic.Credential };
            Title = _monitorTopic.Name;
            BindingContext = _monitorTopic;
        }

        private MonitorTopic _originalTopic;
        private MonitorTopic _monitorTopic;

        async private void ExitEdit_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        async private void SaveEdit_Clicked(object sender, EventArgs e)
        {
            Topic.ReplaceTopic(_originalTopic.Id, _monitorTopic);
            await Navigation.PopModalAsync();
        }

        async private void Add_Clicked(object sender, EventArgs e)
        {
            Topic.AddOrSaveTopic(_monitorTopic);
            await Navigation.PopModalAsync();
        }

        async private void Delete_Clicked(object sender, EventArgs e)
        {
            Topic.DeleteTopic(_monitorTopic);
            await Navigation.PopModalAsync();
        }
    }
}