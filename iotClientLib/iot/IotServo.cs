namespace IotClientLib
{
    /// <summary>
    /// Servo defines properties and functions for a servo
    /// </summary>
    public interface IotServo : IotNode
    {
        /// <summary>
        /// The position of the servo in angular degree
        /// In general, the angle value range -90 to 90
        /// 0 - center
        /// positive - up of right direction
        /// negative - down or left position
        /// </summary>
        int Angle { get; }

        /// <summary>
        /// The offset angle in angular degree to be applied whenever GoTo is called
        /// </summary>
        int AngleOffset { get; set; }

        /// <summary>
        /// send command to move the servo to specified angle + AngleOffset
        /// </summary>
        /// <param name="angle">The physical angle from -90 to 90.</param>
        /// <returns>returns server response</returns>
        string GoTo(int angle);

        /// <summary>
        /// send command to move the servo by delta angle
        /// </summary>
        /// <param name="delta">The delta angle to move from current angle. from -90 to 90.</param>
        /// <returns>returns server response</returns>
        string GoBy(int delta);
    }
}
