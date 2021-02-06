namespace Riot.Pi
{
    /// <summary>
    /// defines the properties for memory in the server
    /// </summary>
    public class MemoryData : IotData
    {
        /// <summary>
        /// the total amount of memory
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// the amount of used memory
        /// </summary>
        public int Used { get; set; }

        /// <summary>
        /// the amount of free memory
        /// </summary>
        public int Free { get; set; }

        /// <summary>
        /// the amount of available memory
        /// </summary>
        public int Available { get; set; }

        /// <summary>
        /// the amount of cached memory
        /// </summary>
        public int Cached { get; set; }
    }
}
