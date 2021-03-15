using HttpLib;
using Newtonsoft.Json;

namespace Riot.IoDevice.Client
{
    /// <summary>
    /// implements RIOT client node for motor
    /// </summary>
    public class MotorClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public MotorClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
            MotorData = new MotorData();
        }

        /// <summary>
        /// data for Motor
        /// </summary>
        public MotorData MotorData
        {
            get { return Data[nameof(MotorData)] as MotorData; }
            internal set
            {
                value.Id = nameof(MotorData);
                UpsertData(value);
            }
        }

        /// <summary>
        /// send command to move the motor at specified speed
        /// </summary>
        /// <param name="speed">The speed from -100 to 100.</param>
        /// <returns>returns server response</returns>
        public string MoveAt(int speed)
        {
            SetSpeed(speed);
            return Post();
        }

        /// <summary>
        /// send command to speed up the motor at specified delta. Or slowdown if delta is negative.
        /// </summary>
        /// <param name="delta">The delta speed from -100 to 100.</param>
        /// <returns>returns server response</returns>
        public string SpeedUp(int delta)
        {
            SetSpeed(MotorData.Speed + delta);
            return Post();
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            MotorData = JsonConvert.DeserializeObject<MotorData>(json);
            return true;
        }

        private void SetSpeed(int speed)
        {
            if (speed > 100) MotorData.Speed = 100;
            else if (speed < -100) MotorData.Speed = -100;
            else MotorData.Speed = speed;
        }

        private string Post()
        {
            string json = string.Format("{{\"{0}\": {1}}}", Id, MotorData.Speed);
            return Client.Post(Parent?.Id, json);
        }
    }
}
