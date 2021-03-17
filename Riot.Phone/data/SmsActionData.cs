using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.Phone
{
    /// <summary>
    /// defines the data for sms message
    /// </summary>
    public class SmsActionData : IotData
    {
        /// <summary>
        /// the text message for SMS
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// the recipients to receive message
        /// </summary>
        public string[] Recipients { get; set; }
    }
}
