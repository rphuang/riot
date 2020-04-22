using System;
using System.Collections.Generic;
using System.Text;

namespace IotClientLib
{
    /// <summary>
    /// encapsulate distance scan action and results
    /// </summary>
    public interface IotScanDistance
    {
        /// <summary>
        /// measured list of distance values from the ultrasonic sensor
        /// </summary>
        IList<double> Distances { get; }

        /// <summary>
        /// the list of horizontal servo angles for corresponding measured distance
        /// </summary>
        IList<int> HorizontalAngles { get; }

        /// <summary>
        /// the list of vertical servo angles for corresponding measured distance
        /// </summary>
        IList<int> VerticalAngles { get; }

        /// <summary>
        /// send command to get scan distance from start to end and update the Distance and angle properties
        /// </summary>
        /// <returns>the raw response from server</returns>
        string Scan(int startHorizontal, int startVertical, int endHorizontal, int endVertical, int incHorizontal, int incVertical);
    }
}
