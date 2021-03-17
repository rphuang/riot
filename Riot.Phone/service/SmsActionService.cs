using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// sends SMS text message on the phone device
    /// </summary>
    public class SmsActionService : BaseActionService
    {
        /// <summary>
        /// constructor
        /// </summary>
        public SmsActionService(IotNode parent) : base("SMS", parent)
        {
        }

        /// <summary>
        /// perform the action 
        /// </summary>
        /// <param name="">the data for the action</param>
        public override bool Act(object data)
        {
            SmsActionData actionData = data as SmsActionData;
            if (actionData == null || string.IsNullOrEmpty(actionData.Text) || actionData.Recipients == null || actionData.Recipients.Length == 0) return false;

            SendSmsMessage(actionData.Text, actionData.Recipients);

            return true;
        }

        /// <summary>
        /// text to speech
        /// </summary>
        public void SendSmsMessage(string text, string[] recipients)
        {
            Task.Run(async () =>
            {
                try
                {
                    var message = new SmsMessage(text, recipients);
                    await Sms.ComposeAsync(message);
                }
                catch (FeatureNotSupportedException ex)
                {
                    // Sms is not supported on this device.
                }
                catch (Exception ex)
                {
                    // Other error has occurred.
                }
            });
        }
    }
}
