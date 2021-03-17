using Xamarin.Essentials;

namespace Riot.Phone
{
    /// <summary>
    /// defines location data
    /// </summary>
    public class LocationData : WireData
    {
        /// <summary>
        /// Location value
        /// </summary>
        public Location Value { get; set; }

        /// <summary>
        /// convert object to string
        /// </summary>
        public override string ToString()
        {
            return $"{FullPath} Reading: Latitude: {Value.Latitude}, Longitude: {Value.Longitude}, Altitude: {Value.Altitude}";

        }
    }
}
