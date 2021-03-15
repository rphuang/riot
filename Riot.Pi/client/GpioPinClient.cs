using HttpLib;
using Newtonsoft.Json;

namespace Riot.Pi.Client
{
    /// <summary>
    /// implements Gpio Pin client node with http protocol
    /// </summary>
    public class GpioPinClient : IotClientNode
    {
        /// <summary>
        /// data for gpio pin
        /// </summary>
        public GpioPinData PinData
        {
            get { return Data[nameof(PinData)] as GpioPinData; }
            internal set
            {
                value.Id = nameof(PinData);
                UpsertData(value);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public GpioPinClient(string id, IotHttpClient client, IotNode parent)
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
            PinData = JsonConvert.DeserializeObject<GpioPinData>(json);
            return true;
        }

        /// <summary>
        /// set mode of the pin on the server
        /// </summary>
        /// <returns>response from server</returns>
        public string SetMode(int mode)
        {
            string json = string.Format("{{\"mode\": {0}}}", mode);
            return Client.Post(FullPath, json);
        }

        /// <summary>
        /// set value of the pin on the server
        /// </summary>
        /// <returns>response from server</returns>
        public string SetValue(int value)
        {
            string json = string.Format("{{\"value\": {0}}}", value);
            return Client.Post(FullPath, json);
        }
    }
}
