namespace Riot.Phone
{
    /// <summary>
    /// defines the sensor rate - the sensitivity/speed/accuracy of the sensor
    /// </summary>
    public enum SensorRate
    {
        /// <summary>
        /// use the default of the device
        /// </summary>
        Default,
        /// <summary>
        /// lowest speed/accuracy
        /// </summary>
        Lowest,
        /// <summary>
        /// low speed/accuracy
        /// </summary>
        Low,
        /// <summary>
        /// medium speed/accuracy
        /// </summary>
        Medium,
        /// <summary>
        /// high speed/accuracy
        /// </summary>
        High,
        /// <summary>
        /// best speed/accuracy
        /// </summary>
        Best
    }
}
