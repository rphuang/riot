using System;
using System.Collections.Generic;
using System.Text;

namespace IotClientLib
{
    /// <summary>
    /// Encapsulate a single IO pin for the GPIO
    /// </summary>
    public interface IotGpioPin : IotNode
    {
        /// <summary>
        /// the pin number for the IO pin
        /// </summary>
        string Pin { get; }

        /// <summary>
        /// the mode for the IO pin
        /// </summary>
        int Mode { get; }

        /// <summary>
        /// the value for the IO pin
        /// </summary>
        int Value { get; }

        /// <summary>
        /// get the pin's data from the server and update the properties
        /// </summary>
        /// <returns>response from server</returns>
        string Get();

        /// <summary>
        /// set mode of the pin on the server
        /// </summary>
        /// <returns>response from server</returns>
        string SetMode(int mode);

        /// <summary>
        /// set value of the pin on the server
        /// </summary>
        /// <returns>response from server</returns>
        string SetValue(int value);
    }
}
