using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotClientLib
{
    /// <summary>
    /// Encapsulate an ultrasonic sensor with HTTP protocol
    /// </summary>
    public class HttpUltrasonic : HttpNode, IotUltrasonic
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HttpUltrasonic(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        /// <summary>
        /// measured distance from the ultrasonic sensor
        /// </summary>
        public double Distance { get; internal set; }

        /// <summary>
        /// the horizontal servo angle for the measured distance
        /// </summary>
        public int HorizontalAngle { get; internal set; }

        /// <summary>
        /// the vertical servo angle for the measured distance
        /// </summary>
        public int VerticalAngle { get; internal set; }

        /// <summary>
        /// send command to get measured distance from the sensor and update the Distance and angle properties
        /// </summary>
        /// <returns>the raw response from server</returns>
        public string MeasureDistance()
        {
            string json = Client.Get(FullPath);
            // deserialize
            UltrasonicData response = JsonConvert.DeserializeObject<UltrasonicData>(json);
            CopyFrom(response);
            return json;
        }

        internal void CopyFrom(UltrasonicData data)
        {
            Distance = data.Value;
            HorizontalAngle = data.PosH;
            VerticalAngle = data.PosV;
        }
    }
}
