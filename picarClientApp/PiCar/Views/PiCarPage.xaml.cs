using IotClientLib;
using PiCar.Models;
using PiCar.Services;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PiCar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PiCarPage : ContentPage
    {
        public PiCarPage(ControlTopic topic)
        {
            InitializeComponent();
            _controlTopic = topic;
            InitializePiCar();
        }

        private Random _random = new Random();

        private ControlTopic _controlTopic;
        private HttpPiCar _piCar;

        private LedLightShow _stripledSequence = LedLightShow.ledAllOFF;
        private LedState _rightLed;
        private LedState _leftLed;
        private ServoStat _cameraHorizontalServo;
        private ServoStat _cameraVertialServo;

        private static string[] PiCarModeNames = { "Manual", "Distance", "Line", "Object", "Face", "Speach" };

        private enum ClientTab
        {
            DriveTab,       // UI controls for picar's drive
            HeadTab,        // UI controls for picar's head
            CarTab,         // UI controls for picar
        }
        private ClientTab _tab = ClientTab.DriveTab;

        private void InitializePiCar()
        {
            _piCar = new HttpPiCar("EricCar", _controlTopic.ServerAddress, _controlTopic.ServerPort, 8002, _controlTopic.UserName, _controlTopic.UserPassword);
            _cameraHorizontalServo = new ServoStat { Servo = _piCar.HeadHorizontalServo };
            _cameraVertialServo = new ServoStat { Servo = _piCar.HeadVerticalServo };
            _rightLed = new LedState { Led = _piCar.RightLed };
            _leftLed = new LedState { Led = _piCar.LeftLed };
            webView.Source = _piCar.VideoStreamUrl;
        }

        private void DisplayLightStatus()
        {
            if (_tab != ClientTab.CarTab)
            {
                button12.BackgroundColor = Color.LightGray;
            }
            else
            {
                switch (_stripledSequence)
                {
                    case LedLightShow.ledAllON:
                        button12.BackgroundColor = Color.LightSalmon;
                        break;
                    case LedLightShow.ledRainbowCycle:
                        button12.BackgroundColor = Color.GreenYellow;
                        break;
                    case LedLightShow.ledTheaterChaseRainbow:
                        button12.BackgroundColor = Color.MistyRose;
                        break;
                    case LedLightShow.ledAllOFF:
                        button12.BackgroundColor = Color.LightGray;
                        break;
                }
            }
        }

        private void DisplayServerStatus(string status = null)
        {
            if (!string.IsNullOrEmpty(status))
            {
                responseLabel.Text = status;
            }
        }

        private PiCarMode FromTextToPiCarMode(string text)
        {
            PiCarMode mode = PiCarMode.Manual;
            string lowercase = text.ToLower();
            switch (lowercase)
            {
                case "distance":
                    mode = PiCarMode.Follow;
                    break;
                case "line":
                    mode = PiCarMode.Findline;
                    break;
                case "object":
                    mode = PiCarMode.Opencv;
                    break;
                case "face":
                    mode = PiCarMode.Speech;
                    break;
            }
            return mode;
        }

        private string FromPiCarModeToString(PiCarMode mode)
        {
            return PiCarModeNames[(int)mode];
        }

        private string NextPiCarModeToString(string text)
        {
            PiCarMode mode = FromTextToPiCarMode(text);
            if (mode == PiCarMode.Speech)
            {
                mode = PiCarMode.Manual;
            }
            else
            {
                mode += 1;
            }
            return FromPiCarModeToString(mode);
        }

        class LedState
        {
            public bool ButtonPressed;
            public IotRGBLed Led;
            public bool LedOn;
        }

        class ServoStat
        {
            public IotServo Servo;
            public bool ButtonPressed;
        }

        private async void ChangingServoAngle(ServoStat servo, int increment)
        {
            servo.ButtonPressed = true;
            while (servo.ButtonPressed)
            {
                string msg = servo.Servo.GoBy(increment);
                DisplayServerStatus(msg);

                await Task.Run(async () =>
                {
                    await Task.Delay(500);   // in milliseconds
                }).ConfigureAwait(true);
            }
        }

        private async void ChangingLedColor(LedState led)
        {
            led.ButtonPressed = true;
            led.LedOn = !led.LedOn;
            IotColor color = IotCommon.LedOFF;
            if (led.LedOn) color = IotCommon.LedON;
            DisplayServerStatus(led.Led.SetColor(color));

            while (led.ButtonPressed)
            {
                await Task.Run(async () =>
                {
                    await Task.Delay(1000);   // in milliseconds
                }).ConfigureAwait(true);
                if (led.ButtonPressed)
                {
                    string msg = led.Led.SetColor(GetRandomRGBLedColor());
                    DisplayServerStatus(msg);
                }
            }
        }

        /// <summary>
        /// LedLightShow available
        /// </summary>
        public enum LedLightShow
        {
            ledAllON,
            ledAllRed,
            ledAllGreen,
            ledAllBlue,
            ledRandom,
            ledRainbowCycle,
            ledTheaterChaseRainbow,
            ledAllOFF
        }

        /// <summary>
        /// turn on LED lightshow
        /// </summary>
        public string LightShow(LedLightShow ledLightShow)
        {
            string response = null;
            switch (ledLightShow)
            {
                case LedLightShow.ledAllRed:
                    response = _piCar?.StripLed.SetColor(Convert(Color.Red));
                    break;
                case LedLightShow.ledAllGreen:
                    response = _piCar?.StripLed.SetColor(Convert(Color.Green));
                    break;
                case LedLightShow.ledAllBlue:
                    response = _piCar?.StripLed.SetColor(Convert(Color.Blue));
                    break;
                case LedLightShow.ledRandom:
                    IotColor color = GetRandomColor();
                    response = _piCar?.StripLed.SetColor(color);
                    break;
                case LedLightShow.ledAllON:
                    response = _piCar?.StripLed.SetColor(Convert(Color.White));
                    break;
                case LedLightShow.ledAllOFF:
                    response = _piCar?.StripLed.SetColor(Convert(Color.Black));
                    break;
                case LedLightShow.ledRainbowCycle:
                    response = _piCar?.StripLed.SetPattern("cycle");
                    break;
                case LedLightShow.ledTheaterChaseRainbow:
                    response = _piCar?.StripLed.SetPattern("chase");
                    break;
                default:
                    response = _piCar?.StripLed.SetPattern("off");
                    break;
            }
            return response;
        }

        /// <summary>
        /// get random color for RGB LED (for each color 0- off 1- on)
        /// </summary>
        public IotColor GetRandomRGBLedColor()
        {
            return new IotColor(_random.Next(2), _random.Next(2), _random.Next(2));
        }

        /// <summary>
        /// get random color
        /// </summary>
        public IotColor GetRandomColor()
        {
            return new IotColor(_random.NextDouble(), _random.NextDouble(), _random.NextDouble());
        }

        public IotColor Convert(Color color)
        {
            return new IotColor(color.R, color.G, color.B);
        }

        private void resetButton_Clicked(object sender, EventArgs e)
        {
            string response = _piCar?.Reset();
            DisplayServerStatus(response);
        }

        private void rebootButton_Clicked(object sender, EventArgs e)
        {
            string response = _piCar?.Reboot();
            DisplayServerStatus(response);
        }

        private void shutdownButton_Clicked(object sender, EventArgs e)
        {
            string response = _piCar?.Shutdown();
            DisplayServerStatus(response);
        }

        async private void GpioButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new PiCarGpioPage(_controlTopic)));
        }

        async private void EditButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new MonitorTopicPage(_controlTopic)));
        }

        private void driveButton_Clicked(object sender, EventArgs e)
        {
            _tab = ClientTab.DriveTab;
            driveButton.BackgroundColor = Color.LightGray;
            headButton.BackgroundColor = Color.LightSlateGray;
            carButton.BackgroundColor = Color.LightSlateGray;
            button12.BackgroundColor = Color.LightGray;
            button11.Text = "Left";
            button12.Text = "Right";
            button13.Text = "Forward";
            button21.Text = "";
            button22.Text = "";
            button23.Text = "Backward";
        }

        private void headButton_Clicked(object sender, EventArgs e)
        {
            _tab = ClientTab.HeadTab;
            driveButton.BackgroundColor = Color.LightSlateGray;
            headButton.BackgroundColor = Color.LightGray;
            carButton.BackgroundColor = Color.LightSlateGray;
            button11.Text = "Look Left";
            button12.Text = "Look Up";
            button13.Text = "Look Right";
            button21.Text = "Ahead";
            button22.Text = "Down";
            button23.Text = "Distance";
            button12.BackgroundColor = Color.LightGray;
        }

        private void carButton_Clicked(object sender, EventArgs e)
        {
            _tab = ClientTab.CarTab;
            driveButton.BackgroundColor = Color.LightSlateGray;
            headButton.BackgroundColor = Color.LightSlateGray;
            carButton.BackgroundColor = Color.LightGray;
            button11.Text = "Left LED";
            button12.Text = "Strip LED";
            button13.Text = "Right LED";
            button21.Text = "Manual";
            button22.Text = "Set Mode";
            button23.Text = "Scan";
            DisplayLightStatus();
        }

        private void button11_Clicked(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // NOP
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button11_Pressed(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // move left
                    response = _piCar?.Steering.GoTo(-Settings.Instance.SteeringAngle);
                    break;
                case ClientTab.HeadTab:     // look left
                    ChangingServoAngle(_cameraHorizontalServo, -Settings.Instance.DeltaCameraAngle);
                    break;
                case ClientTab.CarTab:      // left LED
                    ChangingLedColor(_leftLed);
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button11_Released(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // Steer Straight
                    response = _piCar?.SteerStraight();
                    break;
                case ClientTab.HeadTab:     // look left
                    _cameraHorizontalServo.ButtonPressed = false;
                    break;
                case ClientTab.CarTab:      // left LED
                    _leftLed.ButtonPressed = false;
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button12_Clicked(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // NOP
                    break;
                case ClientTab.CarTab:      // strip LED
                    if (_stripledSequence == LedLightShow.ledAllOFF) _stripledSequence = LedLightShow.ledAllON;
                    else _stripledSequence++;
                    response = LightShow(_stripledSequence);
                    break;
            }
            DisplayServerStatus(response);
            DisplayLightStatus();
        }

        private void button12_Pressed(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // move right
                    response = _piCar?.Steering.GoTo(Settings.Instance.SteeringAngle);
                    break;
                case ClientTab.HeadTab:     // look up
                    ChangingServoAngle(_cameraVertialServo, Settings.Instance.DeltaCameraAngle);
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button12_Released(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // Steer Straight
                    response = _piCar?.SteerStraight();
                    break;
                case ClientTab.HeadTab:     // look up
                    _cameraVertialServo.ButtonPressed = false;
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button13_Clicked(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // NOP
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button13_Pressed(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // move forward
                    response = _piCar?.Drive.MoveAt(Settings.Instance.MotorSpeed);
                    break;
                case ClientTab.HeadTab:     // look right
                    ChangingServoAngle(_cameraHorizontalServo, Settings.Instance.DeltaCameraAngle);
                    break;
                case ClientTab.CarTab:      // right LED
                    ChangingLedColor(_rightLed);
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button13_Released(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // move stop
                    response = _piCar?.Drive.MoveAt(0);
                    break;
                case ClientTab.HeadTab:     // look right
                    _cameraHorizontalServo.ButtonPressed = false;
                    break;
                case ClientTab.CarTab:      // right LED
                    _rightLed.ButtonPressed = false;
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button21_Clicked(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // look straight
                    _piCar?.HeadStraight();
                    break;
                case ClientTab.CarTab:      // Mode
                    button21.Text = NextPiCarModeToString(button21.Text);
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button21_Pressed(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // NOP
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button21_Released(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // NOP
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button22_Clicked(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // NOP
                    break;
                case ClientTab.CarTab:      // set mode
                    PiCarMode mode = FromTextToPiCarMode(button21.Text);
                    response = _piCar.SetMode(mode);
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button22_Pressed(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // look down
                    ChangingServoAngle(_cameraVertialServo, -Settings.Instance.DeltaCameraAngle);
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button22_Released(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // look down
                    _cameraVertialServo.ButtonPressed = false;
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button23_Clicked(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // NOP
                    break;
                case ClientTab.HeadTab:     // distance
                    response = _piCar.Ultrasonic.MeasureDistance();
                    break;
                case ClientTab.CarTab:      // scan
                    // todo: user specified start/end/inc
                    response = _piCar.DistanceScan.Scan(Settings.Instance.HorizontalStartAngle, Settings.Instance.VerticalStartAngle,
                        Settings.Instance.HorizontalEndAngle, Settings.Instance.VerticalEndAngle, Settings.Instance.HorizontalIncAngle, Settings.Instance.VerticalIncAngle);
                    break;
            }
            DisplayServerStatus(response);

        }

        private void button23_Pressed(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // move backward
                    response = _piCar?.Drive.MoveAt(-Settings.Instance.MotorSpeed);
                    break;
                case ClientTab.HeadTab:     // NOP
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }

        private void button23_Released(object sender, EventArgs e)
        {
            string response = string.Empty;
            switch (_tab)
            {
                case ClientTab.DriveTab:    // move stop
                    response = _piCar?.Drive.MoveAt(0);
                    break;
                case ClientTab.HeadTab:     // NOP
                    break;
                case ClientTab.CarTab:      // NOP
                    break;
            }
            DisplayServerStatus(response);
        }
    }
}