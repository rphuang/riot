
namespace IotClientLib
{
    /// <summary>
    /// defines the properties and functions for RGB LED
    /// </summary>
    public interface IotRGBLed : IotNode
    {
        /// <summary>
        /// the color of the RGB LED. Only 0 (off) and 1 (on) are valid values for each color.
        /// </summary>
        IotColor Color { get; }

        /// <summary>
        /// set the LED to the specified color
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns>returns server response</returns>
        string SetColor(IotColor color);
    }
}
