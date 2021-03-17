namespace Riot.Phone
{
    /// <summary>
    /// defines the data for a bool value
    /// </summary>
    public class BoolData : WireData
    {
        /// <summary>
        /// get data value
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// convert object to string
        /// </summary>
        public override string ToString()
        {
            return $"{FullPath} Reading: {Value}";

        }
    }
}
