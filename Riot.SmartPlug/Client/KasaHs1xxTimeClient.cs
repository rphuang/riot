using HttpLib;
using Newtonsoft.Json;

namespace Riot.SmartPlug.Client
{
    /// <summary>
    /// client side node that encapsulate time data in a Kasa Hs1xx smart plug
    /// </summary>
    public class KasaHs1xxTimeClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public KasaHs1xxTimeClient(string path, IotHttpClient client, IotNode parent) : base(path, client, parent)
        { }

        /// <summary>
        /// the system data
        /// </summary>
        public KasaHs1xxTimeData TimeData
        {
            get { return Data[nameof(TimeData)] as KasaHs1xxTimeData; }
            internal set
            {
                value.Id = nameof(TimeData);
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
            TimeData = JsonConvert.DeserializeObject<KasaHs1xxTimeData>(json);
            return true;
        }
    }
}
