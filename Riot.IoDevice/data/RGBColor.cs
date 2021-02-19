namespace Riot.IoDevice
{
    /// <summary>
    /// defines a RGB Color
    /// </summary>
    public class RGBColor
    {
        /// <summary>
        /// constructor
        /// </summary>
        public RGBColor(double red, double green, double blue)
        {
            R = red;
            G = green;
            B = blue;
        }

        /// <summary>
        /// Red color value 0 to 1
        /// </summary>
        public double R { get; set; }

        /// <summary>
        /// Green color value 0 to 1
        /// </summary>
        public double G { get; set; }

        /// <summary>
        /// Blue color value 0 to 1
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// convert color value (0 - 1 double) to IO value (0 - 255 int)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int ConvertColorToIOValue(double value)
        {
            return (int)(value * 255);
        }

    }
}
