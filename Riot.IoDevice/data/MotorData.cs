namespace Riot.IoDevice
{
    /// <summary>
    /// defines the data for DC motor
    /// </summary>
    public class MotorData : IotData
    {
        /// <summary>
        /// The speed of the motor
        /// In general, the value range -100 to 100
        /// 0 - stop
        /// positive - forward direction
        /// negative - reverse direction
        /// </summary>
        public int Speed { get; set; }
    }
}
