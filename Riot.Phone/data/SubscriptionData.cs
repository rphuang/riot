using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.Phone
{
    /// <summary>
    /// defines an item to subscribe
    /// </summary>
    public class SubscriptionItem
    {
        /// <summary>
        /// the node ID
        /// </summary>
        public string Node { get; set; }

        /// <summary>
        /// the data ID
        /// </summary>
        public string Data { get; set; }
    }

    /// <summary>
    /// defines data for subscribing push notification
    /// </summary>
    public class SubscriptionData : WireData
    {
        /// <summary>
        /// the target site to send notification
        /// </summary>
        public HttpTargetSite Target { get; set; }

        /// <summary>
        /// list of items to subscribe
        /// </summary>
        public IList<SubscriptionItem> Items { get; set; }
    }
}
