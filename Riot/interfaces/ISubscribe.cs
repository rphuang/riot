namespace Riot
{
    /// <summary>
    /// defines the interface to subscribe push notification
    /// 1. clients Subscribe for change notification with target site - address, credential, and optional token
    /// 2. server sends new data to all clients using http post to subscribed client target sites
    /// </summary>
    public interface ISubscribe
    {
        /// <summary>
        /// subscribe data change notification
        /// </summary>
        /// <param name="targetAddress">the target http service's address to receive notification (ipaddress:port)</param>
        /// <param name="credential">the credential for http requests post to target service (user:password)</param>
        /// <param name="token">optional token to be included for every http requests</param>
        void Subscribe(string targetAddress, string credential, string token);

        /// <summary>
        /// subscribe data change notification
        /// </summary>
        void Subscribe(HttpTargetSite endpoint);
    }
}
