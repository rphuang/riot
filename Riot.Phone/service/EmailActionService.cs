using Riot.Phone.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// sends email on the phone device
    /// </summary>
    public class EmailActionService : BaseActionService
    {
        /// <summary>
        /// constructor
        /// </summary>
        public EmailActionService(IotNode parent) : base("Email", parent)
        {
        }

        /// <summary>
        /// perform the action 
        /// </summary>
        /// <param name="">the data for the action</param>
        public override bool Act(object data)
        {
            EmailActionData actionData = data as EmailActionData;
            if (actionData == null || actionData.To == null || actionData.To.Count == 0) return false;

            EmailMessage message = new EmailMessage {
                Subject = actionData.Subject,
                Body = actionData.Body,
                To = actionData.To,
                Cc = actionData.Cc,
                Bcc = actionData.Bcc,
                BodyFormat = (EmailBodyFormat) actionData.BodyFormat
            };
            SendEmail(message);

            return true;
        }

        /// <summary>
        /// text to speech
        /// </summary>
        public void SendEmail(EmailMessage emailMessage)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Email.ComposeAsync(emailMessage);
                }
                catch (FeatureNotSupportedException ex)
                {
                    // email is not supported on this device.
                }
                catch (Exception ex)
                {
                    // Other error has occurred.
                }
            });
        }
    }
}
