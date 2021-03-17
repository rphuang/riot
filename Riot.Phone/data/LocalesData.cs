using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Riot.Phone
{
    /// <summary>
    /// data for locales
    /// </summary>
    public class LocalesData : IotData
    {
        /// <summary>
        /// list of locles
        /// </summary>
        public IEnumerable<Locale> Value { get; set; }
    }
}
