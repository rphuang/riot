namespace IotClientLib
{
    /// <summary>
    /// defines the computer system
    /// </summary>
    public interface IotSystem : IotNode
    {
        /// <summary>
        /// the cpu in the system. Should be included in Children proeprty.
        /// </summary>
        IotCpu Cpu { get; }

        /// <summary>
        /// the memory in the system. Should be included in Children proeprty.
        /// </summary>
        IotMemory Memory { get; }

        /// <summary>
        /// get system data (cpu, memory) from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        string Get(bool details = false);

        /// <summary>
        /// execute system command on the server
        /// </summary>
        /// <returns>response from server</returns>
        string Post(string command);
    }
}
