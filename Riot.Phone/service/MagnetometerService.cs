using System;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// implements Magnetometer sensor in a phone
    /// </summary>
    public class MagnetometerService : BaseServiceNode
    {
        /// <summary>
        /// get the single instance of Magnetometer
        /// </summary>
        internal static MagnetometerService Instance { get { return s_instance; } }

        /// <summary>
        /// latest Magnetometer MagneticField value
        /// </summary>
        public Vector3Data MagneticField
        {
            get { return Data[nameof(MagneticField)] as Vector3Data; }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        protected override bool IsOn { get { return Xamarin.Essentials.Magnetometer.IsMonitoring; } }

        /// <summary>
        /// start the sensor
        /// </summary>
        protected override bool StartSensor(SensorRate speed)
        {
            // Register for reading changes.
            Xamarin.Essentials.Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
            Xamarin.Essentials.Magnetometer.Start(ConvertSensorRate(speed));
            return true;
        }

        /// <summary>
        /// stop the sensor
        /// </summary>
        protected override bool StopSensor()
        {
            // unregister for reading changes.
            Xamarin.Essentials.Magnetometer.ReadingChanged -= Magnetometer_ReadingChanged;
            Xamarin.Essentials.Magnetometer.Stop();
            return true;
        }

        /// <summary>
        /// Internal method to create singleton instance must be called once during initialization
        /// </summary>
        internal static MagnetometerService CreateStaticInstance(IotNode parent)
        {
            if (s_instance == null) s_instance = new MagnetometerService(parent);
            return s_instance;
        }

        /// <summary>
        /// constructor
        /// </summary>
        private MagnetometerService(IotNode parent) : base("Magnetometer", parent)
        {
            MagneticField = Vector3Data.CreateZeroData(nameof(MagneticField));
        }

        private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            MagnetometerData reading = e.Reading;
            Vector3Data data = MagneticField;
            data.TimeStamp = DateTime.UtcNow;
            data.Value = reading.MagneticField;
            data.SendNotification();
        }

        private static MagnetometerService s_instance;
    }
}
