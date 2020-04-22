using Newtonsoft.Json;

namespace IotClientLib
{
    /// <summary>
    /// implements IotMemory with http protocol
    /// </summary>
    public class HttpMemory : HttpNode, IotMemory
    {
        #region IotMemory interfaces

        /// <summary>
        /// the total amount of memory
        /// </summary>
        public int Total { get; internal set; }

        /// <summary>
        /// the amount of used memory
        /// </summary>
        public int Used { get; internal set; }

        /// <summary>
        /// the amount of free memory
        /// </summary>
        public int Free { get; internal set; }

        /// <summary>
        /// the amount of available memory
        /// </summary>
        public int Available { get; internal set; }

        /// <summary>
        /// the amount of cached memory
        /// </summary>
        public int Cached { get; internal set; }

        /// <summary>
        /// get memory data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        public string Get()
        {
            string json = Client.Get(FullPath);
            // deserialize
            Memory response = JsonConvert.DeserializeObject<Memory>(json);
            this.CopyFrom(response);
            return json;
        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public HttpMemory(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        internal void CopyFrom(Memory other)
        {
            Total = other.Total;
            Used = other.Used;
            Cached = other.Cached;
            Free = other.Free;
            Available = other.Available;
        }
    }
}
