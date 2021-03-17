using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// vibrate the phone device
    /// </summary>
    public class VibrateActionService : BaseActionService
    {
        /// <summary>
        /// constructor
        /// </summary>
        public VibrateActionService(IotNode parent) : base("Vibrate", parent)
        {
            RequireUiThread = false;
        }

        /// <summary>
        /// perform the action 
        /// </summary>
        /// <param name="">the data for the action</param>
        public override bool Act(object data)
        {
            DoubleData actionData = data as DoubleData;
            double duration = 0;
            if (actionData != null) duration = actionData.Value;

            Vibrate(duration);

            return true;
        }

        /// <summary>
        /// text to speech
        /// </summary>
        public void Vibrate(double duration)
        {
                try
                {
                    Vibration.Vibrate(duration);
                }
                catch (FeatureNotSupportedException ex)
                {
                    // Sms is not supported on this device.
                }
                catch (Exception ex)
                {
                    // Other error has occurred.
                }
        }
    }
}
