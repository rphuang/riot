﻿using Newtonsoft.Json;

namespace Riot.IoDevice.Client
{
    /// <summary>
    /// implements RIOT client node for strip LED pattern
    /// </summary>
    public class StripLedClient : RGBLedClient
    {
        /// <summary>
        /// constructor
        /// </summary>
        public StripLedClient(string id, IotHttpClient client, IotNode parent)
            : base(id, client, parent)
        {
        }

        /// <summary>
        /// data for Strip LED Pattern
        /// </summary>
        public StripLedPatternData StripLedPatternData { get; private set; } = new StripLedPatternData();

        /// <summary>
        /// replace the current Data list with new list
        /// </summary>
        public override void ReplaceData(IotData data)
        {
            StripLedPatternData = data as StripLedPatternData;
            base.ReplaceData(StripLedPatternData);
        }

        /// <summary>
        /// set the LED to display the specified pattern
        /// </summary>
        /// <param name="name">the name of the display pattern.</param>
        /// <returns>returns server response</returns>
        public string SetPattern(string name)
        {
            StripLedPatternData.PatternName = name;
            return PostPattern();
        }

        /// <summary>
        /// process the response from server and update the properties
        /// </summary>
        protected override bool ProcessResponse(HttpResponse response)
        {
            string json = response.Result;
            // deserialize
            ReplaceData(JsonConvert.DeserializeObject<StripLedPatternData>(json));
            return true;
        }

        private string PostPattern()
        {
            string json = string.Format("{{\"{0}\": \"{1}\"}}", Id, StripLedPatternData.PatternName);
            return Client.Post(Parent?.Id, json);
        }
    }
}