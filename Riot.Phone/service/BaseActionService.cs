using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Riot.Phone.Service
{
    /// <summary>
    /// defines the base class for all action services
    /// </summary>
    public abstract class BaseActionService : IotNode
    {
        /// <summary>
        /// whether the action need to be processed in UI thread
        /// </summary>
        public bool RequireUiThread { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public BaseActionService(string id, IotNode parent) : base (id, parent)
        { }

        /// <summary>
        /// perform the action 
        /// </summary>
        /// <param name="">the data for the action</param>
        public abstract bool Act(object data);
    }
}
