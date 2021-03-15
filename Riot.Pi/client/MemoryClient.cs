using HttpLib;
using Newtonsoft.Json;

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
        public MemoryData MemoryData
        {
            get { return Data[nameof(MemoryData)] as MemoryData; }
            internal set
            {
                value.Id = nameof(MemoryData);
                UpsertData(value);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public MemoryClient(string id, IotHttpClient client, IotNode parent)
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
            MemoryData = JsonConvert.DeserializeObject<MemoryData>(json);
            return true;
        }
    }
}
