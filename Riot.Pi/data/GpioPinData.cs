using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.Pi
{
    /// <summary>
    /// define a single IO pin for the GPIO
    /// </summary>
    public class GpioPinData : IotData
    {
        /// <summary>
        /// the pin number for the IO pin
        /// </summary>
        public int Pin { get; set; }

        /// <summary>
        /// the mode for the IO pin
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// the value for the IO pin
        /// </summary>
        public int Value { get; set; }
    }
}
