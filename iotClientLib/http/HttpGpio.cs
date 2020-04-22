using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotClientLib
{
    /// <summary>
    /// Implements IotGpio with http protocol
    /// </summary>
    public class HttpGpio : HttpNode, IotGpio
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HttpGpio(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        #region IotGpio interfces

        /// <summary>
        /// 
        /// </summary>
        public IList<IotGpioPin> Pins { get; internal set; } = new List<IotGpioPin>();

        /// <summary>
        /// get all pins data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        public string Get()
        {
            string json = Client.Get(FullPath);
            // deserialize
            IList<PinIO> response = JsonConvert.DeserializeObject<IList<PinIO>>(json);
            Pins.Clear();
            foreach (PinIO pin in response)
            {
                HttpGpioPin httpPin = new HttpGpioPin(pin.Pin, pin.Pin, Client, this)
                    { Pin = pin.Pin, Mode = pin.Mode, Value = pin.Value };
                Pins.Add(httpPin);
                AddChildNode(httpPin);
            }
            return json;
        }

        #endregion
    }
}
