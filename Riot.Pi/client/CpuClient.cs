using HttpLib;
using Newtonsoft.Json;

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
        public CpuData CpuData
        {
            get { return Data[nameof(CpuData)] as CpuData; }
            internal set
            {
                value.Id = nameof(CpuData);
                UpsertData(value);
            }
        }

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
            CpuData = JsonConvert.DeserializeObject<CpuData>(json);
            return true;
        }
    }
}
