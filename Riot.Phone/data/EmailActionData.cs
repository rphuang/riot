using System;
using System.Collections.Generic;
using System.Text;

namespace Riot.Phone
{
    /// <summary>
    /// defines the data for email
    /// </summary>
    public class EmailActionData : IotData
    {
        /// <summary>
        /// the email Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// the email body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// format 0 - plaintext, 1 - html
        /// </summary>
        public int BodyFormat { get; set; }

        /// <summary>
        /// the email recipients
        /// </summary>
        public List<string> To { get; set; }
        //
        // Summary:
        //     Gets or sets the email's CC recipients.
        public List<string> Cc { get; set; }
        //
        // Summary:
        //     Gets or sets the email's BCC recipients.
        public List<string> Bcc { get; set; }
    }
}
