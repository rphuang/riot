using System;
using System.Numerics;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// implements Accelerometer sensor in a phone
    /// </summary>
    public class AccelerometerService : BaseServiceNode
    {
        /// <summary>
        /// get the single instance of Accelerometer
        /// </summary>
        internal static AccelerometerService Instance { get { return s_instance; } }

        /// <summary>
        /// latest Accelerometer value
        /// </summary>
        public Vector3Data Acceleration
        {
            get { return Data[nameof(Acceleration)] as Vector3Data; }
            internal set { UpsertData(value); }
        }

        public BoolData Shack
        {
            get { return Data[nameof(Shack)] as BoolData; }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        protected override bool IsOn { get { return Xamarin.Essentials.Accelerometer.IsMonitoring; } }

        /// <summary>
        /// start the sensor
        /// </summary>
        protected override bool StartSensor(SensorRate speed)
        {
            // subscribe for reading changes
            Xamarin.Essentials.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            Xamarin.Essentials.Accelerometer.ShakeDetected += Accelerometer_ShakeDetected;
            Xamarin.Essentials.Accelerometer.Start(ConvertSensorRate(speed));
            return true;
        }

        /// <summary>
        /// stop the sensor
        /// </summary>
        protected override bool StopSensor()
        {
            // unsubscribe for reading changes
            Xamarin.Essentials.Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            Xamarin.Essentials.Accelerometer.ShakeDetected -= Accelerometer_ShakeDetected;
            Xamarin.Essentials.Accelerometer.Stop();
            return true;
        }

        /// <summary>
        /// Internal method to create singleton instance must be called once during initialization
        /// </summary>
        internal static AccelerometerService CreateStaticInstance(IotNode parent)
        {
            if (s_instance == null) s_instance = new AccelerometerService(parent);
            return s_instance;
        }

        private AccelerometerService(IotNode parent) : base("Accelerometer", parent)
        {
            Acceleration = Vector3Data.CreateZeroData(nameof(Acceleration));
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            Xamarin.Essentials.AccelerometerData reading = e.Reading;
            Vector3Data data = Acceleration;
            data.Value = reading.Acceleration;
            data.TimeStamp = DateTime.UtcNow;
            data.SendNotification();
        }

        void Accelerometer_ShakeDetected(object sender, EventArgs e)
        {
            Shack.TimeStamp = DateTime.UtcNow;
            Shack.Value = true;
            Shack.SendNotification();
        }

        private static AccelerometerService s_instance;
    }
}
