using HttpLib;
using Newtonsoft.Json;

namespace Riot.SmartPlug.Client
{
    /// <summary>
    /// client side node that encapsulate emeter data in a Kasa Hs1xx smart plug
    /// </summary>
    public class KasaHs1xxEmeterClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public KasaHs1xxEmeterClient(string path, IotHttpClient client, IotNode parent) : base(path, client, parent)
        { }

        /// <summary>
        /// the system data
        /// </summary>
        public KasaHs1xxEmeterData EmeterData
        {
            get { return Data[nameof(EmeterData)] as KasaHs1xxEmeterData; }
            internal set
            {
                value.Id = nameof(EmeterData);
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
            EmeterData = JsonConvert.DeserializeObject<KasaHs1xxEmeterData>(json);
            return true;
        }
    }
}
