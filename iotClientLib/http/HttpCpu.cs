using Newtonsoft.Json;
using System.Collections.Generic;

namespace IotClientLib
{
    /// <summary>
    /// implements IotCpu with http protocol
    /// </summary>
    public class HttpCpu : HttpNode, IotCpu
    {
        #region IotCpu interfaces

        /// <summary>
        /// the usage percentage for the cpu/core
        /// </summary>
        public double Usage { get; internal set; }

        /// <summary>
        /// the percentage used by user for the cpu/core
        /// </summary>
        public double UserUsage { get; internal set; }

        /// <summary>
        /// the percentage used by system for the cpu/core
        /// </summary>
        public double SystemUsage { get; internal set; }

        /// <summary>
        /// the idle percentage for the cpu/core
        /// </summary>
        public double Idle { get; internal set; }

        /// <summary>
        /// the temperature for the cpu/core
        /// </summary>
        public double Temperature { get; internal set; }

        /// <summary>
        /// the cpu cores contained; only valid for the root level cpu node
        /// </summary>
        public IDictionary<string, IotCpu> Cores { get; internal set; }

        /// <summary>
        /// get cpu data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        public string Get()
        {
            string json = Client.Get(FullPath);
            // deserialize
            Cpu response = JsonConvert.DeserializeObject<Cpu>(json);
            CopyFrom(response);
            return json;
        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public HttpCpu(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        internal void CopyFrom(Cpu cpu)
        {
            Usage = cpu.Usage;
            UserUsage = cpu.UserUsage;
            SystemUsage = cpu.SystemUsage;
            Idle = cpu.Idle;
            Temperature = cpu.Temperature;
            if (cpu.Cores != null)
            {
                if (Cores == null) Cores = new Dictionary<string, IotCpu>();
                Cores.Clear();
                foreach (var core in cpu.Cores)
                {
                    HttpCpu httpCore = new HttpCpu(core.Key, core.Key, Client, this);
                    Cores.Add(core.Key, httpCore);
                    httpCore.CopyFrom(core.Value);
                }
            }
        }
    }
}
