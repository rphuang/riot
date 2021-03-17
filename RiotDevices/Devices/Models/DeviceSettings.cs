using PlatformLib;
using SettingsLib;
using System.IO;

namespace Devices.Models
{
    public class DeviceSettings : Settings
    {
        /// <summary>
        /// Get the instance of the Settings 
        /// </summary>
        public static DeviceSettings Instance
        {
            get { return s_Settings; }
        }

        #region settings for phone web service

        /// <summary>
        /// if enabled a shell flyout item will be added to start phone service
        /// </summary>
        public bool EnablePhoneService
        {
            get { return GetOrAddSetting(nameof(EnablePhoneService), true, PhoneServiceGroupId); }
            set { SetSetting(nameof(EnablePhoneService), value, PhoneServiceGroupId); }
        }

        /// <summary>
        /// server prefix to listen ex: http://*:5678
        /// </summary>
        public string ServerPrefix
        {
            get { return GetOrAddSetting(nameof(ServerPrefix), "http://*:5678", PhoneServiceGroupId); }
            set { SetSetting(nameof(ServerPrefix), value, PhoneServiceGroupId); }
        }

        /// <summary>
        /// root path to listen. can be empty string or startwith "/" but not end with "/" 
        /// </summary>
        public string ServiceRootPath
        {
            get { return GetOrAddSetting(nameof(ServiceRootPath), string.Empty, PhoneServiceGroupId); }
            set { SetSetting(nameof(ServiceRootPath), value, PhoneServiceGroupId); }
        }

        /// <summary>
        /// root path to listen for actions. can be empty string or startwith "/" but not end with "/" 
        /// </summary>
        public string ServiceActionRootPath
        {
            get { return GetOrAddSetting(nameof(ServiceActionRootPath), "/cmd", PhoneServiceGroupId); }
            set { SetSetting(nameof(ServiceActionRootPath), value, PhoneServiceGroupId); }
        }

        /// <summary>
        /// credentials for accessing phone web service in format "user1:password1, user2:password2"
        /// </summary>
        public string ServiceCredentials
        {
            get { return GetOrAddSetting(nameof(ServiceCredentials), string.Empty, PhoneServiceGroupId); }
            set { SetSetting(nameof(ServiceCredentials), value, PhoneServiceGroupId); }
        }

        #endregion settings for phone web service

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
        /// the steering angle (0 to 90)
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
        /// whether the settings is valid
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// private constructor
        /// </summary>
        private DeviceSettings()
            : base(null)
        {
            string folderPath = FolderUtil.GetFolderPath(ThingsSettingsFolder, false);
            if (!string.IsNullOrEmpty(folderPath))
            {
                SettingsFile = Path.Combine(folderPath, ThingsSettingsFile);
                IsValid = LoadSettings();
            }
        }

        private const string ThingsSettingsFolder = "docs";
        private const string ThingsSettingsFile = "devicessettings.txt";
        private const string PhoneServiceGroupId = "PhoneService";
        private static DeviceSettings s_Settings = new DeviceSettings();
    }
}
