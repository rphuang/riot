using Newtonsoft.Json;
using System;

namespace Riot
{
    /// <summary>
    /// IotData defines the base class for data point in RIOT
    /// </summary>
    public abstract class IotData : IotPoint
    {
        /// <summary>
        /// the time stamp for the data
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// serialize the data with Newtonsoft Json serializer
        /// derived class can implement special serialization 
        /// </summary>
        public virtual string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// constructor
        /// </summary>
        protected IotData()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        protected IotData(string id, IIotPoint parent) : base(id, parent)
        {
        }
    }
}
