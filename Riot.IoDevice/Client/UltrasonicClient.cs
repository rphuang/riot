using HttpLib;
using Newtonsoft.Json;

namespace Riot.IoDevice.Client
{
    /// <summary>
    /// implements RIOT client node for ultrasonic sensor
    /// </summary>
    public class UltrasonicClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public UltrasonicClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
            UltrasonicData = new UltrasonicData();
        }

        /// <summary>
        /// data for Ultrasonic
        /// </summary>
        public UltrasonicData UltrasonicData
        {
            get { return Data[nameof(UltrasonicData)] as UltrasonicData; }
            internal set
            {
                value.Id = nameof(UltrasonicData);
                UpsertData(value);
            }
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            UltrasonicData = JsonConvert.DeserializeObject<UltrasonicData>(json);
            return true;
        }

        /// <summary>
        /// send command to get measured distance from the sensor and update the Distance and angle properties
        /// </summary>
        /// <returns>the raw response from server</returns>
        public string MeasureDistance()
        {
            // todo: impl. post to get realtime data
            // for now get the latest measured data
            return Get();
        }
    }
}
