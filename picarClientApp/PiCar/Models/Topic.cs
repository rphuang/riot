using ItemLib;
using PlatformLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PiCar.Models
{
    /// <summary>
    /// Defines a base topic
    /// </summary>
    public class Topic : Item
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public Topic(string topicType)
        {
            TopicType = topicType;
        }

        /// <summary>
        /// construct with properties
        /// </summary>
        public Topic(IDictionary<string, Property> properties, string topicType)
            : base(properties)
        {
            TopicType = topicType;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public Topic(Topic topic)
            : base(topic)
        {
            TopicType = topic.TopicType;
        }

        /// <summary>
        /// create a Clone of this Topic;
        /// note that this method hides (not overrides) the Item.Clone() so it returns strongly typed Topic
        /// </summary>
        public virtual Topic Clone()
        {
            return new Topic(this);
        }

        /// <summary>
        /// the title of the topic
        /// </summary>
        public string Title
        {
            get { return GetPropertyValue<string>(nameof(Title)); }
            set { SetPropertyValue(nameof(Title), value); }
        }

        /// <summary>
        /// the type of the topic
        /// </summary>
        public string TopicType
        {
            get { return GetPropertyValue<string>(nameof(TopicType)); }
            set { SetPropertyValue(nameof(TopicType), value); }
        }

        /// <summary>
        /// get Topic by index
        /// </summary>
        public static Topic GetDefaultTopic()
        {
            return Topics.Values.FirstOrDefault();
        }

        /// <summary>
        /// get Topic by Topic ID (Name)
        /// </summary>
        public static Topic GetTopic(string categoryId)
        {
            return Topics[categoryId];
        }

        /// <summary>
        /// Add a new topic
        /// </summary>
        public static void AddOrSaveTopic(Topic topic)
        {
            if (topic == null) return;
            // make a copy of the topic to isolate what would be added to Topics
            Topic clone = topic.Clone();
            string key = clone.Id;
            if (string.IsNullOrEmpty(key)) key = clone.Name;
            if (Topics.ContainsKey(key)) Topics[key] = clone;
            else Topics.Add(key, clone);
            UpdateMenuItemsAndSaveTopics();
        }

        /// <summary>
        /// Replace a topic
        /// </summary>
        public static void ReplaceTopic(string topicId, Topic topic)
        {
            if (topic == null) return;

            if (!string.IsNullOrEmpty(topicId)) Topics.Remove(topicId);
            AddOrSaveTopic(topic);
        }

        /// <summary>
        /// Delete a topic
        /// </summary>
        public static void DeleteTopic(Topic topic)
        {
            if (topic == null) return;

            Topics.Remove(topic.Id);
            UpdateMenuItemsAndSaveTopics();
        }

        #region static members for initializing topics

        public static Dictionary<string, Topic> Topics { get; } = InitializeTopics();

        private static Dictionary<string, Topic> InitializeTopics()
        {
            Dictionary<string, Topic> topics = InitializeTopicsFromLocalStorage();
            if (topics == null || topics.Count == 0) topics = InitializeTopicsFromDefault();
            return topics;
        }

        private static Dictionary<string, Topic> InitializeTopicsFromLocalStorage()
        {
            string filePath = GetTopicsFilePath(false);
            if (File.Exists(filePath))
            {
                XsvFileParser parser = new XsvFileParser { FirstRowHeader = true, UseDelimiterFromExtension = true };
                parser.XsvParser.ItemFactory = TopicFactory.Instance;
                ItemGroup itemGroup = parser.ParseXsvFile(filePath);
                Dictionary<string, Topic> topics = new Dictionary<string, Topic>();
                foreach (Topic item in itemGroup.Items)
                {
                    topics.Add(item.Id, item);
                }
                return topics;
            }

            return null;
        }

        private static Dictionary<string, Topic> InitializeTopicsFromDefault()
        {
            Dictionary<string, Topic> topics = new Dictionary<string, Topic>();
            AddTopicToDictionary(topics, new MonitorTopic { Id = "PiStats - Pi Car", Name = "Pi Car", Server = "192.168.1.111.8001", Credential = "" });
            AddTopicToDictionary(topics, new ControlTopic { Id = "PiControl - Pi Car", Name = "Pi Car", Server = "192.168.1.111.8001", Credential = "" });
            return topics;
        }

        private static void AddTopicToDictionary(Dictionary<string, Topic> topics, Topic item)
        {
            topics.Add(item.Id, item);
        }

        #endregion

        #region static members for initializing topics

        public static ObservableCollection<HomeMenuItem> MenuItems { get; } = InitializeMenuItems();

        private static ObservableCollection<HomeMenuItem> InitializeMenuItems()
        {
            ObservableCollection<HomeMenuItem> menuItems = new ObservableCollection<HomeMenuItem>();
            PopulateMenuItemsFromTopics(menuItems);
            return menuItems;
        }

        private static void PopulateMenuItemsFromTopics(ObservableCollection<HomeMenuItem> menuItems)
        {
            foreach (var item in Topic.Topics)
            {
                menuItems.Add(new HomeMenuItem { MenuType = MenuItemType.PiStats, Title = item.Key, Topic = item.Value });
            }
            menuItems.Add(new HomeMenuItem { MenuType = MenuItemType.Settings, Title = "Settings" });
            menuItems.Add(new HomeMenuItem { MenuType = MenuItemType.About, Title = "About" });
        }

        #endregion

        #region save topics

        private static string GetTopicsFilePath(bool createFolder)
        {
            string folderPath = FolderUtil.GetDocsFolderPath(createFolder);
            if (string.IsNullOrEmpty(folderPath)) return null;

            string filePath = Path.Combine(folderPath, Settings.Instance.TopicsFile);
            return filePath;
        }

        private static void UpdateMenuItemsAndSaveTopics()
        {
            SaveTopics();
            MenuItems.Clear();
            PopulateMenuItemsFromTopics(MenuItems);
        }

        private static void SaveTopics()
        {
            try
            {
                string filePath = GetTopicsFilePath(true);
                if (string.IsNullOrEmpty(filePath)) return;

                using (TextWriter writer = File.CreateText(filePath))
                {
                    // write header
                    writer.WriteLine("Id,Name,TopicType,FileName,RootFolder,Folder,ShowImageOnItemsPage,Server,Credential");
                    foreach (Topic topic in Topics.Values)
                    {
                        MonitorTopic monitorTopic = topic as MonitorTopic;
                        writer.WriteLine("{0},{1},{2},,,,,{3},{4}", topic.Id, topic.Name, topic.TopicType, monitorTopic.Server, monitorTopic.Credential);
                    }
                    writer.Close();
                }
            }
            catch (Exception err)
            {
                // todo
            }
        }

        #endregion
    }
}
