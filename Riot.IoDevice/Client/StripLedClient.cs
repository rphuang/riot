using HttpLib;
using Newtonsoft.Json;

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
            StripLedPatternData = new StripLedPatternData();
        }

        /// <summary>
        /// data for Strip LED Pattern
        /// </summary>
        public StripLedPatternData StripLedPatternData
        {
            get { return Data[nameof(StripLedPatternData)] as StripLedPatternData; }
            internal set
            {
                value.Id = nameof(StripLedPatternData);
                UpsertData(value);
            }
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
            StripLedPatternData = JsonConvert.DeserializeObject<StripLedPatternData>(json);
            return true;
        }

        private string PostPattern()
        {
            string json = string.Format("{{\"{0}\": \"{1}\"}}", Id, StripLedPatternData.PatternName);
            return Client.Post(Parent?.Id, json);
        }
    }
}
