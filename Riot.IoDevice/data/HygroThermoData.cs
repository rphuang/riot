namespace Riot.IoDevice
{
    /// <summary>
    /// defines the data properties for hydro thermo sensor
    /// </summary>
    public class HygroThermoData : IotData
    {
        /// <summary>
        /// the humidity from the sensor in %
        /// </summary>
        public double Humidity { get; set; }

        /// <summary>
        /// the Temperature from the sensor in celcius
        /// </summary>
        public double Temperature { get; set; }

    }
}
