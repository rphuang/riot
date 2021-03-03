using System;
using System.Collections.Generic;
using System.Text;

namespace Riot
{
    /// <summary>
    /// defines the interface to send notification
    /// 1. clients Subscribe for change notification with target site - address, credential, and optional token
    /// 2. services Notify new data to all clients using http post to subscribed client target sites
    /// </summary>
    public interface INotify
    {
        /// <summary>
        /// send notification to subscribed clients
        /// </summary>
        void SendNotification();
    }
}
