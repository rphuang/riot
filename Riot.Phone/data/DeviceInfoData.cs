using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Riot.Phone
{
    /// <summary>
    /// defines the data for a phone device
    /// </summary>
    public class DeviceInfoData : IotData
    {
        /// <summary>
        /// constructor
        /// </summary>
        public DeviceInfoData(string id, IIotPoint parent) : base(id, parent)
        {
            Model = DeviceInfo.Model;
            Manufacturer = DeviceInfo.Manufacturer;
            Name = DeviceInfo.Name;
            VersionString = DeviceInfo.VersionString;
            Version = DeviceInfo.Version;
            Platform = DeviceInfo.Platform;
            Idiom = DeviceInfo.Idiom;
            DeviceType = DeviceInfo.DeviceType;
            // add fixed display info
            DisplayInfo display = DeviceDisplay.MainDisplayInfo;
            Height = display.Height;
            Width = display.Width;
            Density = display.Density;
            TimeStamp = DateTime.UtcNow;
        }

        //
        // Summary:
        //     Gets the model of the device.
        //
        // Value:
        //     Device model.
        public string Model { get; set; }
        //
        // Summary:
        //     Gets the manufacturer of the device.
        //
        // Value:
        //     Device manufacturer.
        public string Manufacturer { get; set; }
        //
        // Summary:
        //     Gets the name of the device.
        //
        // Value:
        //     The name of the device (often specified by the user).
        public string Name { get; set; }
        //
        // Summary:
        //     Gets the version of the operating system.
        //
        // Value:
        //     The version of the operating system.
        public string VersionString { get; set; }
        //
        // Summary:
        //     Gets the version of the operating system.
        //
        // Value:
        //     The device operating system.
        public Version Version { get; set; }
        //
        // Summary:
        //     Gets the platform or operating system of the device.
        //
        // Value:
        //     The platform of device.
        public DevicePlatform Platform { get; set; }
        //
        // Summary:
        //     Gets the idiom of the device.
        //
        // Value:
        //     The idiom.
        public DeviceIdiom Idiom { get; set; }
        //
        // Summary:
        //     Gets the type of device the application is running on.
        //
        // Value:
        //     The device type.
        public DeviceType DeviceType { get; set; }
        //
        // Summary:
        //     Gets the width of the scrreen for the current orientation.
        //
        // Value:
        //     The width in pixels.
        public double Width { get; }
        //
        // Summary:
        //     Gets the height of the screen for the current orientation.
        //
        // Value:
        //     The height in pixels.
        public double Height { get; }
        //
        // Summary:
        //     Gets a value representing the screen density.
        //
        // Value:
        //     The screen density.
        //
        // Remarks:
        //     The density is the scaling or a factor that can be used to convert between physical
        //     pixels and scaled pixels. For example, on high resolution displays, the physical
        //     number of pixels increases, but the scaled pixels remain the same.
        //     In a practical example for iOS, the Retina display will have a density of 2.0
        //     or 3.0, but the units used to lay out a view does not change much. A view with
        //     a UI width of 100 may be 100 physical pixels (density = 1) on a non-Retina device,
        //     but be 200 physical pixels (density = 2) on a Retina device.
        //     On Windows or UWP, the density works similarly, and may often relate to the scale
        //     used in the display. On some monitors, the scale is set to 100% (density = 1),
        //     but on other high resolution monitors, the scale may be set to 200% (density
        //     = 2) or even 250% (density = 2.5).
        public double Density { get; }
    }
}
