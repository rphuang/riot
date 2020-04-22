using System.Collections.Generic;

namespace IotClientLib
{
    /// <summary>
    /// Node defines the base interface for any components in IOT
    /// </summary>
    public interface IotNode
    {
        /// <summary>
        /// the ID for the node
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// the name for the node
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The parent of this node
        /// </summary>
        IotNode Parent { get; set; }

        /// <summary>
        /// The children nodes of this node
        /// </summary>
        IList<IotNode> Children { get; }
    }
}
