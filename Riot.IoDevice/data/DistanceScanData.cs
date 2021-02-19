using System.Collections.Generic;

namespace Riot.IoDevice
{
    /// <summary>
    /// defines the data for distance scan
    /// </summary>
    public class DistanceScanData : IotData
    {
        /// <summary>
        /// measured list of distance values from the ultrasonic sensor
        /// </summary>
        public IList<double> Distances { get; set; }

        /// <summary>
        /// the list of horizontal servo angles for corresponding measured distance
        /// </summary>
        public IList<int> HorizontalAngles { get; set; }

        /// <summary>
        /// the list of vertical servo angles for corresponding measured distance
        /// </summary>
        public IList<int> VerticalAngles { get; set; }
    }
}
