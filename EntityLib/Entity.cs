using System;
using System.Collections.Generic;
using System.Text;

namespace EntityLib
{
    /// <summary>
    /// Entity represents any object with ID and Type.
    /// </summary>
    public class Entity : PropertyBag
    {
        /// <summary>
        /// Default constructor that generates an ID using new GUID
        /// </summary>
        public Entity()
            : this((string)null)
        {
        }

        /// <summary>
        /// Constructor that generates an ID using new GUID
        /// </summary>
        public Entity(string type)
            : this(type, Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Constructor with given type & id
        /// </summary>
        public Entity(string type, string id)
        {
            Id = id;
            if (type != null)
            {
                Type = type;
            }
        }

        /// <summary>
        /// Constructs a new Entity with properties
        /// </summary>
        public Entity(IDictionary<string, object> properties) : base(properties)
        {
            object id;
            if (Properties.TryGetValue(nameof(Id), out id))
            {
                if (id == null || string.IsNullOrEmpty(id.ToString()))
                {
                    Id = Guid.NewGuid().ToString();
                }
            }
            else
            {
                Id = Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// Get or set the Id of the entity
        /// </summary>
        public virtual string Id
        {
            get { return GetProperty<string>(nameof(Id)); }
            set { SetProperty(nameof(Id), value); }
        }

        /// <summary>
        /// Get or set the EntityType of the entity
        /// </summary>
        public virtual string Type
        {
            get { return GetProperty<string>(nameof(Type)); }
            set { SetProperty(nameof(Type), value); }
        }

        /// <summary>
        /// Whether the Entity has error or not. This property is not persisted in the DB.
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// Useing the property bag to get all property names
        /// </summary>
        /// <returns></returns>
        public string GetAllPropertyNames()
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, object> item in Properties)
            {
                builder.AppendFormat("{0},", item.Key);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Useing the property bag to get all property values
        /// </summary>
        /// <returns></returns>
        public string GetAllPropertyValues(bool csvFormat = false)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, object> item in Properties)
            {
                if (csvFormat) builder.AppendFormat("{0},", item.Value);
                else builder.AppendFormat("{0}:{1} ", item.Key, item.Value);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Enforce a property's value
        /// </summary>
        protected void EnforceProperty(string name, object value)
        {
            if (Properties.ContainsKey(name))
            {
                Properties[name] = value;
            }
            else
            {
                Properties.Add(name, value);
            }
        }
    }
}
