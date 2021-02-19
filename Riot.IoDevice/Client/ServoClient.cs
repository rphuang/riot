using Newtonsoft.Json;

namespace Riot.IoDevice.Client
{
    /// <summary>
    /// implements RIOT client node for servo
    /// </summary>
    public class ServoClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public ServoClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
        }

        /// <summary>
        /// data for Servo
        /// </summary>
        public ServoData ServoData { get; private set; } = new ServoData();

        /// <summary>
        /// The offset angle in angular degree to be applied on the physical device
        /// </summary>
        public int AngleOffset { get; set; }

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void ReplaceData(IotData data)
        {
            ServoData = data as ServoData;
            base.ReplaceData(ServoData);
        }

        /// <summary>
        /// send command to move the servo to specified angle
        /// </summary>
        /// <param name="angle">The physical angle from -90 to 90.</param>
        /// <returns>returns server response</returns>
        public string GoTo(int angle)
        {
            SetAngle(angle + AngleOffset);
            return Post();
        }

        /// <summary>
        /// send command to move the servo by delta angle
        /// </summary>
        /// <param name="delta">The delta angle to move from current angle. from -90 to 90.</param>
        /// <returns>returns server response</returns>
        public string GoBy(int delta)
        {
            SetAngle(ServoData.Angle + delta);
            return Post();
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            ReplaceData(JsonConvert.DeserializeObject<ServoData>(json));
            return true;
        }

        private void SetAngle(int angle)
        {
            if (angle > 90) ServoData.Angle = 90;
            else if (angle < -90) ServoData.Angle = -90;
            else ServoData.Angle = angle;
        }

        private string Post()
        {
            string json = string.Format("{{\"{0}\": {1}}}", Id, ServoData.Angle);
            return Client.Post(Parent?.Id, json);
        }
    }
}
