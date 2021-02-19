namespace Riot.IoDevice
{
    /// <summary>
    /// defines the data for ultrasonic sensor
    /// </summary>
    public class UltrasonicData : IotData
    {
        /// <summary>
        /// measured distance from the ultrasonic sensor in meter
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// the horizontal servo angle for the measured distance
        /// </summary>
        public int HorizontalAngle { get; set; }

        /// <summary>
        /// the vertical servo angle for the measured distance
        /// </summary>
        public int VerticalAngle { get; set; }
    }
}
