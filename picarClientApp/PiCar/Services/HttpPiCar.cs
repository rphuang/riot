using IotClientLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace PiCar.Services
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
    class HttpPiCar : HttpNode
    {
        /// <summary>
        /// create and initialize a PiCar
        /// </summary>
        public static HttpPiCar CreatePiCar(string name, string server, int port, int videoPort, string user, string password)
        {
            HttpIotClient httpIotClient = new HttpIotClient();
            httpIotClient.SetServer(server, port);
            httpIotClient.SetCredential(user, password);

            // discover server endpoints and paths
            IList<HttpEndpoint> endpoints = httpIotClient.DiscoverAvailableEndpoints();
            var picarEndpoints = endpoints.Where((HttpEndpoint item) => string.Equals(item.Name, "picar", StringComparison.OrdinalIgnoreCase));
            if (picarEndpoints.Count() == 0) return null;   // not picar

            HttpSystem httpSystem = null;
            var sysEndpoints = endpoints.Where((HttpEndpoint item) => string.Equals(item.Name, "PiSystem", StringComparison.OrdinalIgnoreCase));
            if (sysEndpoints.Count() > 0)
            {
                string path = sysEndpoints.First().Path;
                httpSystem = new HttpSystem(path, "Server", httpIotClient, null);
            }

            string piCarPath = picarEndpoints.First().Path;
            HttpPiCar picar = new HttpPiCar(piCarPath, name, httpIotClient, httpSystem, server, videoPort);
            return picar;
        }

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
        public int LastStatusCode { get { return Client.LastStatusCode; } }

        /// <summary>
        /// get last request message for PiClient
        /// </summary>
        public string LastMessage { get { return Client.LastMessage; } }

        /// <summary>
        /// the server computer
        /// </summary>
        public IotSystem Server { get; internal set; }

        /// <summary>
        /// the main drive motor for the picar
        /// </summary>
        public IotMotor Drive { get; internal set; }

        /// <summary>
        /// the steering servo for the picar
        /// </summary>
        public IotServo Steering { get; internal set; }

        /// <summary>
        /// the servo controls the horizontal movement of the robotic head for the picar
        /// </summary>
        public IotServo HeadHorizontalServo { get; internal set; }

        /// <summary>
        /// the servo controls the vertical movement of the robotic head for the picar
        /// </summary>
        public IotServo HeadVerticalServo { get; internal set; }

        /// <summary>
        /// the left RGB LED
        /// </summary>
        public IotRGBLed LeftLed { get; internal set; }

        /// <summary>
        /// the left RGB LED
        /// </summary>
        public IotRGBLed RightLed { get; internal set; }

        /// <summary>
        /// the left RGB LED
        /// </summary>
        public IotStripLed StripLed { get; internal set; }

        /// <summary>
        /// the ultrasonic sensor for distance
        /// </summary>
        public IotUltrasonic Ultrasonic { get; internal set; }

        /// <summary>
        /// scan distance in front of picar
        /// </summary>
        public IotScanDistance DistanceScan { get; internal set; }

        /// <summary>
        /// stop PiCar movement
        /// </summary>
        public string Reset()
        {
            // turn off: drive motor, steering servo, both servos for head, left led, right led
            string msg = PostPiCarCommand("{\"motor\": 0, \"servo\": 0, \"servoCamV\": 0, \"servoCamH\": 0, \"ledRight\": \"0,0,0\", \"ledLeft\": \"0,0,0\"}");
            ((HttpServo)HeadVerticalServo).Angle = ((HttpServo)HeadHorizontalServo).Angle = 0;
            StripLed.SetColor(Convert(Color.Black));
            return msg;
        }

        /// <summary>
        /// steer PiCar haed (camera) to look straight
        /// </summary>
        public string HeadStraight()
        {
            ((HttpServo)HeadVerticalServo).Angle = ((HttpServo)HeadHorizontalServo).Angle = 0;
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
            _connected = Client.LastStatusCode == 200 && !string.IsNullOrEmpty(msg) && msg.Contains("picarclient");
            return msg;
        }

        /// <summary>
        /// reboot the PiCar server
        /// </summary>
        public string Reboot()
        {
            string msg = PostSysCommand("sleep 1; sudo reboot");
            _connected = Client.LastStatusCode == 200 && !string.IsNullOrEmpty(msg) && msg.Contains("picarclient");
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

        public string VideoStreamUrl { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        private HttpPiCar(string path, string name, HttpIotClient httpIotClient, HttpSystem httpSystem, string server, int videoPort)
            : base(path, name, null, null)
        {
            Client = httpIotClient;
            Server = httpSystem;
            Initialize(server, videoPort);
        }

        private string Initialize(string server, int videoPort)
        {
            Drive = new HttpMotor("motor", "Drive", Client, this);
            Steering = new HttpServo("servo", "Steering", Client, this) { AngleOffset = SteeringOffset };
            HeadHorizontalServo = new HttpServo("servoCamH", "HeadHorizontalServo", Client, this);
            HeadVerticalServo = new HttpServo("servoCamV", "HeadVerticalServo", Client, this);
            LeftLed = new HttpRGBLed("ledLeft", "LeftLed", Client, this);
            RightLed = new HttpRGBLed("ledRight", "RightLed", Client, this);
            StripLed = new HttpStripLed("ledStrip", "StripLed", Client, this);
            Ultrasonic = new HttpUltrasonic("ultrasonic", "Ultrasonic", Client, this);
            DistanceScan = new HttpScanDistance("scan", "DistanceScaner", Client, this);
            VideoStreamUrl = string.Format("http://{0}:{1}/{2}", server, videoPort, "");
            string msg = Reset();
            _connected = Client.LastStatusCode == 200;
            return msg;
        }

        private IotColor Convert(Color color)
        {
            return new IotColor(color.R, color.G, color.B);
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
