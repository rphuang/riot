using Android.OS;
using PlatformLib;
using System.IO;
using PiCar.Droid;
using PiCar.Models;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidFolderPath))]

namespace PiCar.Droid
{
    class AndroidFolderPath : IFolderPath
    {
        /// <summary>
        /// Get full path with the specified folderName
        /// </summary>
        public string GetFullFolderPath(string folderName)
        {
            string dir;
            if (string.IsNullOrWhiteSpace(folderName))
            {
                dir = Environment.ExternalStorageDirectory.AbsolutePath;
            }
            else
            {
                dir = Environment.GetExternalStoragePublicDirectory(folderName).AbsolutePath;
            }

            return dir;
        }

        /// <summary>
        /// Get full path with the specified rootFolder and folderName
        /// </summary>
        public string GetFullFolderPath(string rootFolder, string folderName)
        {
            string root = GetFullFolderPath(rootFolder);
            return Path.Combine(root, folderName);
        }
    }
}