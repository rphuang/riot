using Newtonsoft.Json;
using System;

namespace Riot.IoDevice.Client
{
    /// <summary>
    /// implements RIOT client node for distance scan
    /// </summary>
    public class DistanceScanClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public DistanceScanClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
        }

        /// <summary>
        /// data for DistanceScan
        /// </summary>
        public DistanceScanData DistanceScanData { get; private set; } = new DistanceScanData();

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            ReplaceData(JsonConvert.DeserializeObject<DistanceScanData>(json));
            return true;
        }

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void ReplaceData(IotData data)
        {
            DistanceScanData = data as DistanceScanData;
            base.ReplaceData(DistanceScanData);
        }

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
                ReplaceData(JsonConvert.DeserializeObject<DistanceScanData>(jsonResponse));
                return jsonResponse;
            }
            catch (Exception err)
            {
                return "Exception: " + err.Message;
            }
        }
    }
}
