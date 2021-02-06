namespace Riot.Pi
{
    /// <summary>
    /// define Pi System data with http protocol
    /// </summary>
    public class SystemData : IotData
    {
        /// <summary>
        /// the cpu in the system
        /// </summary>
        public CpuData Cpu { get; set; }

        /// <summary>
        /// the memory in the system
        /// </summary>
        public MemoryData Memory { get; set; }
    }
}
