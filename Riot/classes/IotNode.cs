using System.Collections.Generic;

namespace Riot
{
    /// <summary>
    /// IotNode defines the base class for any composite node in RIOT
    /// </summary>
    public class IotNode : IotPoint, IIotNode
    {
        /// <summary>
        /// The data points of this node
        /// </summary>
        public IList<IotData> Data { get; private set; } = new List<IotData>();

        /// <summary>
        /// The children nodes of this node
        /// </summary>
        public IList<IIotNode> Children { get; } = new List<IIotNode>();

        /// <summary>
        /// add a child node
        /// </summary>
        public void AddNode(IIotNode node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        /// <summary>
        /// replace the current Data list with single item
        /// </summary>
        public virtual void ReplaceData(IotData data)
        {
            Data.Clear();
            Data.Add(data);
        }

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public virtual void ReplaceData(IList<IotData> data)
        {
            if (data != null) Data = data;
            else Data.Clear();
        }

        /// <summary>
        /// constructor
        /// </summary>
        protected IotNode()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        protected IotNode(string id, IIotPoint parent) : base(id, parent)
        {
        }
    }
}
