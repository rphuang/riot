using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotClientLib
{
    /// <summary>
    /// implement IotScanDistance (distance scan action and results) with HTTP protocol
    /// </summary>
    public class HttpScanDistance : HttpNode, IotScanDistance
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HttpScanDistance(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        /// <summary>
        /// measured list of distance values from the ultrasonic sensor
        /// </summary>
        public IList<double> Distances { get; internal set; }

        /// <summary>
        /// the list of horizontal servo angles for corresponding measured distance
        /// </summary>
        public IList<int> HorizontalAngles { get; internal set; }

        /// <summary>
        /// the list of vertical servo angles for corresponding measured distance
        /// </summary>
        public IList<int> VerticalAngles { get; internal set; }

        /// <summary>
        /// send command to get scan distance from start to end and update the Distance and angle properties
        /// </summary>
        /// <returns>the raw response from server</returns>
        public string Scan(int startHorizontal, int startVertical, int endHorizontal, int endVertical, int incHorizontal, int incVertical)
        {
            try
            {
                string jsonBody = string.Format("{{\"starth\": {0},\"startv\": {1},\"endh\": {2},\"endv\": {3},\"inch\": {4},\"incv\": {5} }}",
                    startHorizontal, startVertical, endHorizontal, endVertical, incHorizontal, incVertical);
                string jsonResponse = Client.Post(FullPath, jsonBody);
                // deserialize
                ScanDistanceData response = JsonConvert.DeserializeObject<ScanDistanceData>(jsonResponse);
                CopyFrom(response);
                return jsonResponse;
            }
            catch (Exception err)
            {
                return "Exception: " + err.Message;
            }
        }

        internal void CopyFrom(ScanDistanceData data)
        {
            Distances = data.Value;
            HorizontalAngles = data.PosH;
            VerticalAngles = data.PosV;
        }
    }
}
