using Newtonsoft.Json;
using System.Collections.Generic;

namespace Riot.Pi.Client
{
    /// <summary>
    /// implements cpu client node with http protocol
    /// </summary>
    public class CpuClient : IotClientNode
    {
        /// <summary>
        /// data for cpu
        /// </summary>
        public CpuData CpuData { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        public CpuClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            ReplaceData(JsonConvert.DeserializeObject<CpuData>(json));
            return true;
        }

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void ReplaceData(IotData data)
        {
            CpuData = data as CpuData;
            base.ReplaceData(CpuData);
        }
    }
}
