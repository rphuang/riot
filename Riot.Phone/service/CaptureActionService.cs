using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Riot.Phone.Service
{
    /// <summary>
    /// capture photo/video on the phone device
    /// </summary>
    public class CaptureActionService : BaseActionService
    {
        /// <summary>
        /// constructor
        /// </summary>
        public CaptureActionService(IotNode parent) : base("Capture", parent)
        {
            RequireUiThread = true;
        }

        /// <summary>
        /// perform the action 
        /// </summary>
        /// <param name="">the data for the action</param>
        public override bool Act(object data)
        {
            CaptureActionData actionData = data as CaptureActionData;
            bool captureVideo = string.Equals("video", actionData?.Form, StringComparison.OrdinalIgnoreCase);

            Capture(captureVideo);

            return true;
        }

        /// <summary>
        /// capture
        /// </summary>
        public void Capture(bool captureVideo, MediaPickerOptions options = null)
        {
            FileResult file = null;
            Task.Run(async () =>
            {
                try
                {
                    if (captureVideo)
                    {
                        file = await MediaPicker.CaptureVideoAsync(options);
                    }
                    else
                    {
                        file = await MediaPicker.CapturePhotoAsync(options);
                    }
                    await LoadPhotoAsync(file);
                }
                catch (Exception ex)
                {
                    // Other error has occurred.
                }
            });
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);
        }
    }
}
