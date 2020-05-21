namespace IotClientLib
{
    /// <summary>
    /// encapsulate the interface to a Hygro-Thermo Sensor
    /// </summary>
    public interface IotHygroThermoSensor : IotNode
    {
        /// <summary>
        /// the humidity from the sensor
        /// </summary>
        double Humidity { get; }

        /// <summary>
        /// the Temperature from the sensor in celcius
        /// </summary>
        double Temperature { get; }

        /// <summary>
        /// get the sensor data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        string Get();
    }
}
