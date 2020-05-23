using ItemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace PiCar.Models
{
    /// <summary>
    /// encapsulate a topic of Pi system for monitoring
    /// </summary>
    public class ControlTopic : MonitorTopic
    {
        /// <summary>
        /// the type name for MonitorTopic
        /// </summary>
        public const string ControlTopicType = "Control";

        public ControlTopic() : base(ControlTopicType)
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        public ControlTopic(IDictionary<string, Property> properties)
            : base(properties, ControlTopicType)
        {
        }

        /// <summary>
        /// create a Clone of this Topic;
        /// </summary>
        public override Topic Clone()
        {
            Dictionary<string, Property> properties = new Dictionary<string, Property>(this.Properties);
            return new ControlTopic(properties);
        }

        /// <summary>
        /// The server video port
        /// </summary>
        public int VideoPort
        {
            get { return GetPropertyValue<int>(nameof(VideoPort)); }
            set { SetPropertyValue(nameof(VideoPort), value); }
        }
    }
}
