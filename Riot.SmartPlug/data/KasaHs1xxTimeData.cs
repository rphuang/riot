namespace Riot.SmartPlug
{
    /// <summary>
    /// defines the data for Kasa Smart Plug time data
    /// </summary>
    public class KasaHs1xxTimeData : IotData
    {
        /// <summary>
        /// the time value
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// the timezone from Hs1xx
        /// </summary>
        public TimeZone Timezone { get; set; }

        public class TimeZone
        {
            /// <summary>
            /// Err_code for timezone from Hs1xx
            /// </summary>
            public int Err_code { get; set; }

            /// <summary>
            /// Index for timezone from Hs1xx
            /// </summary>
            public int Index { get; set; }
        }
    }
}
