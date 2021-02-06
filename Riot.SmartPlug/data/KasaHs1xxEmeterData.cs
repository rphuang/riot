namespace Riot.SmartPlug
{
    /// <summary>
    /// defines the data for Kasa Smart Plug emeter data
    /// </summary>
    public class KasaHs1xxEmeterData : IotData
    {
        /// <summary>
        /// the real time statistic from Hs1xx
        /// </summary>
        public double realtime { get; set; }

        /// <summary>
        /// the monthly statistic from Hs1xx
        /// </summary>
        public double Monthstat { get; set; }

        /// <summary>
        /// the daily statistic from Hs1xx
        /// </summary>
        public double Daystat { get; set; }
    }
}
