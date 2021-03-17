using System;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// implements Gyroscope sensor in a phone
    /// </summary>
    public class GyroscopeService : BaseServiceNode
    {
        /// <summary>
        /// get the single instance of Gyroscope
        /// </summary>
        internal static GyroscopeService Instance { get { return s_instance; } }

        /// <summary>
        /// latest Gyroscope AngularVelocity data
        /// </summary>
        public Vector3Data AngularVelocity
        {
            get { return Data[nameof(AngularVelocity)] as Vector3Data; }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        protected override bool IsOn { get { return Xamarin.Essentials.Gyroscope.IsMonitoring; } }

        /// <summary>
        /// start the sensor
        /// </summary>
        protected override bool StartSensor(SensorRate speed)
        {
            // Register for reading changes.
            Xamarin.Essentials.Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            Xamarin.Essentials.Gyroscope.Start(ConvertSensorRate(speed));
            return true;
        }

        /// <summary>
        /// stop the sensor
        /// </summary>
        protected override bool StopSensor()
        {
            // Register for reading changes.
            Xamarin.Essentials.Gyroscope.ReadingChanged -= Gyroscope_ReadingChanged;
            Xamarin.Essentials.Gyroscope.Stop();
            return true;
        }

        /// <summary>
        /// Internal method to create singleton instance must be called once during initialization
        /// </summary>
        internal static GyroscopeService CreateStaticInstance(IotNode parent)
        {
            if (s_instance == null) s_instance = new GyroscopeService(parent);
            return s_instance;
        }

        /// <summary>
        /// constructor
        /// </summary>
        private GyroscopeService(IotNode parent) : base("Gyroscope", parent)
        {
            AngularVelocity = Vector3Data.CreateZeroData(nameof(AngularVelocity));
        }

        private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            GyroscopeData reading = e.Reading;
            Vector3Data data = AngularVelocity;
            data.TimeStamp = DateTime.UtcNow;
            data.Value = reading.AngularVelocity;
            data.SendNotification();
        }

        private static GyroscopeService s_instance;
    }
}
