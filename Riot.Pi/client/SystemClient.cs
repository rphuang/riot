using Newtonsoft.Json;
using System.Collections.Generic;

namespace Riot.Pi.Client
{
    /// <summary>
    /// implements Pi System with http protocol
    /// </summary>
    public class SystemClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public SystemClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
            CpuClient = new CpuClient("cpu", Client, this);
            Children.Add(CpuClient);
            MemoryClient = new MemoryClient("memory", Client, this);
            Children.Add(MemoryClient);
        }

        /// <summary>
        /// the system data
        /// </summary>
        public SystemData SystemData { get; private set; } = new SystemData();

        /// <summary>
        /// CpuClient in the system
        /// </summary>
        public CpuClient CpuClient { get; private set; }

        /// <summary>
        /// MemoryClient in the system
        /// </summary>
        public MemoryClient MemoryClient { get; private set; }

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

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void ReplaceData(IotData data)
        {
            SystemData = data as SystemData;
            base.ReplaceData(SystemData);
        }

        /// <summary>
        /// get full path of the system node
        /// </summary>
        public override string FullPath
        {
            get
            {
                return Id;
            }
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            ReplaceData(JsonConvert.DeserializeObject<SystemData>(json));
            return true;
        }
    }
}
