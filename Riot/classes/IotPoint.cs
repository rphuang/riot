using Newtonsoft.Json;

namespace Riot
{
    /// <summary>
    /// IotPoint is the base class for all points in RIOT
    /// </summary>
    public abstract class IotPoint : IIotPoint
    {
        /// <summary>
        /// the ID for the node
        /// </summary>
        public string Id { get; set; }

        ///// <summary>
        ///// the ID for the parent node
        ///// </summary>
        //public string ParentId { get; internal set; }

        /// <summary>
        /// The parent of this node
        /// </summary>
        [JsonIgnore]
        public IIotPoint Parent { get; set; }

        /// <summary>
        /// get full path of the IotPoint
        /// For http requests, the FullPath is the request url endpoint path.
        /// </summary>
        public virtual string FullPath
        {
            get
            {
                IotPoint parent = Parent as IotPoint;
                if (parent == null || string.IsNullOrEmpty(parent.FullPath)) return Id;
                return parent.FullPath + "/" + Id;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        protected IotPoint()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        protected IotPoint(string id, IIotPoint parent)
        {
            Id = id;
            Parent = parent;
        }
    }
}
