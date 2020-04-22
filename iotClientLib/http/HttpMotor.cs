namespace IotClientLib
{
    /// <summary>
    /// Implements IotMotor with http protocol
    /// </summary>
    public class HttpMotor : HttpNode, IotMotor
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HttpMotor(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        #region IotMotor interfaces

        /// <summary>
        /// The speed of the motor
        /// In general, the value range -100 to 100
        /// 0 - stop
        /// positive - forward direction
        /// negative - reverse direction
        /// </summary>
        public int Speed { get; internal set; }

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
            SetSpeed(Speed + delta);
            return Post();
        }

        #endregion

        private void SetSpeed(int speed)
        {
            if (speed > 100) Speed = 100;
            else if (speed < -100) Speed = -100;
            else Speed = speed;
        }

        private string Post()
        {
            string json = string.Format("{{\"{0}\": {1}}}", Id, Speed);
            return Client.Post(Parent?.Id, json);
        }
    }
}
