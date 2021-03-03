using HttpLib;
using Newtonsoft.Json;

namespace Riot.IoDevice.Client
{
    /// <summary>
    /// implements RIOT client node for HygroThermo sensor
    /// </summary>
    public class HygroThermoSensorClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HygroThermoSensorClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
        }

        /// <summary>
        /// data for HygroThermo sensor
        /// </summary>
        public HygroThermoData HygroThermoData { get; private set; }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            UpsertData(JsonConvert.DeserializeObject<HygroThermoData>(json));
            return true;
        }

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void UpsertData(IotData data)
        {
            HygroThermoData = data as HygroThermoData;
            base.UpsertData(HygroThermoData);
        }
    }
}
