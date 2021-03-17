using System;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// implements Compass sensor in a phone
    /// </summary>
    public class CompassService : BaseServiceNode
    {
        /// <summary>
        /// get the single instance of Compass
        /// </summary>
        internal static CompassService Instance { get { return s_instance; } }

        /// <summary>
        /// Compass data Heading
        /// </summary>
        public DoubleData Heading
        {
            get { return Data[nameof(Heading)] as DoubleData; }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        protected override bool IsOn { get { return Xamarin.Essentials.Compass.IsMonitoring; } }

        /// <summary>
        /// start the sensor
        /// </summary>
        protected override bool StartSensor(SensorRate speed)
        {
            // Register for reading changes.
            Xamarin.Essentials.Compass.ReadingChanged += Compass_ReadingChanged;
            Xamarin.Essentials.Compass.Start(ConvertSensorRate(speed));
            return true;
        }

        /// <summary>
        /// stop the sensor
        /// </summary>
        protected override bool StopSensor()
        {
            // unregister for reading changes.
            Xamarin.Essentials.Compass.ReadingChanged -= Compass_ReadingChanged;
            Xamarin.Essentials.Compass.Stop();
            return true;
        }

        /// <summary>
        /// Internal method to create singleton instance must be called once during initialization
        /// </summary>
        internal static CompassService CreateStaticInstance(IotNode parent)
        {
            if (s_instance == null) s_instance = new CompassService(parent);
            return s_instance;
        }

        /// <summary>
        /// constructor
        /// </summary>
        private CompassService(IotNode parent) : base("Compass", parent)
        {
            Heading = DoubleData.CreateZeroData(nameof(Heading));
        }

        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            Xamarin.Essentials.CompassData reading = e.Reading;
            DoubleData data = Heading;
            data.TimeStamp = DateTime.UtcNow;
            data.Value = reading.HeadingMagneticNorth;
            data.SendNotification();
        }

        private static CompassService s_instance;
    }
}
