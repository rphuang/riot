using HttpLib;

namespace Riot.SmartPlug.Client
{
    public class SmartPlugClient : IotClientNode
    {
        /// <summary>
        /// constructor
        /// </summary>
        public SmartPlugClient(string path, IotHttpClient client) : base(path, client, null)
        { }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            return true;
        }
    }
}
