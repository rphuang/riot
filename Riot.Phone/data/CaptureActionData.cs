using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.Phone
{
    /// <summary>
    /// defines the data for sms message
    /// </summary>
    public class CaptureActionData : IotData
    {
        /// <summary>
        /// capture form - photo, video
        /// </summary>
        public string Form { get; set; }

        /// <summary>
        /// the text message for SMS
        /// </summary>
        public string FileName { get; set; }

    }
}
