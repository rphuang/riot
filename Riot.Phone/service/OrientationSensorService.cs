using System;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// implements OrientationSensor sensor in a phone
    /// </summary>
    public class OrientationSensorService : BaseServiceNode
    {
        /// <summary>
        /// get the single instance of OrientationSensor
        /// </summary>
        public static OrientationSensorService Instance { get { return s_instance; } }

        /// <summary>
        /// latest OrientationSensor Orientation value
        /// </summary>
        public QuaternionData Orientation
        {
            get { return Data[nameof(Orientation)] as QuaternionData; }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        protected override bool IsOn { get { return Xamarin.Essentials.OrientationSensor.IsMonitoring; } }

        /// <summary>
        /// start the sensor
        /// </summary>
        protected override bool StartSensor(SensorRate speed)
        {
            // Register for reading changes.
            Xamarin.Essentials.OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
            Xamarin.Essentials.OrientationSensor.Start(ConvertSensorRate(speed));
            return true;
        }

        /// <summary>
        /// stop the sensor
        /// </summary>
        protected override bool StopSensor()
        {
            // unregister for reading changes.
            Xamarin.Essentials.OrientationSensor.ReadingChanged -= OrientationSensor_ReadingChanged;
            Xamarin.Essentials.OrientationSensor.Stop();
            return true;
        }

        /// <summary>
        /// Internal method to create singleton instance must be called once during initialization
        /// </summary>
        internal static OrientationSensorService CreateStaticInstance(IotNode parent)
        {
            if (s_instance == null) s_instance = new OrientationSensorService(parent);
            return s_instance;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public OrientationSensorService(IotNode parent) : base("OrientationSensor", parent)
        {
            Orientation = QuaternionData.CreateZeroData(nameof(Orientation));
        }

        private void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            OrientationSensorData reading = e.Reading;
            QuaternionData data = Orientation;
            data.Value = reading.Orientation;
            data.TimeStamp = DateTime.UtcNow;
            data.SendNotification();
        }

        private static OrientationSensorService s_instance;
    }
}
