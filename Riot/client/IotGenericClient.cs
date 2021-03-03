using HttpLib;

namespace Riot.Client
{
    /// <summary>
    /// implement a generic container for other client nodes
    /// </summary>
    public class IotGenericClient : IotClientNode
    {
        /// <summary>
        /// constructor with server:port and user:password
        /// </summary>
        public IotGenericClient(IotHttpClient client) : base(string.Empty, client, null)
        { }

        /// <summary>
        /// constructor with server:port and user:password
        /// </summary>
        public IotGenericClient(string server, string credential) : base(string.Empty, null, null)
        {
            Client = new IotHttpClient(server, credential);
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            return true;
        }
    }
}
