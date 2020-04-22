namespace IotClientLib
{
    /// <summary>
    /// defines the properties and functions for strip LED
    /// </summary>
    public interface IotStripLed : IotRGBLed
    {
        /// <summary>
        /// the name for the strip pattern
        /// </summary>
        string PatternName { get; set; }

        /// <summary>
        /// set the LED to display the specified pattern
        /// </summary>
        /// <param name="name">the name of the display pattern.</param>
        /// <returns>returns server response</returns>
        string SetPattern(string name);
    }
}
