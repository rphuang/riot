using System.Collections.Generic;

namespace IotClientLib
{
    /// <summary>
    /// Implements IotNode with http protocol
    /// A base class for all nodes using http protocol to communicate to server
    /// </summary>
    public class HttpNode : IotNode
    {
        #region IotNode interfaces

        /// <summary>
        /// the ID for the node
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name for the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parent of this node
        /// </summary>
        public IotNode Parent { get; set; }

        /// <summary>
        /// The children nodes of this node
        /// </summary>
        public IList<IotNode> Children { get; private set; } = new List<IotNode>();

        #endregion

        /// <summary>
        /// add a child node to this node
        /// </summary>
        public void AddChildNode(IotNode node)
        {
            if (!Children.Contains(node)) Children.Add(node);
        }

        /// <summary>
        /// HttpClient used to communicate to server
        /// </summary>
        public HttpIotClient Client { get; protected set; }

        /// <summary>
        /// constructor
        /// </summary>
        protected HttpNode() { }

        /// <summary>
        /// constructor
        /// </summary>
        protected HttpNode(string id, string name, HttpIotClient client, IotNode parent)
        {
            Id = id;
            Name = name;
            Client = client;
            Parent = parent;
        }

        /// <summary>
        /// convert color value (0 - 1 double) to IO value (0 - 255 int)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected int ConvertColorValue(double value)
        {
            return (int)(value * 255);
        }

        /// <summary>
        /// get full path of the node
        /// </summary>
        protected virtual string FullPath
        {
            get
            {
                if (Parent == null) return Id;
                return ((HttpNode)Parent).FullPath + "/" + Id;
            }
        }
    }
}
