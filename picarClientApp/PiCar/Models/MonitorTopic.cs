using ItemLib;
using System.Collections.Generic;

namespace PiCar.Models
{
    /// <summary>
    /// encapsulate a topic of Pi system for monitoring
    /// </summary>
    public class MonitorTopic : Topic
    {
        /// <summary>
        /// the type name for MonitorTopic
        /// </summary>
        public const string MonitorTopicType = "Monitor";

        /// <summary>
        /// constructor
        /// </summary>
        public MonitorTopic() : base(MonitorTopicType)
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public MonitorTopic(string topicType) : base(topicType)
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public MonitorTopic(IDictionary<string, Property> properties, string topicType = MonitorTopicType)
            : base(properties, topicType)
        {
            ParseServer();
            ParseCredential();
        }

        /// <summary>
        /// create a Clone of this Topic;
        /// </summary>
        public override Topic Clone()
        {
            Dictionary<string, Property> properties = new Dictionary<string, Property>(this.Properties);
            return new MonitorTopic(properties);
        }

        /// <summary>
        /// the server definition include server name (or IP address) and port number in the form of 192.168.0.111:8008
        /// </summary>
        public string Server
        {
            get { return GetPropertyValue<string>(nameof(Server)); }
            set
            {
                SetPropertyValue(nameof(Server), value);
                ParseServer();
            }
        }

        /// <summary>
        /// the account credential include user name and password in the form of user:password
        /// </summary>
        public string Credential
        {
            get { return GetPropertyValue<string>(nameof(Credential)); }
            set
            {
                SetPropertyValue(nameof(Credential), value);
                ParseCredential();
            }
        }

        /// <summary>
        /// the server address
        /// </summary>
        public string ServerAddress { get; private set; }

        /// <summary>
        /// the server address
        /// </summary>
        public int ServerPort { get; private set; }

        /// <summary>
        /// the user name
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// the UserPassword
        /// </summary>
        public string UserPassword { get; private set; }

        private void ParseServer()
        {
            string value = GetPropertyValue<string>(nameof(Server));
            if (string.IsNullOrEmpty(value)) return;
            string[] parts = value.Split(ColonDelimiter);
            ServerAddress = parts[0];
            if (parts.Length > 1) ServerPort = int.Parse(parts[1]);
            else ServerPort = 80;
        }

        private void ParseCredential()
        {
            string value = GetPropertyValue<string>(nameof(Credential));
            if (string.IsNullOrEmpty(value)) return;
            string[] parts = value.Split(ColonDelimiter);
            UserName = parts[0];
            if (parts.Length > 1) UserPassword = parts[1];
            else UserPassword = string.Empty;
        }

        private static readonly char[] ColonDelimiter = { ':' };
    }
}
