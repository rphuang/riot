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
        public HygroThermoData HygroThermoData
        {
            get { return Data[nameof(HygroThermoData)] as HygroThermoData; }
            internal set
            {
                value.Id = nameof(HygroThermoData);
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
            HygroThermoData = JsonConvert.DeserializeObject<HygroThermoData>(json);
            return true;
        }
    }
}
