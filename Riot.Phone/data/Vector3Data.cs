using System.Numerics;

namespace Riot.Phone
{
    /// <summary>
    /// defines a Vector3 data
    /// </summary>
    public class Vector3Data : WireData
    {
        /// <summary>
        /// get data value
        /// </summary>
        public Vector3 Value { get; set; }

        /// <summary>
        /// convert object to string
        /// </summary>
        public override string ToString()
        {
            return $"{FullPath} Reading: X: {Value.X}, Y: {Value.Y}, Z: {Value.Z}";

        }

        /// <summary>
        /// create a new Vector3Data with zero values
        /// </summary>
        public static Vector3Data CreateZeroData(string id)
        {
            return new Vector3Data { Id = id, Value = new Vector3 { X = 0, Y = 0, Z = 0 } };
        }
    }
}
