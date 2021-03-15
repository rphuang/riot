using System;
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
        public IDictionary<string, IotData> Data { get; private set; } = new Dictionary<string, IotData>(StringComparer.OrdinalIgnoreCase);

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
        /// get the data by ID
        /// </summary>
        public T GetData<T>(string id) where T : IotData
        {
            IotData data;
            if (Data.TryGetValue(id, out data))
            {
                return data as T;
            }
            return null;
        }

        /// <summary>
        /// update or insert the data in Data dictionary
        /// </summary>
        public virtual void UpsertData(IotData data)
        {
            if (data == null) return;
            data.Parent = this;
            if (Data.ContainsKey(data.Id)) Data[data.Id] = data;
            else Data.Add(data.Id, data);
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
