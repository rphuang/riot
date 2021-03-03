using HttpLib;
using Newtonsoft.Json;

namespace Riot.IoDevice.Client
{
    /// <summary>
    /// implements RIOT client node for RGB LED
    /// </summary>
    public class RGBLedClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public RGBLedClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
        }

        /// <summary>
        /// data for RGBLed
        /// </summary>
        public RGBLedData RGBLedData { get; private set; } = new RGBLedData();

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void UpsertData(IotData data)
        {
            RGBLedData = data as RGBLedData;
            base.UpsertData(RGBLedData);
        }

        /// <summary>
        /// set the LED to the specified color
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns>returns server response</returns>
        public string SetColor(RGBColor color)
        {
            RGBLedData.Color = color;
            return Post();
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            UpsertData(JsonConvert.DeserializeObject<RGBLedData>(json));
            return true;
        }

        private string Post()
        {
            const string JsonFormat = "{{\"{0}\": \"{1:000}, {2:000}, {3:000}\"}}";
            string json = string.Format(JsonFormat, Id, 
                RGBColor.ConvertColorToIOValue(RGBLedData.Color.R), RGBColor.ConvertColorToIOValue(RGBLedData.Color.G), RGBColor.ConvertColorToIOValue(RGBLedData.Color.B));
            return Client.Post(Parent?.Id, json);
        }
    }
}
