using HttpLib;
using Newtonsoft.Json;

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
        public KasaHs1xxSystemData SystemData
        {
            get { return Data[nameof(SystemData)] as KasaHs1xxSystemData; }
            internal set
            {
                value.Id = nameof(SystemData);
                UpsertData(value);
            }
        }

        /// <summary>
        /// turn the smart plug on or off
        /// </summary>
        public string TurnPlugOnOff(bool on)
        {
            string json = string.Format("{{\"relay_state\": {0}}}", on? 1 : 0);
            return Post(json);
        }

        /// <summary>
        /// set the LED off status - true: LED always off
        /// </summary>
        public string SetLedAlwaysOff(bool on)
        {
            string json = string.Format("{{\"led_off\": {0}}}", on ? 1 : 0);
            return Post(json);
        }

        /// <summary>
        /// post/put data to the server
        /// </summary>
        /// <returns>response from server</returns>
        public string Post(string json)
        {
            //string json = string.Format("{{\"cmd\": \"{0}\"}}", command);
            string msg = Client.Post(FullPath, json);
            return msg;
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            SystemData = JsonConvert.DeserializeObject<KasaHs1xxSystemData>(json);
            return true;
        }
    }
}
