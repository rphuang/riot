using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.Pi
{
    /// <summary>
    /// define GPIO data for Raspberry Pi System with http protocol
    /// </summary>
    public class GpioData : IotData
    {
        /// <summary>
        /// all gpio pins' data
        /// </summary>
        public IList<GpioPinData> Pins { get; internal set; } = new List<GpioPinData>();
    }
}
