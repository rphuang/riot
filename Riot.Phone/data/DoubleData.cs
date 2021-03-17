using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.Phone
{
    /// <summary>
    /// defines the data for a double value
    /// </summary>
    public class DoubleData : WireData
    {
        /// <summary>
        /// get data value
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// convert object to string
        /// </summary>
        public override string ToString()
        {
            return $"{FullPath} Reading: {Value}";

        }

        /// <summary>
        /// create a new Vector3Data with zero values
        /// </summary>
        public static DoubleData CreateZeroData(string id)
        {
            return new DoubleData { Id = id, Value = 0 };
        }
    }
}
