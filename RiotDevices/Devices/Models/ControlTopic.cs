using System.Collections.Generic;

namespace Devices.Models
{
    /// <summary>
    /// encapsulate a topic of system for controlling
    /// </summary>
    public class ControlTopic : Topic
    {
        /// <summary>
        /// the type name for ControlTopic
        /// </summary>
        public const string ControlTopicType = "Control";

        public ControlTopic() : base(ControlTopicType)
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        public ControlTopic(IDictionary<string, object> properties)
            : base(properties, ControlTopicType)
        {
        }

        /// <summary>
        /// create a Clone of this Topic;
        /// </summary>
        public override Topic Clone()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>(this.Properties);
            return new ControlTopic(properties);
        }

        /// <summary>
        /// The server video port
        /// </summary>
        public int VideoPort
        {
            get { return GetProperty<int>(nameof(VideoPort)); }
            set { SetProperty(nameof(VideoPort), value); }
        }
    }
}
