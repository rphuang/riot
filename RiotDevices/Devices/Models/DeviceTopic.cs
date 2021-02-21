using System.Collections.Generic;

namespace Devices.Models
{
    /// <summary>
    /// encapsulate a topic of device
    /// </summary>
    public class DeviceTopic : Topic
    {
        /// <summary>
        /// the type name for DeviceTopic
        /// </summary>
        public const string DeviceTopicType = "Device";

        public DeviceTopic() : base(DeviceTopicType)
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        public DeviceTopic(IDictionary<string, object> properties)
            : base(properties, DeviceTopicType)
        {
        }

        /// <summary>
        /// create a Clone of this Topic;
        /// </summary>
        public override Topic Clone()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>(this.Properties);
            return new DeviceTopic(properties);
        }
    }
}
