using System;
using System.Collections.Generic;
using System.Text;

namespace PiCar.Models
{
    public enum MenuItemType
    {
        PiStats,
        PiGpio,
        Settings,
        About
    }

    public class HomeMenuItem
    {
        public MenuItemType MenuType { get; set; }

        public string Title { get; set; }

        public Topic Topic { get; set; }
    }
}
