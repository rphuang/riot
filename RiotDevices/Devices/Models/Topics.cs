using SettingsLib;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Models
{
    /// <summary>
    /// Defines a base topic
    /// </summary>
    public class Topics
    {
        /// <summary>
        /// loaded topic items
        /// </summary>
        public static Dictionary<string, Topic> TopicItems { get; private set; }

        /// <summary>
        /// initialize topics and menuitem
        /// </summary>
        public static void InitializeTopics()
        {
            InitializeTopicsFromSettingsFile();
            if (TopicItems == null || TopicItems.Count == 0)
            {
                InitializeTopicsFromDefault();
            }
        }

        /// <summary>
        /// get Topic by Topic ID (Name)
        /// </summary>
        public static bool TryGetTopic(string topicId, out Topic topic)
        {
            if (TopicItems.TryGetValue(topicId, out topic)) return true;
            return false;
        }

        /// <summary>
        /// Add a new topic
        /// </summary>
        public static void AddOrSaveTopic(Topic topic)
        {
            if (topic == null) return;
            // make a copy of the topic to isolate what would be added to Topics
            Topic clone = topic.Clone() as Topic;
            string key = clone.Id;
            if (string.IsNullOrEmpty(key)) key = clone.Name;
            if (TopicItems.ContainsKey(key)) TopicItems[key] = clone;
            else TopicItems.Add(key, clone);
            SaveTopics();
        }

        /// <summary>
        /// Replace a topic
        /// </summary>
        public static void ReplaceTopic(string topicId, Topic topic)
        {
            if (topic == null) return;

            if (!string.IsNullOrEmpty(topicId)) TopicItems.Remove(topicId);
            AddOrSaveTopic(topic);
        }

        /// <summary>
        /// Delete a topic
        /// </summary>
        public static void DeleteTopic(Topic topic)
        {
            if (topic == null) return;

            TopicItems.Remove(topic.Id);
            SaveTopics();
        }

        #region static members for initializing topics

        private static void InitializeTopicsFromSettingsFile()
        {
            TopicItems = new Dictionary<string, Topic>();
            DeviceSettings settings = DeviceSettings.Instance;
            if (settings.IsValid)
            {
                IEnumerable<SettingGroup> groups = settings.GetSettingGroups();
                foreach (SettingGroup group in groups)
                {
                    string type;
                    if (group.Settings.TryGetValue("Type", out type))
                    {
                        IDictionary<string, string> dict = group.Settings;
                        IDictionary<string, object> dictobj = dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value);
                        if (string.Equals("PiSystem", type, System.StringComparison.OrdinalIgnoreCase))
                        {
                            AddTopicToDictionary(TopicItems, new Topic(dictobj));
                        }
                        else if (string.Equals("KasaHS1xx", type, System.StringComparison.OrdinalIgnoreCase))
                        {
                            AddTopicToDictionary(TopicItems, new DeviceTopic(dictobj));
                        }
                        else if (string.Equals("PiCar", type, System.StringComparison.OrdinalIgnoreCase))
                        {
                            AddTopicToDictionary(TopicItems, new ControlTopic(dictobj));
                        }
                    }
                }
            }
        }

        private static void InitializeTopicsFromDefault()
        {
            TopicItems = new Dictionary<string, Topic>();
            AddTopicToDictionary(TopicItems, new Topic { Id = "Monitor - Pi Server", Name = "Pi Server", Server = "192.168.0.123:1234", Credential = "user:password" });
            AddTopicToDictionary(TopicItems, new DeviceTopic { Id = "Smart Plug 1", Name = "Smart Plug 1", Server = "192.168.0.123:1234", Credential = "user:password", Path = "plug01" });
            AddTopicToDictionary(TopicItems, new ControlTopic { Id = "Control - Pi Car", Name = "Pi Car", Server = "192.168.0.123:1234", Credential = "user:password", VideoPort = 5678 });
        }

        private static void AddTopicToDictionary(Dictionary<string, Topic> topics, Topic item)
        {
            topics.Add(item.Id, item);
        }

        #endregion

        private static void SaveTopics()
        {
            if (DeviceSettings.Instance.IsValid) DeviceSettings.Instance.SaveSettings();
        }
    }
}
