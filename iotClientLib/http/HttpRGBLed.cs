namespace IotClientLib
{
    /// <summary>
    /// Implements IotRGBLed with http protocol
    /// </summary>
    public class HttpRGBLed : HttpNode, IotRGBLed
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HttpRGBLed(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        #region IotRGBLed interfaces

        /// <summary>
        /// the color of the RGB LED. 
        /// For some RGB LED, only 0 (off) and none-zero (on) are interpreted for each color.
        /// </summary>
        public IotColor Color { get; internal set; }

        /// <summary>
        /// set the LED to the specified color
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns>returns server response</returns>
        public string SetColor(IotColor color)
        {
            Color = color;
            return Post();
        }

        #endregion

        private string Post()
        {
            const string JsonFormat = "{{\"{0}\": \"{1:000}, {2:000}, {3:000}\"}}";
            string json = string.Format(JsonFormat, Id, ConvertColorValue(Color.R), ConvertColorValue(Color.G), ConvertColorValue(Color.B));
            return Client.Post(Parent?.Id, json);
        }
    }
}
