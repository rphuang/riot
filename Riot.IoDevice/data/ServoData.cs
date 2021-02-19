namespace Riot.IoDevice
{
    /// <summary>
    /// defines the data for servo
    /// </summary>
    public class ServoData : IotData
    {
        /// <summary>
        /// The position of the servo in angular degree
        /// In general, the angle value range -90 to 90
        /// 0 - center
        /// positive - up of right direction
        /// negative - down or left position
        /// </summary>
        public int Angle { get; set; }

        /// <summary>
        /// The offset angle in angular degree to be applied on the physical device
        /// </summary>
        public int AngleOffset { get; set; }
    }
}
