using HttpLib;

namespace Riot.Phone.Service
{
    /// <summary>
    /// Http service host for phone sensors
    /// </summary>
    public class PhoneServiceHost : HttpServiceHost
    {
        /// <summary>
        /// constructor
        /// </summary>
        public PhoneServiceHost(string serverPrefix, string rootPath, string actionRootPath, string credentials) : base(serverPrefix)
        {
            _rootPath = rootPath;
            _actionRootPath = actionRootPath;
            _credentials = credentials;
        }

        /// <summary>
        /// initialize 
        /// </summary>
        public override HttpServiceHost Init()
        {
            _phoneService = PhoneService.Instance;
            // create handlers with sensors as root paths
            new PhoneRequestHandler(_phoneService.Accelerometer, "Accelerometer", _rootPath + "/accelerometer", "Accelerometer", nameof(AccelerometerService.Acceleration), _credentials);
            new PhoneRequestHandler(_phoneService.Barometer, "Barometer", _rootPath + "/barometer", "Barometer", nameof(BarometerService.Pressure), _credentials);
            new PhoneRequestHandler(_phoneService.Compass, "Compass", _rootPath + "/compass", "Compass", nameof(CompassService.Heading), _credentials);
            new PhoneRequestHandler(_phoneService.GpsLocation, "GPS", _rootPath + "/gps", "GPS", nameof(GpsLocationService.Location), _credentials);
            new PhoneRequestHandler(_phoneService.Gyroscope, "Gyroscope", _rootPath + "/gyroscope", "Gyroscope", nameof(GyroscopeService.AngularVelocity), _credentials);
            new PhoneRequestHandler(_phoneService.Magnetometer, "Magnetometer", _rootPath + "/magnetometer", "Magnetometer", nameof(MagnetometerService.MagneticField), _credentials);
            new PhoneRequestHandler(_phoneService.OrientationSensor, "OrientationSensor", _rootPath + "/orientationSensor", "OrientationSensor", nameof(OrientationSensorService.Orientation), _credentials);
            new PhoneRequestHandler(_phoneService, "DeviceInfo", _rootPath + "/deviceinfo", "DeviceInfoData", nameof(PhoneService.DeviceInfo), _credentials);
            new PhoneRequestHandler(_phoneService.TextToSpeechService, "TextToSpeech", _rootPath + "/texttospeech", "LocalesData", nameof(TextToSpeechService.Locales), _credentials);
            new PhoneRequestHandler(_phoneService.Battery, "Battery", _rootPath + "/battery", "BatteryData", nameof(BatteryService.BatteryStatus), _credentials);
            new ActionRequestHandler<SpeechActionData>(_phoneService.TextToSpeechService, "TextToSpeech", _actionRootPath + "/speak", _credentials);
            new ActionRequestHandler<SmsActionData>(_phoneService.Sms, "SMS", _actionRootPath + "/sms", _credentials);
            new ActionRequestHandler<DoubleData>(_phoneService.VibrateAction, "Vibrate", _actionRootPath + "/vibrate", _credentials);
            new ActionRequestHandler<CaptureActionData>(_phoneService.CaptureAction, "Capture", _actionRootPath + "/capture", _credentials);
            new ActionRequestHandler<EmailActionData>(_phoneService.Email, "Email", _actionRootPath + "/email", _credentials);
            new SubscribeRequestHandler(_phoneService, "Subscription", _rootPath + "/subscribe", _credentials);

            // create handler to handle "/"
            EndpointRequestHandler root = new EndpointRequestHandler("Root", "/", _credentials);
            return base.Init();
        }

        /// <summary>
        /// start all handlers
        /// </summary>
        public override void Start()
        {
            // start all sensors
            _phoneService.StartAll();
            base.Start();
        }

        /// <summary>
        /// start all handlers
        /// </summary>
        public override void Stop()
        {
            // stop all sensors
            _phoneService.StopAll();
            base.Stop();
        }

        protected string _rootPath;
        protected string _actionRootPath;
        protected string _credentials;
        protected PhoneService _phoneService;
    }
}
