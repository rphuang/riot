using System;
using System.Collections.Generic;
using System.Text;

namespace IotClientLib
{
    /// <summary>
    /// Encapsulate the GPIO device for accessing the IO pins
    /// </summary>
    public interface IotGpio : IotNode
    {
        /// <summary>
        /// 
        /// </summary>
        IList<IotGpioPin> Pins { get; }

        /// <summary>
        /// get all pins data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        string Get();
    }
}
