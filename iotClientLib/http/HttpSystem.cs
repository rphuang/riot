using Newtonsoft.Json;

namespace IotClientLib
{
    /// <summary>
    /// implements IotSystem with http protocol
    /// </summary>
    public class HttpSystem : HttpNode, IotSystem
    {
        public HttpSystem()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        public HttpSystem(string name, HttpIotClient client, HttpNode parent)
            : base("sys", name, client, parent)
        {
            Cpu = new HttpCpu("cpu", name, client, this);
            Memory = new HttpMemory("memory", name, client, this);
            AddChildNode(Cpu);
            AddChildNode(Memory);
        }

        #region IotSystem interfaces

        /// <summary>
        /// the cpu in the system. Should be included in Children proeprty.
        /// </summary>
        public IotCpu Cpu { get; }

        /// <summary>
        /// the memory in the system. Should be included in Children proeprty.
        /// </summary>
        public IotMemory Memory { get; }

        /// <summary>
        /// get system data (cpu, memory) from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        public string Get(bool details = false)
        {
            string json = Client.Get(FullPath);
            // deserialize
            Sys response = JsonConvert.DeserializeObject<Sys>(json);
            ((HttpCpu)Cpu).CopyFrom(response.Cpu);
            ((HttpMemory)Memory).CopyFrom(response.Memory);
            return json;
        }

        /// <summary>
        /// execute system command on the server
        /// </summary>
        /// <returns>response from server</returns>
        public string Post(string command)
        {
            string json = string.Format("{{\"cmd\": \"{0}\"}}", command);
            string msg = Client.Post(FullPath, json);
            return msg;
        }

        #endregion

        /// <summary>
        /// get full path of the node
        /// </summary>
        protected override string FullPath
        {
            get
            {
                return Id;
            }
        }
    }
}
