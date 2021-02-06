using System.Collections.Generic;

namespace Riot
{
    /// <summary>
    /// IIotNode defines the base interface for any composite node in RIOT
    /// </summary>
    public interface IIotNode : IIotPoint
    {
        ///// <summary>
        ///// get the status of the node
        ///// </summary>
        //IotNodeStatus Status { get; }

        /// <summary>
        /// The children nodes of this node
        /// </summary>
        IList<IIotNode> Children { get; }

        ///// <summary>
        ///// start the node
        ///// </summary>
        //void Start();

        ///// <summary>
        ///// stop the node
        ///// </summary>
        //void Stop();
    }
}
