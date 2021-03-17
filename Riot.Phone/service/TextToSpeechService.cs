using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// performs Text To Speech on the phone device
    /// </summary>
    public class TextToSpeechService : BaseActionService
    {
        /// <summary>
        /// constructor
        /// </summary>
        public TextToSpeechService(IotNode parent) : base("TextToSpeech", parent)
        {
            GetLocalesAsync();
        }

        /// <summary>
        /// available locales
        /// </summary>
        public LocalesData Locales
        {
            get { return Data[nameof(Locales)] as LocalesData; }
            internal set { UpsertData(value); }
        }

        /// <summary>
        /// perform the action 
        /// </summary>
        /// <param name="">the data for the action</param>
        public override bool Act(object data)
        {
            SpeechActionData actionData = data as SpeechActionData;
            if (actionData == null || string.IsNullOrEmpty(actionData.Text)) return false;

            SpeechOptions options = new SpeechOptions { 
                Locale = actionData.Locale,
                Volume = actionData.Volume,
                Pitch = actionData.Pitch
            };

            SpeakAsync(actionData.Text, options);

            return true;
        }

        /// <summary>
        /// text to speech
        /// </summary>
        public void SpeakAsync(string text, SpeechOptions settings = null)
        {
            Task.Run(async () =>
            {
                await TextToSpeech.SpeakAsync(text, settings);
            });
        }

        /// <summary>
        /// default SpeechOptions
        /// </summary>
        public static SpeechOptions DefaultSpeechOption = new SpeechOptions()
        {
            Volume = .75f,
            Pitch = 1.0f,
            //Locale = locale
        };

        private void GetLocalesAsync()
        {
            Task.Run(async () =>
            {
                var locales = await TextToSpeech.GetLocalesAsync();
                Locales = new LocalesData { Id = nameof(Locales), Value = locales, Parent = this, TimeStamp = DateTime.UtcNow };
            });
        }
    }
}
