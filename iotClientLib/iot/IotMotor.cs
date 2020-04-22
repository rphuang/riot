namespace IotClientLib
{
    /// <summary>
    /// IotMotor defines properties and functions for a motor
    /// </summary>
    public interface IotMotor : IotNode
    {
        /// <summary>
        /// The speed of the motor
        /// In general, the value range -100 to 100
        /// 0 - stop
        /// positive - forward direction
        /// negative - reverse direction
        /// </summary>
        int Speed { get; }

        /// <summary>
        /// send command to move the motor at specified speed
        /// </summary>
        /// <param name="speed">The speed from -100 to 100.</param>
        /// <returns>returns server response</returns>
        string MoveAt(int speed);

        /// <summary>
        /// send command to speed up the motor at specified delta. Or slowdown if delta is negative.
        /// </summary>
        /// <param name="delta">The delta speed from -100 to 100.</param>
        /// <returns>returns server response</returns>
        string SpeedUp(int delta);
    }
}
