using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.SmartPlug.Client
{
    /// <summary>
    /// client side node that encapsulate system data in a Kasa Hs1xx smart plug
    /// </summary>
    public class KasaHs1xxSystemClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public KasaHs1xxSystemClient(string path, IotHttpClient client, IotNode parent) : base(path, client, parent)
        { }

        /// <summary>
        /// the system data
        /// </summary>
        public KasaHs1xxSystemData SystemData { get; private set; }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            ReplaceData(JsonConvert.DeserializeObject<KasaHs1xxSystemData>(json));
            return true;
        }

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void ReplaceData(IotData data)
        {
            SystemData = data as KasaHs1xxSystemData;
            base.ReplaceData(SystemData);
        }
    }
}
