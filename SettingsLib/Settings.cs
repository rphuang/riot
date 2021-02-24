using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SettingsLib
{
    /// <summary>
    /// Base class for settings that load/save to text file
    /// The setting values will be saved as key=value pairs in text file with "=" as delimiter one line per setting
    /// Group setting starts with Group=groupSettingId and ends with EndGroup or another group
    /// Example of setting for derived class:
    /// public string Sample
    /// {
    ///     get { return GetOrAddSetting(nameof(Sample), "newstopics.ssv"); }
    ///     set { SetSetting(nameof(Sample), value); }
    /// }
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// constructor
        /// </summary>
        public Settings(string fileName)
        {
            SettingsFile = fileName;
        }

        /// <summary>
        /// automatically save settings whenever SetSetting
        /// </summary>
        public bool AutoSave { get; set; }

        /// <summary>
        /// get or add a setting
        /// </summary>
        /// <typeparam name="T">the data type</typeparam>
        /// <param name="name">the name of the setting</param>
        /// <param name="defaultValue">the default value when adding new setting</param>
        /// <returns>the setting value</returns>
        public T GetOrAddSetting<T>(string name, T defaultValue)
        {
            string value;
            if (SettingsDictionary.TryGetValue(name, out value))
            {
                return (T)System.Convert.ChangeType(value, typeof(T));
            }
            SetSetting(name, defaultValue.ToString());
            return defaultValue;
        }

        /// <summary>
        /// try get a setting group by ID
        /// </summary>
        /// <param name="id">the ID of the setting group</param>
        /// <param name="groupSetting">the out setting group</param>
        /// <returns>returns true if exists</returns>
        public bool TryGetSettingGroup(string id, out SettingGroup settingGroup)
        {
            return GroupsDictionary.TryGetValue(id, out settingGroup);
        }

        /// <summary>
        /// get all setting groups
        /// </summary>
        /// <returns>a list of setting groups</returns>
        public IEnumerable<SettingGroup> GetSettingGroups()
        {
            return GroupsDictionary.Values;
        }

        /// <summary>
        /// get all setting groups with propertyName = value
        /// </summary>
        /// <param name="propertyName">the setting name within the group</param>
        /// <param name="value">the value of the setting</param>
        /// <returns>a list of setting groups that match the property value</returns>
        public IEnumerable<SettingGroup> GetSettingGroups(string propertyName, string value)
        {
            return GroupsDictionary.Values.Where((SettingGroup item) =>
            {
                string propertyValue;
                if (item.Settings.TryGetValue(propertyName, out propertyValue))
                {
                    if (string.Equals(value, propertyValue, StringComparison.OrdinalIgnoreCase)) return true;
                }
                return false;
            });
        }

        /// <summary>
        /// set a setting
        /// </summary>
        public void SetSetting(string name, object value)
        {
            string strValue = value?.ToString();
            if (SettingsDictionary.ContainsKey(name)) SettingsDictionary[name] = strValue;
            else SettingsDictionary.Add(name, strValue);
            if (AutoSave) SaveSettings();
        }

        /// <summary>
        /// set a group setting
        /// </summary>
        public void SetSettingGroup(SettingGroup group)
        {
            string id = group.Id;
            if (GroupsDictionary.ContainsKey(id)) GroupsDictionary[id] = group;
            else GroupsDictionary.Add(id, group);
            bool oldAutoSave = AutoSave;
            AutoSave = false;
            foreach (var item in group.Settings)
            {
                SetSetting(id + "." + item.Key, item.Value);
            }
            AutoSave = oldAutoSave;
            if (AutoSave) SaveSettings();
        }

        /// <summary>
        /// Save the current settings to local file
        /// </summary>
        public bool SaveSettings()
        {
            try
            {
                if (string.IsNullOrEmpty(SettingsFile)) return false;

                using (TextWriter writer = File.CreateText(SettingsFile))
                {
                    foreach (SettingGroup group in GroupsDictionary.Values)
                    {
                        writer.WriteLine("Group={0}", group.Id);
                        foreach (var item in group.Settings)
                        {
                            writer.WriteLine("    {0}={1}", item.Key, item.Value);
                        }
                        writer.WriteLine("EndGroup={0}", group.Id);
                    }
                    foreach (var item in SettingsDictionary)
                    {
                        // skip those "group.key=value" for setting group 
                        string key = item.Key;
                        string[] parts = key.Split(GroupNameDelimiter);
                        if (parts.Length == 1 || !GroupsDictionary.ContainsKey(parts[0]))
                        {
                            writer.WriteLine("{0}={1}", key, item.Value);
                        }
                    }
                    writer.Close();
                }
                return true;
            }
            catch (Exception err)
            {
                // todo
            }
            return false;
        }

        /// <summary>
        /// Load the settings from local file
        /// </summary>
        public bool LoadSettings()
        {
            bool oldAutoSave = AutoSave;
            AutoSave = false;
            try
            {
                if (string.IsNullOrEmpty(SettingsFile)) return false;

                using (TextReader reader = File.OpenText(SettingsFile))
                {
                    string groupName = string.Empty;
                    SettingGroup group = null;
                    string buffer;
                    while ((buffer = reader.ReadLine()) != null)
                    {
                        buffer = buffer.Trim();
                        if (string.IsNullOrEmpty(buffer) || buffer.StartsWith("#")) continue;

                        string[] parts = buffer.Split(SettingValueDelimiter);
                        if (parts.Length > 1)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            if (string.Equals("group", key, StringComparison.OrdinalIgnoreCase))
                            {
                                groupName = value;
                                group = new SettingGroup() { Id = value };
                                GroupsDictionary.Add(value, group);
                                // adding ID to both dictionaries
                                group.SetSetting("Id", value);
                                SetSetting(groupName + ".Id", value);
                            }
                            else if (string.Equals("endgroup", key, StringComparison.OrdinalIgnoreCase))
                            {
                                groupName = string.Empty;
                                group = null;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(groupName)) SetSetting(key, value);
                                else
                                {
                                    string fullName = groupName + "." + key;
                                    SetSetting(fullName, value);
                                    group.SetSetting(key, value);
                                }
                            }
                        }
                    }
                    reader.Close();
                }
                return true;
            }
            catch (Exception err)
            {
                // todo
            }
            AutoSave = oldAutoSave;
            return false;
        }

        protected string SettingsFile { get; set; }
        protected Dictionary<string, string> SettingsDictionary { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        protected Dictionary<string, SettingGroup> GroupsDictionary { get; } = new Dictionary<string, SettingGroup>(StringComparer.OrdinalIgnoreCase);

        protected static char[] SettingValueDelimiter = { '=' };
        protected static char[] GroupNameDelimiter = { '.' };
    }
}
