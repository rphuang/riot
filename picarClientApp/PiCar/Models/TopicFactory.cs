using ItemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace PiCar.Models
{
    /// <summary>
    /// Factory for Category. Used for the XsvParser to create strongly typed Category Item.
    /// </summary>
    class TopicFactory : IItemFactory
    {
        /// <summary>
        /// create a new empty Category Item
        /// </summary>
        public Item CreateItem(IDictionary<string, Property> properties)
        {
            string type = null;
            Property property;
            if (properties.TryGetValue(nameof(Topic.TopicType), out property))
            {
                type = property.Value as string;

            }

            if (string.Equals(MonitorTopic.MonitorTopicType, type, System.StringComparison.OrdinalIgnoreCase))
            {
                return new MonitorTopic(properties);
            }
            else if (string.Equals(ControlTopic.ControlTopicType, type, System.StringComparison.OrdinalIgnoreCase))
            {
                return new ControlTopic(properties);
            }
            return null;
        }

        /// <summary>
        /// static CategoryFactory instance
        /// </summary>
        public static TopicFactory Instance = new TopicFactory();
    }
}
