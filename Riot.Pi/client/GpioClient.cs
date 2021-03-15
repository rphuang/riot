using HttpLib;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Riot.Pi.Client
{
    /// <summary>
    /// implements Gpio client node with http protocol
    /// </summary>
    public class GpioClient : IotClientNode
    {
        /// <summary>
        /// data for gpio
        /// </summary>
        public GpioData GpioData
        {
            get { return Data[nameof(GpioData)] as GpioData; }
            internal set
            {
                value.Id = nameof(GpioData);
                UpsertData(value);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public GpioClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
            GpioData = new GpioData { Parent = this, Pins = new List<GpioPinData>() };
            for (int jj = 1; jj <= 40; jj++)
            {
                GpioPinClient pinClient = new GpioPinClient(jj.ToString(), Client, this);
                GpioPinClients.Add(pinClient);
                Children.Add(pinClient);
            }
        }

        /// <summary>
        /// pin client for all pins. access via index from 0 (pin #1) to 39 (pin 40)
        /// </summary>
        public IList<GpioPinClient> GpioPinClients { get; internal set; } = new List<GpioPinClient>();

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            GpioData = new GpioData
            {
                Parent = this,
                Pins = JsonConvert.DeserializeObject<List<GpioPinData>>(json)
            };
            // populate GpioPinClients' PinData
            foreach (GpioPinData pinData in GpioData.Pins)
            {
                GpioPinClients[pinData.Pin - 1].PinData = pinData;
            }
            return true;
        }
    }
}
