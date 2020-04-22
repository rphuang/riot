namespace IotClientLib
{
    /// <summary>
    /// defines the properties for memory in the server
    /// </summary>
    public interface IotMemory : IotNode
    {
        /// <summary>
        /// the total amount of memory
        /// </summary>
        int Total { get; }

        /// <summary>
        /// the amount of used memory
        /// </summary>
        int Used { get; }

        /// <summary>
        /// the amount of free memory
        /// </summary>
        int Free { get; }

        /// <summary>
        /// the amount of available memory
        /// </summary>
        int Available { get; }

        /// <summary>
        /// the amount of cached memory
        /// </summary>
        int Cached { get; }

        /// <summary>
        /// get memory data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        string Get();
    }
}
