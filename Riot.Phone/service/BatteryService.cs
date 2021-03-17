using System;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// implements barometer sensor in a phone
    /// </summary>
    public class BatteryService : BaseServiceNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public BatteryService(IotNode parent) : base("Battery", parent)
        {
            BatteryStatus = new BatteryData {
                Id = nameof(BatteryStatus),
                ChargeLevel = Battery.ChargeLevel,
                BatteryState = Battery.State.ToString(),
                PowerSource = Battery.PowerSource.ToString(),
                EnergySaverStatus = Battery.EnergySaverStatus.ToString(),
                TimeStamp = DateTime.UtcNow
        };
            // Register for battery changes, be sure to unsubscribe when needed
            Battery.BatteryInfoChanged += Battery_BatteryInfoChanged;
        }

        /// <summary>
        /// barometer pressure value
        /// </summary>
        public BatteryData BatteryStatus
        {
            get { return Data[nameof(BatteryStatus)] as BatteryData; }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        protected override bool IsOn { get { return Battery.State != BatteryState.NotPresent && Battery.State != BatteryState.Unknown; } }

        /// <summary>
        /// start the sensor
        /// </summary>
        protected override bool StartSensor(SensorRate speed)
        {
            return true;
        }

        /// <summary>
        /// stop the sensor
        /// </summary>
        protected override bool StopSensor()
        {
            return true;
        }

        private void Battery_BatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
        {
            BatteryData data = BatteryStatus;
            data.ChargeLevel = e.ChargeLevel;
            data.BatteryState = e.State.ToString();
            data.PowerSource = e.PowerSource.ToString();
            data.TimeStamp = DateTime.UtcNow;
            data.SendNotification();
        }
    }
}
