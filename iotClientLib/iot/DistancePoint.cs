using System;
using System.Collections.Generic;
using System.Text;

namespace IotClientLib
{
    /// <summary>
    /// represents a measured distance point
    /// </summary>
    public class DistancePoint
    {
        /// <summary>
        /// measured distance from the ultrasonic sensor
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
