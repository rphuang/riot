using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PiCar.ViewModels
{
    public class AboutViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://github.com/rphuang/riot"));
        }

        public ICommand OpenWebCommand { get; }
        public string Title { get; set; }
    }
}