using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Riot.Phone
{
    /// <summary>
    /// defines the data for text to speech
    /// </summary>
    public class SpeechActionData : IotData
    {
        /// <summary>
        /// the text for speaking
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// the locale for speaking
        /// </summary>
        public Locale Locale { get; set; }

        /// <summary>
        /// the pitch used for speaking
        /// </summary>
        public float? Pitch { get; set; }

        /// <summary>
        /// the volume used for speaking
        /// </summary>
        public float? Volume { get; set; }
    }
}
