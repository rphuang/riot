using System.Collections.Generic;

namespace Riot.Pi
{
    /// <summary>
    /// defines the properties for CPU or core in the server
    /// </summary>
    public class CpuData : IotData
    {
        /// <summary>
        /// the total usage percentage for the cpu/core
        /// </summary>
        public double Usage { get; set; }

        /// <summary>
        /// the percentage used by user for the cpu/core
        /// </summary>
        public double UserUsage { get; set; }

        /// <summary>
        /// the percentage used by system for the cpu/core
        /// </summary>
        public double SystemUsage { get; set; }

        /// <summary>
        /// the idle percentage for the cpu/core
        /// </summary>
        public double Idle { get; set; }

        /// <summary>
        /// the temperature for the cpu/core
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// the cpu cores contained; only valid for the root level cpu node
        /// </summary>
        public IDictionary<string, CpuData> Cores { get; set; }
    }
}
