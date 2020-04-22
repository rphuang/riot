namespace IotClientLib
{
    /// <summary>
    /// Implements IotServo with http protocol
    /// </summary>
    public class HttpServo : HttpNode, IotServo
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HttpServo(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        #region IotServo interfaces

        /// <summary>
        /// The position of the servo in angular degree
        /// In general, the angle value range -90 to 90
        /// 0 - center
        /// positive - up of right direction
        /// negative - down or left position
        /// </summary>
        public int Angle { get; set; }

        /// <summary>
        /// The offset angle in angular degree to be applied whenever GoTo is called
        /// </summary>
        public int AngleOffset { get; set; }

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
            SetAngle(Angle + delta);
            return Post();
        }

        #endregion

        private void SetAngle(int angle)
        {
            if (angle > 90) Angle = 90;
            else if (angle < -90) Angle = -90;
            else Angle = angle;
        }

        private string Post()
        {
            string json = string.Format("{{\"{0}\": {1}}}", Id, Angle);
            return Client.Post(Parent?.Id, json);
        }
    }
}
