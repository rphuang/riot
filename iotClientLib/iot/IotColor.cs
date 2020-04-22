namespace IotClientLib
{
    /// <summary>
    /// defines a RGB Color
    /// </summary>
    public class IotColor
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IotColor(double red, double green, double blue)
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
    }
}
