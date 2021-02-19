namespace Riot.IoDevice
{
    /// <summary>
    /// defines the data for RGB LED
    /// </summary>
    public class RGBLedData : IotData
    {
        /// <summary>
        /// the color of the RGB LED. 
        /// For some RGB LED, only 0 (off) and none-zero (on) are interpreted for each color.
        /// </summary>
        public RGBColor Color { get; set; }

        /// <summary>
        /// value to turn on RGB LED
        /// </summary>
        public static RGBColor LedON = new RGBColor(1, 1, 1);

        /// <summary>
        /// value to turn off RGB LED
        /// </summary>
        public static RGBColor LedOFF = new RGBColor(0, 0, 0);
    }
}
