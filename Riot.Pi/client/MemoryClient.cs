using Newtonsoft.Json;
using System.Collections.Generic;

namespace Riot.Pi.Client
{
    /// <summary>
    /// implements IotMemory with http protocol
    /// </summary>
    public class MemoryClient : IotClientNode
    {
        /// <summary>
        /// data for memory
        /// </summary>
        public MemoryData MemoryData { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        public MemoryClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
        }

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void ReplaceData(IotData data)
        {
            MemoryData = data as MemoryData;
            base.ReplaceData(MemoryData);
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            ReplaceData(JsonConvert.DeserializeObject<MemoryData>(json));
            return true;
        }
    }
}
