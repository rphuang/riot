using System.Numerics;

namespace Riot.Phone
{
    /// <summary>
    /// defines a Quaternion data
    /// </summary>
    public class QuaternionData : WireData
    {
        /// <summary>
        /// get data value
        /// </summary>
        public Quaternion Value { get; set; }

        /// <summary>
        /// convert object to string
        /// </summary>
        public override string ToString()
        {
            return $"{FullPath} Reading: X: {Value.X}, Y: {Value.Y}, Z: {Value.Z}, W: {Value.W}";

        }

        /// <summary>
        /// create a new Vector3Data with zero values
        /// </summary>
        public static QuaternionData CreateZeroData(string id)
        {
            return new QuaternionData { Id = id, Value = new Quaternion { W = 0, X = 0, Y = 0, Z = 0 } };
        }
    }
}
