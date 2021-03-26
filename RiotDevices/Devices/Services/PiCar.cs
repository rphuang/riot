using Riot;
using Riot.IoDevice;
using Riot.IoDevice.Client;
using Riot.Pi.Client;
using Xamarin.Forms;

namespace Devices.Services
{
    /// <summary>
    /// defines the operation mode for PiCar
    /// </summary>
    public enum PiCarMode
    {
        Manual,     // manual control
        Follow,     // follow current target
        Findline,   // find line and follow
        Opencv,     // opencv follow
        Speech      // use speech command to control
    }

    /// <summary>
    /// implement the PiCar - a Raspberry Pi robotic kit from Adeept
    /// </summary>
    public class PiCar : IotClientNode
    {
        /// <summary>
        /// whether the connection is connected
        /// </summary>
        public bool Connected { get { return _connected; } }

        /// <summary>
        /// get/set the operation mode of the PiCar
        /// </summary>
        public PiCarMode Mode
        {
            get { return _mode; }
            set { SetMode(value); }
        }

        /// <summary>
        /// get last request message for PiClient
        /// </summary>
        //public int LastStatusCode { get { return Client.LastStatusCode; } }

        /// <summary>
        /// get last request message for PiClient
        /// </summary>
        //public string LastMessage { get { return Client.LastMessage; } }

        /// <summary>
        /// the server computer
        /// </summary>
        public SystemClient Server { get; internal set; }

        /// <summary>
        /// the main drive motor for the picar
        /// </summary>
        public MotorClient Drive { get; internal set; }

        /// <summary>
        /// the steering servo for the picar
        /// </summary>
        public ServoClient Steering { get; internal set; }

        /// <summary>
        /// the servo controls the horizontal movement of the robotic head for the picar
        /// </summary>
        public ServoClient HeadHorizontalServo { get; internal set; }

        /// <summary>
        /// the servo controls the vertical movement of the robotic head for the picar
        /// </summary>
        public ServoClient HeadVerticalServo { get; internal set; }

        /// <summary>
        /// the left RGB LED
        /// </summary>
        public RGBLedClient LeftLed { get; internal set; }

        /// <summary>
        /// the left RGB LED
        /// </summary>
        public RGBLedClient RightLed { get; internal set; }

        /// <summary>
        /// the left RGB LED
        /// </summary>
        public StripLedClient StripLed { get; internal set; }

        /// <summary>
        /// the ultrasonic sensor for distance
        /// </summary>
        public UltrasonicClient Ultrasonic { get; internal set; }

        /// <summary>
        /// scan distance in front of picar
        /// </summary>
        public DistanceScanClient DistanceScan { get; internal set; }

        /// <summary>
        /// stop PiCar movement
        /// </summary>
        public string Reset()
        {
            // turn off: drive motor, steering servo, both servos for head, left led, right led
            string msg = PostPiCarCommand("{\"motor\": 0, \"servo\": 0, \"servoCamV\": 0, \"servoCamH\": 0, \"ledRight\": \"0,0,0\", \"ledLeft\": \"0,0,0\"}");
            HeadVerticalServo.ServoData.Angle = HeadHorizontalServo.ServoData.Angle = 0;
            StripLed.SetColor(Convert(Color.Black));
            return msg;
        }

        /// <summary>
        /// steer PiCar haed (camera) to look straight
        /// </summary>
        public string HeadStraight()
        {
            HeadVerticalServo.ServoData.Angle = HeadHorizontalServo.ServoData.Angle = 0;
            return PostPiCarCommand("{\"servoCamV\": 0, \"servoCamH\": 0}");
        }

        /// <summary>
        /// steer PiCar to straight
        /// </summary>
        public string SteerStraight()
        {
            return Steering.GoTo(0);
        }

        /// <summary>
        /// shut down the PiCar server
        /// </summary>
        public string Shutdown()
        {
            string msg = PostSysCommand("sleep 1; sudo shutdown -h 0");
            //_connected = Client.LastStatusCode == 200 && !string.IsNullOrEmpty(msg) && msg.Contains("picarclient");
            _connected = !string.IsNullOrEmpty(msg) && msg.Contains("picarclient");
            return msg;
        }

        /// <summary>
        /// reboot the PiCar server
        /// </summary>
        public string Reboot()
        {
            string msg = PostSysCommand("sleep 1; sudo reboot");
            //_connected = Client.LastStatusCode == 200 && !string.IsNullOrEmpty(msg) && msg.Contains("picarclient");
            _connected = !string.IsNullOrEmpty(msg) && msg.Contains("picarclient");
            return msg;
        }

        /// <summary>
        /// set the mode for picar
        /// </summary>
        public string SetMode(PiCarMode mode)
        {
            _mode = mode;
            string json = string.Format("{{\"mode/value\": \"{0}\"}}", mode.ToString());
            return PostPiCarCommand(json);
        }

        /// <summary>
        /// post http request to save setting
        /// </summary>
        /// <returns></returns>
        public string PostSetting(string json)
        {
            return Client.Post($"{Id}/settings", json);
        }

        /// <summary>
        /// post http request to save a setting
        /// </summary>
        /// <returns></returns>
        public string PostSetting(string key, string value)
        {
            string json = $"{{\"{key}\": \"{value}\"}}";
            return Client.Post($"{Id}/settings", json);
        }

        public string VideoStreamUrl { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public PiCar(string path, IotHttpClient httpIotClient)
            : base(path, null, null)
        {
            Client = httpIotClient;
            //Server = httpSystem;
            //Initialize(server, videoPort);
        }

        public string Initialize(string server, int videoPort)
        {
            Drive = new MotorClient("motor", Client, this);
            Steering = new ServoClient("servo", Client, this) { AngleOffset = SteeringOffset };
            HeadHorizontalServo = new ServoClient("servoCamH", Client, this);
            HeadVerticalServo = new ServoClient("servoCamV", Client, this);
            LeftLed = new RGBLedClient("ledLeft", Client, this);
            RightLed = new RGBLedClient("ledRight", Client, this);
            StripLed = new StripLedClient("ledStrip", Client, this);
            Ultrasonic = new UltrasonicClient("ultrasonic", Client, this);
            DistanceScan = new DistanceScanClient("scan", Client, this);
            VideoStreamUrl = string.Format("http://{0}:{1}/{2}", server, videoPort, "");
            string msg = Reset();
            //_connected = Client.LastStatusCode == 200;
            _connected = true;
            return msg;
        }

        private RGBColor Convert(Color color)
        {
            return new RGBColor(color.R, color.G, color.B);
        }

        private string PostSysCommand(string command)
        {
            return Server.Post(command);
        }

        private string PostPiCarCommand(string json)
        {
            return Client.Post(Id, json);
        }

        private bool _connected;
        private PiCarMode _mode = PiCarMode.Manual;
        // the value to offset the steering
        private int SteeringOffset = 0;
    }
}
