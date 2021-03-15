using HttpLib;
using Newtonsoft.Json;

namespace Riot.SmartPlug.Client
{
    /// <summary>
    /// client side node that encapsulate a Kasa Hs1xx smart plug
    /// </summary>
    public class KasaHs1xxClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public KasaHs1xxClient(string path, IotHttpClient client) : base(path, client, null)
        {
            System = new KasaHs1xxSystemClient("system", client, this);
            Children.Add(System);
            Time = new KasaHs1xxTimeClient("time", client, this);
            Children.Add(Time);
            Emeter = new KasaHs1xxEmeterClient("emeter", client, this);
            Children.Add(Emeter);
        }

        /// <summary>
        /// the system client
        /// </summary>
        public KasaHs1xxSystemClient System { get; private set; }

        /// <summary>
        /// the time client
        /// </summary>
        public KasaHs1xxTimeClient Time { get; private set; }

        /// <summary>
        /// the emeter client
        /// </summary>
        public KasaHs1xxEmeterClient Emeter { get; private set; }

        /// <summary>
        /// turn the device on or off
        /// </summary>
        public string TurnPlugOnOff(bool on)
        {
            return System.TurnPlugOnOff(on);
        }

        /// <summary>
        /// set the LED off status - true: LED always off
        /// </summary>
        public string SetLedAlwaysOff(bool on)
        {
            return System.SetLedAlwaysOff(on);
        }

        /// <summary>
        /// send command to reboot the device
        /// </summary>
        public string Reboot(int delay)
        {
            return PostCommand("reboot", delay);
        }

        /// <summary>
        /// send command to reset the device
        /// </summary>
        public string Reset(int delay)
        {
            return PostCommand("reset", delay);
        }

        /// <summary>
        /// execute system command on the server
        /// </summary>
        /// <returns>response from server</returns>
        public string PostCommand(string command, int delay)
        {
            string json = string.Format("{{\"delay\": {0}}}", delay);
            string msg = Client.Post($"{FullPath}/cmd/{command}", json);
            return msg;
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            System.SystemData = JsonConvert.DeserializeObject<KasaHs1xxSystemData>(json);
            return true;
        }
    }
}
