namespace Riot.Phone
{
    /// <summary>
    /// defines the data for the status of phone sensor node
    /// </summary>
    public class SensorStatusData : WireData
    {
        /// <summary>
        /// get/set sensor rate for a phone sensor device
        /// </summary>
        public SensorRate SensorRate { get; set; }

        /// <summary>
        /// get/set the sensor on/off state
        /// </summary>
        public bool IsOn { get; set; }
    }
}
