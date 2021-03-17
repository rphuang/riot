using System;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// implements barometer sensor in a phone
    /// </summary>
    public class BarometerService : BaseServiceNode
    {
        /// <summary>
        /// get the single instance of Barometer
        /// </summary>
        internal static BarometerService Instance { get { return s_instance; } }

        /// <summary>
        /// barometer pressure value
        /// </summary>
        public DoubleData Pressure
        {
            get { return Data[nameof(Pressure)] as DoubleData; }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        protected override bool IsOn { get { return Xamarin.Essentials.Barometer.IsMonitoring; } }

        /// <summary>
        /// start the sensor
        /// </summary>
        protected override bool StartSensor(SensorRate speed)
        {
            // Register for reading changes.
            Xamarin.Essentials.Barometer.ReadingChanged += Barometer_ReadingChanged;
            Xamarin.Essentials.Barometer.Start(ConvertSensorRate(speed));
            return true;
        }

        /// <summary>
        /// stop the sensor
        /// </summary>
        protected override bool StopSensor()
        {
            // Unregister for reading changes.
            Xamarin.Essentials.Barometer.ReadingChanged -= Barometer_ReadingChanged;
            Xamarin.Essentials.Barometer.Stop();
            return true;
        }

        /// <summary>
        /// Internal method to create singleton instance must be called once during initialization
        /// </summary>
        internal static BarometerService CreateStaticInstance(IotNode parent)
        {
            if (s_instance == null) s_instance = new BarometerService(parent);
            return s_instance;
        }

        /// <summary>
        /// constructor
        /// </summary>
        private BarometerService(IotNode parent) : base("Barometer", parent)
        {
            Pressure = DoubleData.CreateZeroData(nameof(Pressure));
        }

        private void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
        {
            Xamarin.Essentials.BarometerData reading = e.Reading;
            DoubleData data = Pressure;
            data.TimeStamp = DateTime.UtcNow;
            data.Value = reading.PressureInHectopascals;
            data.SendNotification();
        }

        private static BarometerService s_instance;
    }
}
