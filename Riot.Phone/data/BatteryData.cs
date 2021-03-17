namespace Riot.Phone
{
    /// <summary>
    /// defines the data for battery state
    /// </summary>
    public class BatteryData : WireData
    {
        /// <summary>
        /// battery ChargeLevel
        /// </summary>
        public double ChargeLevel { get; set; }

        /// <summary>
        /// battery state
        /// </summary>
        public string BatteryState { get; set; }

        /// <summary>
        /// battery PowerSource
        /// </summary>
        public string PowerSource { get; set; }

        /// <summary>
        /// battery EnergySaverStatus
        /// </summary>
        public string EnergySaverStatus { get; set; }
    }
}
