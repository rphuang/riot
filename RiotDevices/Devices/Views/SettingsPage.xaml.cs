using Devices.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Devices.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            _theSettings = DeviceSettings.Instance;
            BindingContext = _theSettings;
        }

        private void ExitSettings_Clicked(object sender, EventArgs e)
        {
        }

        private void SaveSettings_Clicked(object sender, EventArgs e)
        {
            _theSettings.SaveSettings();
        }

        /// <summary>
        /// Settings instance for binding
        /// </summary>
        private DeviceSettings _theSettings;
    }
}