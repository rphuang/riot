using PlatformLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace PiCar.Models
{
    public class Settings : SettingsBase
    {
        /// <summary>
        /// Get the instance of the Settings 
        /// </summary>
        public static Settings Instance
        {
            get { return s_Settings; }
        }

        /// <summary>
        /// The user ID when personalization is on
        /// </summary>
        public string TopicsFile
        {
            get { return GetOrAddSetting(nameof(TopicsFile), "index.csv"); }
            set { SetSetting(nameof(TopicsFile), value); }
        }

        /// <summary>
        /// the Monitor Refresh Rate in seconds
        /// </summary>
        public double MonitorRefreshRate
        {
            get { return GetOrAddSetting(nameof(MonitorRefreshRate), 1.1); }
            set { SetSetting(nameof(MonitorRefreshRate), value); }
        }

        /// <summary>
        /// the speed for motor (-100 to 100)
        /// </summary>
        public int MotorSpeed
        {
            get { return GetOrAddSetting(nameof(MotorSpeed), 100); }
            set { SetSetting(nameof(MotorSpeed), value); }
        }

        /// <summary>
        /// the steering angle for motor (0 to 90)
        /// </summary>
        public int SteeringAngle
        {
            get { return GetOrAddSetting(nameof(SteeringAngle), 40); }
            set { SetSetting(nameof(SteeringAngle), value); }
        }

        /// <summary>
        /// the delta angle for moving camera both vertically and horizontally (0 to 90)
        /// </summary>
        public int DeltaCameraAngle
        {
            get { return GetOrAddSetting(nameof(DeltaCameraAngle), 5); }
            set { SetSetting(nameof(DeltaCameraAngle), value); }
        }

        #region distance scan default values

        /// <summary>
        /// the horizontal start angle for distance scan (-90 to 90)
        /// </summary>
        public int HorizontalStartAngle
        {
            get { return GetOrAddSetting(nameof(HorizontalStartAngle), -90); }
            set { SetSetting(nameof(HorizontalStartAngle), value); }
        }

        /// <summary>
        /// the horizontal end angle for distance scan (-90 to 90)
        /// </summary>
        public int HorizontalEndAngle
        {
            get { return GetOrAddSetting(nameof(HorizontalEndAngle), 90); }
            set { SetSetting(nameof(HorizontalEndAngle), value); }
        }

        /// <summary>
        /// the increment for horizontal angle for distance scan (-90 to 90)
        /// </summary>
        public int HorizontalIncAngle
        {
            get { return GetOrAddSetting(nameof(HorizontalIncAngle), 5); }
            set { SetSetting(nameof(HorizontalIncAngle), value); }
        }

        /// <summary>
        /// the Vertical start angle for distance scan (-90 to 90)
        /// </summary>
        public int VerticalStartAngle
        {
            get { return GetOrAddSetting(nameof(VerticalStartAngle), 0); }
            set { SetSetting(nameof(VerticalStartAngle), value); }
        }

        /// <summary>
        /// the Vertical end angle for distance scan (-90 to 90)
        /// </summary>
        public int VerticalEndAngle
        {
            get { return GetOrAddSetting(nameof(VerticalEndAngle), 0); }
            set { SetSetting(nameof(VerticalEndAngle), value); }
        }

        /// <summary>
        /// the increment for Vertical angle for distance scan (-90 to 90)
        /// </summary>
        public int VerticalIncAngle
        {
            get { return GetOrAddSetting(nameof(VerticalIncAngle), 5); }
            set { SetSetting(nameof(VerticalIncAngle), value); }
        }

        #endregion

        /// <summary>
        /// private constructor
        /// </summary>
        private Settings()
            : base(PiCarSettingsFolder, PiCarSettingsFile)
        {
        }

        private const string PiCarSettingsFolder = "docs";
        private const string PiCarSettingsFile = "PiCarsettings.txt";
        private static Settings s_Settings = new Settings();
    }
}
