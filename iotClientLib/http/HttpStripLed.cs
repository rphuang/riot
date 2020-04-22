namespace IotClientLib
{
    /// <summary>
    /// Implements IotStripLed with http protocol
    /// </summary>
    public class HttpStripLed : HttpRGBLed, IotStripLed
    {
        /// <summary>
        /// constructor
        /// </summary>
        public HttpStripLed(string id, string name, HttpIotClient client, HttpNode parent)
            : base(id, name, client, parent)
        {
        }

        #region IotStripLed interfaces

        /// <summary>
        /// the name for the strip pattern
        /// </summary>
        public string PatternName { get; set; }

        /// <summary>
        /// set the LED to display the specified pattern
        /// </summary>
        /// <param name="name">the name of the display pattern.</param>
        /// <returns>returns server response</returns>
        public string SetPattern(string name)
        {
            PatternName = name;
            return PostPattern();
        }

        #endregion

        private string PostPattern()
        {
            string json = string.Format("{{\"{0}\": \"{1}\"}}", Id, PatternName);
            return Client.Post(Parent?.Id, json);
        }
    }
}
