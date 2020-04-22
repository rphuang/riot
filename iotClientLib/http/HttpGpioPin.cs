using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotClientLib
{
    /// <summary>
    /// Implements IotGpioPin with http protocol
    /// </summary>
    public class HttpGpioPin : HttpNode, IotGpioPin
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HttpGpioPin(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        #region IotGpioPin interfaces

        /// <summary>
        /// the pin number for the IO pin
        /// </summary>
        public string Pin { get; internal set; }

        /// <summary>
        /// the mode for the IO pin
        /// </summary>
        public int Mode { get; internal set; }

        /// <summary>
        /// the value for the IO pin
        /// </summary>
        public int Value { get; internal set; }

        /// <summary>
        /// get the pin's data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        public string Get()
        {
            string json = Client.Get(FullPath);
            // deserialize
            PinIO response = JsonConvert.DeserializeObject<PinIO>(json);
            this.CopyFrom(response);
            return json;
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

        #endregion

        internal void CopyFrom(PinIO pin)
        {
            Pin = pin.Pin;
            Mode = pin.Mode;
            Value = pin.Value;
        }
    }
}
