namespace Riot.SmartPlug
{
    /// <summary>
    /// defines the data for Kasa Smart Plug system data
    /// </summary>
    public class KasaHs1xxSystemData : IotData
    {
        /// <summary>
        /// the device IP Address (server:port) 
        /// </summary>
        public string Ipaddress { get; set; }

        /// <summary>
        /// the device ID 
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// the hardware ID 
        /// </summary>
        public string HwId { get; set; }

        /// <summary>
        /// the mac address 
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// the device name 
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// the device Led state off 
        /// </summary>
        public int Led_off { get; set; }

        /// <summary>
        /// the device state (the relay state on/off) 
        /// </summary>
        public int Relay_state { get; set; }

        /// <summary>
        /// the latitude 
        /// </summary>
        public int Latitude_i { get; set; }

        /// <summary>
        /// the Longitude 
        /// </summary>
        public int Longitude_i { get; set; }

        /// <summary>
        /// the model of the smart plug
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Wifi signal strength
        /// </summary>
        public int Rssi { get; set; }

        /// <summary>
        /// active_mode
        /// </summary>
        public string Active_mode { get; set; }

        /// <summary>
        /// dev_name
        /// </summary>
        public string Dev_name { get; set; }

        /// <summary>
        /// err_code
        /// </summary>
        public int Err_code { get; set; }

        /// <summary>
        /// feature
        /// </summary>
        public string Feature { get; set; }

        /// <summary>
        /// hw_ver
        /// </summary>
        public string hw_ver { get; set; }

        /// <summary>
        /// icon_hash
        /// </summary>
        public string Icon_hash { get; set; }

        /// <summary>
        /// mic_type
        /// </summary>
        public string Mic_type { get; set; }

        /// <summary>
        /// NextAction
        /// </summary>
        public NextAction next_action { get; set; }

        /// <summary>
        /// ntc_state
        /// </summary>
        public int Ntc_state { get; set; }

        /// <summary>
        /// oemId
        /// </summary>
        public string OemId { get; set; }

        /// <summary>
        /// On_time
        /// </summary>
        public int On_time { get; set; }

        /// <summary>
        /// status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// sw_ver
        /// </summary>
        public string Sw_ver { get; set; }

        /// <summary>
        /// updating
        /// </summary>
        public int Updating { get; set; }

        /// <summary>
        /// NextAction properties
        /// </summary>
        public class NextAction
        {
            /// <summary>
            /// the action 
            /// </summary>
            public int Action { get; set; }

            /// <summary>
            /// the ID 
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// the scheduled time in seconds start from midnight 
            /// </summary>
            public int schd_sec { get; set; }

            /// <summary>
            /// the action type
            /// </summary>
            public int Type { get; set; }
        }
    }
}
