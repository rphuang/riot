namespace IotClientLib
{
    /// <summary>
    /// Encapsulate an ultrasonic sensor
    /// </summary>
    public interface IotUltrasonic : IotNode
    {
        /// <summary>
        /// measured distance from the ultrasonic sensor
        /// </summary>
        double Distance { get; }

        /// <summary>
        /// the horizontal servo angle for the measured distance
        /// </summary>
        int HorizontalAngle { get; }

        /// <summary>
        /// the vertical servo angle for the measured distance
        /// </summary>
        int VerticalAngle { get; }

        /// <summary>
        /// send command to get measured distance from the sensor and update the Distance and angle properties
        /// </summary>
        /// <returns>the raw response from server</returns>
        string MeasureDistance();
    }
}
