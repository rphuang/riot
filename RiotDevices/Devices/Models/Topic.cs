using EntityLib;
using System.Collections.Generic;

namespace Devices.Models
{
    /// <summary>
    /// encapsulate a topic of Pi system for monitoring
    /// </summary>
    public class Topic : Entity
    {
        /// <summary>
        /// the type name for MonitorTopic
        /// </summary>
        public const string MonitorTopicType = "Monitor";

        /// <summary>
        /// constructor
        /// </summary>
        public Topic()
        {
            Type = MonitorTopicType;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public Topic(string topicType)
        {
            Type = topicType;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public Topic(IDictionary<string, object> properties, string topicType = MonitorTopicType)
            : base(properties)
        {
            Type = topicType;
            ParseServer();
            ParseCredential();
        }

        /// <summary>
        /// Get or set the Name of the topic
        /// </summary>
        public string Name
        {
            get { return GetProperty<string>(nameof(Name)); }
            set { SetProperty(nameof(Name), value); }
        }

        /// <summary>
        /// create a Clone of this Topic;
        /// </summary>
        public virtual Topic Clone()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>(this.Properties);
            return new Topic(properties);
        }

        /// <summary>
        /// the server definition include server name (or IP address) and port number in the form of 192.168.0.111:8008
        /// </summary>
        public string Server
        {
            get { return GetProperty<string>(nameof(Server)); }
            set
            {
                SetProperty(nameof(Server), value);
                ParseServer();
            }
        }

        /// <summary>
        /// the account credential include user name and password in the form of user:password
        /// </summary>
        public string Credential
        {
            get { return GetProperty<string>(nameof(Credential)); }
            set
            {
                SetProperty(nameof(Credential), value);
                ParseCredential();
            }
        }


        /// <summary>
        /// The path to the service
        /// </summary>
        public string Path
        {
            get { return GetProperty<string>(nameof(Path)); }
            set { SetProperty(nameof(Path), value); }
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
            string value = GetProperty<string>(nameof(Server));
            if (string.IsNullOrEmpty(value)) return;
            string[] parts = value.Split(ColonDelimiter);
            ServerAddress = parts[0];
            if (parts.Length > 1) ServerPort = int.Parse(parts[1]);
            else ServerPort = 80;
        }

        private void ParseCredential()
        {
            string value = GetProperty<string>(nameof(Credential));
            if (string.IsNullOrEmpty(value)) return;
            string[] parts = value.Split(ColonDelimiter);
            UserName = parts[0];
            if (parts.Length > 1) UserPassword = parts[1];
            else UserPassword = string.Empty;
        }

        private static readonly char[] ColonDelimiter = { ':' };
    }
}
