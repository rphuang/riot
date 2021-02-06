using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.SmartPlug.Client
{
    /// <summary>
    /// client side node that encapsulate a Kasa Hs1xx smart plug
    /// </summary>
    public class KasaHs1xxClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public KasaHs1xxClient(string path, IotHttpClient client) : base(path, client, null)
        {
            System = new KasaHs1xxSystemClient("system", client, this);
            Children.Add(System);
            Time = new KasaHs1xxTimeClient("time", client, this);
            Children.Add(Time);
            Emeter = new KasaHs1xxEmeterClient("emeter", client, this);
            Children.Add(Emeter);
        }

        /// <summary>
        /// the system client
        /// </summary>
        public KasaHs1xxSystemClient System { get; private set; }

        /// <summary>
        /// the time client
        /// </summary>
        public KasaHs1xxTimeClient Time { get; private set; }

        /// <summary>
        /// the emeter client
        /// </summary>
        public KasaHs1xxEmeterClient Emeter { get; private set; }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            return true;
        }
    }
}
