using Newtonsoft.Json;

namespace IotClientLib
{
    /// <summary>
    /// implement the interface to a Hygro-Thermo Sensor via HTTP
    /// </summary>
    public class HttpHygroThermoSensor : HttpNode, IotHygroThermoSensor
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HttpHygroThermoSensor(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        #region IotHygroThermoSensor interfaces

        /// <summary>
        /// the humidity from the sensor
        /// </summary>
        public double Humidity { get; internal set; }

        /// <summary>
        /// the Temperature from the sensor in celcius
        /// </summary>
        public double Temperature { get; internal set; }

        /// <summary>
        /// get the sensor data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        public string Get()
        {
            string json = Client.Get(FullPath);
            // deserialize
            HygroThermoData response = JsonConvert.DeserializeObject<HygroThermoData>(json);
            this.CopyFrom(response);
            return json;
        }

        #endregion

        internal void CopyFrom(HygroThermoData data)
        {
            Temperature = data.Temperature;
            Humidity = data.Humidity;
        }
    }
}
