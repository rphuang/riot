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
        public KasaHs1xxEmeterData EmeterData { get; private set; }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            ReplaceData(JsonConvert.DeserializeObject<KasaHs1xxEmeterData>(json));
            return true;
        }

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void ReplaceData(IotData data)
        {
            EmeterData = data as KasaHs1xxEmeterData;
            base.ReplaceData(EmeterData);
        }
    }
}
