using System;
using System.Collections.Generic;

namespace EntityLib
{
    /// <summary>
    /// PropertyBag defines a base class with properties stored in Dictionary
    /// </summary>
    public class PropertyBag
    {
        /// <summary>
        /// Constructs a new case insensitive PropertyBag
        /// </summary>
        public PropertyBag()
        {
            Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Constructs a new PropertyBag with dictionary
        /// </summary>
        public PropertyBag(IDictionary<string, object> properties)
        {
            Properties = properties;
        }

        /// <summary>
        /// Whether the Entity is dirty or not. This property is not persisted in the DB.
        /// </summary>
        public bool IsDirty { get; protected set; }

        /// <summary>
        /// Get or set the property value by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get { return this.Properties[name]; }
            set { SetProperty(name, value); }
        }

        /// <summary>
        /// Get property value by name
        /// </summary>
        public object GetProperty(string name)
        {
            return this.Properties[name];
        }

        /// <summary>
        /// Get property value by name
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The property value.</returns>
        public virtual T GetProperty<T>(string propertyName)
        {
            object value;
            if (this.Properties.TryGetValue(propertyName, out value))
            {
                if (value != null)
                {
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch (InvalidCastException ex)
                    {
                        string msg = string.Format("Unable to Convert '{0}' to type '{1}'.", value, typeof(T));
                        throw new Exception(msg, ex);
                    }
                    catch (FormatException ex)
                    {
                        string msg = string.Format("Unable to Convert '{0}' to type '{1}'.", value, typeof(T));
                        throw new Exception(msg, ex);
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// Set property value by name
        /// </summary>
        public void SetProperty(string name, object value)
        {
            this.Properties[name] = value;
            IsDirty = true;
        }

        /// <summary>
        /// Whether the entity has the specified property
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns></returns>
        public bool HasProperty(string propertyName)
        {
            return Properties.ContainsKey(propertyName);
        }

        /// <summary>
        /// Get all the properties for the entity
        /// </summary>
        public IDictionary<string, object> Properties { get; protected set; }
    }
}
