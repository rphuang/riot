using System;
using System.Collections.Generic;
using System.Text;

namespace Riot
{
    /// <summary>
    /// IIotData defines the base interface for data point in RIOT
    /// </summary>
    public interface IIotData : IIotPoint
    {
        /// <summary>
        /// the time stamp for the data
        /// </summary>
        DateTime TimeStamp { get; set; }

        /// <summary>
        /// serialize the data
        /// </summary>
        string Serialize();
    }
}
