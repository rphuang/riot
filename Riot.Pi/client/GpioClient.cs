using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
        public GpioData GpioData { get; private set; }

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
        /// replace the current Data list with new list
        /// </summary>
        public override void ReplaceData(IotData data)
        {
            GpioData = data as GpioData;
            base.ReplaceData(GpioData);
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            ReplaceData(new GpioData
            {
                Parent = this,
                Pins = JsonConvert.DeserializeObject<List<GpioPinData>>(json)
            });
            // populate GpioPinClients' PinData
            foreach (GpioPinData pinData in GpioData.Pins)
            {
                GpioPinClients[pinData.Pin - 1].ReplaceData(pinData);
            }
            return true;
        }
    }
}
