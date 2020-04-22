using System.Collections.Generic;

namespace IotClientLib
{
    /// <summary>
    /// defines the properties for CPU or core in the server
    /// </summary>
    public interface IotCpu : IotNode
    {
        /// <summary>
        /// the total usage percentage for the cpu/core
        /// </summary>
        double Usage { get; }

        /// <summary>
        /// the percentage used by user for the cpu/core
        /// </summary>
        double UserUsage { get; }

        /// <summary>
        /// the percentage used by system for the cpu/core
        /// </summary>
        double SystemUsage { get; }

        /// <summary>
        /// the idle percentage for the cpu/core
        /// </summary>
        double Idle { get; }

        /// <summary>
        /// the temperature for the cpu/core
        /// </summary>
        double Temperature { get; }

        /// <summary>
        /// the cpu cores contained; only valid for the root level cpu node
        /// </summary>
        IDictionary<string, IotCpu> Cores { get; }

        /// <summary>
        /// get cpu data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        string Get();
    }
}
