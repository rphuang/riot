using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// encapsulates all the sensor nodes for a phone
    /// </summary>
    public class PhoneService : BaseServiceNode
    {
        /// <summary>
        /// get static instance of PhoneService
        /// </summary>
        public static PhoneService Instance { get { return s_instance; } }

        /// <summary>
        /// constructor
        /// </summary>
        public PhoneService(string server, string credential)
        {
            Accelerometer = AccelerometerService.CreateStaticInstance(this);
            AddNode(Accelerometer);
            Barometer = BarometerService.CreateStaticInstance(this);
            AddNode(Barometer);
            Compass = CompassService.CreateStaticInstance(this);
            AddNode(Compass);
            GpsLocation = new GpsLocationService(this);
            AddNode(GpsLocation);
            Gyroscope = GyroscopeService.CreateStaticInstance(this);
            AddNode(Gyroscope);
            Magnetometer = MagnetometerService.CreateStaticInstance(this);
            AddNode(Magnetometer);
            OrientationSensor = OrientationSensorService.CreateStaticInstance(this);
            AddNode(OrientationSensor);
            TextToSpeechService = new TextToSpeechService(this);
            AddNode(TextToSpeechService);
            Sms = new SmsActionService(this);
            AddNode(Sms);
            VibrateAction = new VibrateActionService(this);
            AddNode(VibrateAction);
            CaptureAction = new CaptureActionService(this);
            AddNode(CaptureAction);
            Email = new EmailActionService(this);
            AddNode(Email);
            Battery = new BatteryService(this);
            AddNode(Battery);
            // device data
            DeviceInfo = new DeviceInfoData(nameof(DeviceInfo), this);
        }

        /// <summary>
        /// device info data
        /// </summary>
        public DeviceInfoData DeviceInfo
        {
            get { return Data[nameof(DeviceInfo)] as DeviceInfoData; }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// Accelerometer sensor on the phone
        /// </summary>
        public AccelerometerService Accelerometer { get; private set; }

        /// <summary>
        /// Barometer sensor on the phone
        /// </summary>
        public BarometerService Barometer { get; private set; }

        /// <summary>
        /// Compass sensor on the phone
        /// </summary>
        public CompassService Compass { get; private set; }

        /// <summary>
        /// GpsLocation sensor on the phone
        /// </summary>
        public GpsLocationService GpsLocation { get; private set; }

        /// <summary>
        /// Gyroscope sensor on the phone
        /// </summary>
        public GyroscopeService Gyroscope { get; private set; }

        /// <summary>
        /// Magnetometer sensor on the phone
        /// </summary>
        public MagnetometerService Magnetometer { get; private set; }

        /// <summary>
        /// Orientation sensor on the phone
        /// </summary>
        public OrientationSensorService OrientationSensor { get; private set; }

        /// <summary>
        /// TextToSpeech Service on the phone
        /// </summary>
        public TextToSpeechService TextToSpeechService { get; private set; }

        /// <summary>
        /// send SMS message 
        /// </summary>
        public SmsActionService Sms { get; private set; }

        /// <summary>
        /// vibrate 
        /// </summary>
        public VibrateActionService VibrateAction { get; private set; }

        /// <summary>
        /// Capture
        /// </summary>
        public CaptureActionService CaptureAction { get; private set; }

        /// <summary>
        /// email
        /// </summary>
        public EmailActionService Email { get; private set; }

        /// <summary>
        /// battery status
        /// </summary>
        public BatteryService Battery { get; private set; }

        /// <summary>
        /// start all sensors
        /// </summary>
        public void StartAll(SensorRate rate = SensorRate.Medium)
        {
            foreach (IotNode node in Children)
            {
                BaseServiceNode sensor = node as BaseServiceNode;
                if (sensor == null) continue;
                sensor.Status.SensorRate = rate;
                sensor.Start();
            }
        }

        /// <summary>
        /// stop all sensors
        /// </summary>
        public void StopAll()
        {
            foreach (IotNode node in Children)
            {
                BaseServiceNode sensor = node as BaseServiceNode;
                if (sensor == null) continue;
                sensor.Stop();
            }
        }

        /// <summary>
        /// whether the sensor is on
        /// </summary>
        [JsonIgnore]
        protected override bool IsOn { get; } = false;

        /// <summary>
        /// start the sensor
        /// </summary>
        protected override bool StartSensor(SensorRate speed) { return true; }

        /// <summary>
        /// stop the sensor
        /// </summary>
        protected override bool StopSensor() { return true; }

        private static PhoneService s_instance = new PhoneService(null, null);
    }
}
