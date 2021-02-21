using Devices.Services;
using Devices.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Devices
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
